import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { RelatedHolidaySummary } from '../../../core/models/holiday';
import { HolidayTypeRequest } from '../../../core/models/holiday-type';
import { HolidayTypeService } from '../../../core/services/holiday-type-service';
import { Notifier } from '../../../shared/notifier';

@Component({
  selector: 'app-holiday-type-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './holiday-type-form.html',
  styleUrl: './holiday-type-form.scss',
})
export class HolidayTypeForm implements OnInit {
  /** Route param `:id`, filled by the router. Undefined on /tipos/nuevo. */
  readonly id = input<string>();

  private readonly holidayTypeService = inject(HolidayTypeService);
  private readonly notifier = inject(Notifier);
  private readonly router = inject(Router);

  protected readonly holidayTypeId = computed(() => (this.id() ? Number(this.id()) : null));
  protected readonly isEdit = computed(() => this.holidayTypeId() !== null);
  protected readonly saving = signal(false);
  protected readonly holidays = signal<RelatedHolidaySummary[]>([]);

  protected readonly form = new FormGroup({
    type: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.pattern(/\S/), Validators.maxLength(100)],
    }),
  });

  ngOnInit(): void {
    const id = this.holidayTypeId();
    if (id !== null) this.loadHolidayType(id);
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const request: HolidayTypeRequest = { type: this.form.controls.type.value.trim() };
    const id = this.holidayTypeId();
    const save$: Observable<unknown> =
      id === null
        ? this.holidayTypeService.create(request)
        : this.holidayTypeService.update(id, request);

    this.saving.set(true);
    save$.subscribe({
      next: () => {
        this.notifier.success('Tipo de festivo guardado.');
        this.router.navigate(['/tipos']);
      },
      error: (error) => {
        this.notifier.error(error);
        this.saving.set(false);
      },
    });
  }

  private loadHolidayType(id: number): void {
    this.holidayTypeService.getById(id).subscribe({
      next: (holidayType) => {
        this.form.setValue({ type: holidayType.type });
        this.holidays.set(holidayType.holidays);
      },
      error: (error) => {
        this.notifier.error(error);
        this.router.navigate(['/tipos']);
      },
    });
  }
}
