import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { filter, forkJoin, switchMap } from 'rxjs';
import { CountryOverview } from '../../../core/models/country';
import { HolidayOverview } from '../../../core/models/holiday';
import { HolidayTypeOverview, isEasterBased } from '../../../core/models/holiday-type';
import { CountryService } from '../../../core/services/country-service';
import { HolidayService } from '../../../core/services/holiday-service';
import { HolidayTypeService } from '../../../core/services/holiday-type-service';
import { MONTH_NAMES } from '../../../core/utils/months';
import { Confirmation } from '../../../shared/confirm-dialog/confirmation';
import { Notifier } from '../../../shared/notifier';

/** What the table shows: names instead of ids, and the rule as text. */
interface HolidayRow {
  id: number;
  name: string;
  countryId: number;
  countryName: string;
  typeName: string;
  rule: string;
}

@Component({
  selector: 'app-holiday-list',
  imports: [
    RouterLink,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatFormFieldModule,
    MatSelectModule,
  ],
  templateUrl: './holiday-list.html',
  styleUrl: './holiday-list.scss',
})
export class HolidayList implements OnInit {
  private readonly holidayService = inject(HolidayService);
  private readonly countryService = inject(CountryService);
  private readonly holidayTypeService = inject(HolidayTypeService);
  private readonly confirmation = inject(Confirmation);
  private readonly notifier = inject(Notifier);

  protected readonly rows = signal<HolidayRow[]>([]);
  protected readonly countries = signal<CountryOverview[]>([]);
  protected readonly loading = signal(true);
  protected readonly columns = ['name', 'country', 'type', 'rule', 'actions'];

  /** Country filter; null = all countries. */
  protected readonly selectedCountryId = signal<number | null>(null);
  protected readonly visibleRows = computed(() => {
    const countryId = this.selectedCountryId();
    return countryId === null
      ? this.rows()
      : this.rows().filter((row) => row.countryId === countryId);
  });

  ngOnInit(): void {
    this.loadHolidays();
  }

  protected deleteHoliday(row: HolidayRow): void {
    this.confirmation
      .ask({
        title: 'Eliminar festivo',
        message: `¿Seguro que desea eliminar "${row.name}" (${row.countryName})?`,
        confirmText: 'Eliminar',
      })
      .pipe(
        filter((confirmed) => confirmed),
        switchMap(() => this.holidayService.delete(row.id)),
      )
      .subscribe({
        next: () => {
          this.notifier.success('Festivo eliminado.');
          this.loadHolidays();
        },
        error: (error) => this.notifier.error(error),
      });
  }

  private loadHolidays(): void {
    this.loading.set(true);
    // Three requests in parallel; emits once all three have answered (like Task.WhenAll)
    forkJoin({
      holidays: this.holidayService.getAll(),
      countries: this.countryService.getAll(),
      holidayTypes: this.holidayTypeService.getAll(),
    }).subscribe({
      next: ({ holidays, countries, holidayTypes }) => {
        this.countries.set(countries);
        this.rows.set(this.toRows(holidays, countries, holidayTypes));
        this.loading.set(false);
      },
      error: (error) => {
        this.notifier.error(error);
        this.loading.set(false);
      },
    });
  }

  private toRows(
    holidays: HolidayOverview[],
    countries: CountryOverview[],
    holidayTypes: HolidayTypeOverview[],
  ): HolidayRow[] {
    // id -> name lookups, like ToDictionary()
    const countryNames = new Map(countries.map((country) => [country.id, country.name]));
    const typeNames = new Map(
      holidayTypes.map((holidayType) => [holidayType.id, holidayType.type]),
    );

    return holidays.map((holiday) => ({
      id: holiday.id,
      name: holiday.name,
      countryId: holiday.countryId,
      countryName: countryNames.get(holiday.countryId) ?? '?',
      typeName: typeNames.get(holiday.typeId) ?? '?',
      rule: this.describeRule(holiday),
    }));
  }

  /** "25 de diciembre" or "Pascua +61 días". */
  private describeRule(holiday: HolidayOverview): string {
    if (!isEasterBased(holiday.typeId))
      return `${holiday.day} de ${MONTH_NAMES[holiday.month - 1]}`;

    if (holiday.easterDays === 0) return 'Domingo de Pascua';
    const sign = holiday.easterDays > 0 ? '+' : '';
    return `Pascua ${sign}${holiday.easterDays} días`;
  }
}
