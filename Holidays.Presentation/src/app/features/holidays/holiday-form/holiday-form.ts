import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { Router, RouterLink } from '@angular/router';
import { forkJoin, Observable } from 'rxjs';
import { CountryOverview } from '../../../core/models/country';
import { HolidayRequest } from '../../../core/models/holiday';
import {
  HolidayTypeOverview,
  isEasterBased,
  SUPPORTED_HOLIDAY_TYPE_IDS,
} from '../../../core/models/holiday-type';
import { CountryService } from '../../../core/services/country-service';
import { HolidayService } from '../../../core/services/holiday-service';
import { HolidayTypeService } from '../../../core/services/holiday-type-service';
import { MONTH_NAMES } from '../../../core/utils/months';
import { dayInMonthValidator } from '../../../core/validators/holiday-date';
import { Notifier } from '../../../shared/notifier';

const INTEGER_PATTERN = /^-?\d+$/;

@Component({
  selector: 'app-holiday-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
  ],
  templateUrl: './holiday-form.html',
  styleUrl: './holiday-form.scss',
})
export class HolidayForm implements OnInit {
  /** Route param `:id`, filled by the router. Undefined on /festivos/nuevo. */
  readonly id = input<string>();

  private readonly holidayService = inject(HolidayService);
  private readonly countryService = inject(CountryService);
  private readonly holidayTypeService = inject(HolidayTypeService);
  private readonly notifier = inject(Notifier);
  private readonly router = inject(Router);

  protected readonly monthNames = MONTH_NAMES;
  protected readonly holidayId = computed(() => (this.id() ? Number(this.id()) : null));
  protected readonly isEdit = computed(() => this.holidayId() !== null);
  protected readonly saving = signal(false);
  protected readonly countries = signal<CountryOverview[]>([]);
  protected readonly holidayTypes = signal<HolidayTypeOverview[]>([]);

  /** Drives which fields the template shows (day/month vs. days from Easter). */
  private readonly selectedTypeId = signal<number | null>(null);
  protected readonly usesEasterDays = computed(() => {
    const typeId = this.selectedTypeId();
    return typeId !== null && isEasterBased(typeId);
  });

  protected readonly form = new FormGroup({
    name: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.pattern(/\S/), Validators.maxLength(100)],
    }),
    countryId: new FormControl<number | null>(null, Validators.required),
    typeId: new FormControl<number | null>(null, Validators.required),
    day: new FormControl<number | null>(null, [Validators.required, dayInMonthValidator]),
    month: new FormControl<number | null>(null, Validators.required),
    // Disabled until an Easter-based type is picked. Disabled controls skip validation.
    easterDays: new FormControl<number | null>({ value: 0, disabled: true }, [
      Validators.required,
      Validators.pattern(INTEGER_PATTERN),
    ]),
  });

  constructor() {
    // takeUntilDestroyed: unsubscribe automatically when the page is closed
    this.form.controls.typeId.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe((typeId) => this.applyTypeRules(typeId));

    // The valid day range depends on the month, so re-check the day when the month changes
    this.form.controls.month.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.form.controls.day.updateValueAndValidity());
  }

  ngOnInit(): void {
    this.loadOptions();
    const id = this.holidayId();
    if (id !== null) this.loadHoliday(id);
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    // getRawValue() includes disabled controls; validators already guarantee non-null values (!)
    const { name, countryId, typeId, day, month, easterDays } = this.form.getRawValue();
    const usesEasterDays = this.usesEasterDays();
    const request: HolidayRequest = {
      name: name.trim(),
      countryId: countryId!,
      typeId: typeId!,
      day: usesEasterDays ? 0 : day!,
      month: usesEasterDays ? 0 : month!,
      easterDays: usesEasterDays ? easterDays! : 0,
    };

    const id = this.holidayId();
    const save$: Observable<unknown> =
      id === null ? this.holidayService.create(request) : this.holidayService.update(id, request);

    this.saving.set(true);
    save$.subscribe({
      next: () => {
        this.notifier.success('Festivo guardado.');
        this.router.navigate(['/festivos']);
      },
      error: (error) => {
        this.notifier.error(error);
        this.saving.set(false);
      },
    });
  }

  /** Enable the fields the selected type uses and disable the rest. */
  private applyTypeRules(typeId: number | null): void {
    this.selectedTypeId.set(typeId);
    const { day, month, easterDays } = this.form.controls;

    if (typeId !== null && isEasterBased(typeId)) {
      day.disable();
      month.disable();
      easterDays.enable();
    } else {
      day.enable();
      month.enable();
      easterDays.disable();
    }
  }

  private loadOptions(): void {
    forkJoin({
      countries: this.countryService.getAll(),
      holidayTypes: this.holidayTypeService.getAll(),
    }).subscribe({
      next: ({ countries, holidayTypes }) => {
        this.countries.set(countries);
        // Only types the API can compute dates for (see docs/API.md)
        this.holidayTypes.set(
          holidayTypes.filter((holidayType) => SUPPORTED_HOLIDAY_TYPE_IDS.includes(holidayType.id)),
        );
      },
      error: (error) => this.notifier.error(error),
    });
  }

  private loadHoliday(id: number): void {
    this.holidayService.getById(id).subscribe({
      next: (holiday) => {
        const isSupportedType = SUPPORTED_HOLIDAY_TYPE_IDS.includes(holiday.type.id);
        this.form.patchValue({
          name: holiday.name,
          countryId: holiday.country.id,
          typeId: isSupportedType ? holiday.type.id : null, // unsupported type: user must pick one
          day: holiday.day || null, // Easter-based rows store 0: show empty if the type is changed
          month: holiday.month || null,
          easterDays: holiday.easterDays,
        });
      },
      error: (error) => {
        this.notifier.error(error);
        this.router.navigate(['/festivos']);
      },
    });
  }
}
