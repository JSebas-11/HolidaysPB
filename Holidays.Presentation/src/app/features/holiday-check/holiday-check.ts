import { DatePipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { forkJoin } from 'rxjs';
import { HolidayDate } from '../../core/models/calendar';
import { CountryOverview } from '../../core/models/country';
import { CalendarService } from '../../core/services/calendar-service';
import { CountryService } from '../../core/services/country-service';
import { toLocalIsoDate } from '../../core/utils/dates';
import { Notifier } from '../../shared/notifier';

/** Snapshot of the last check, so the result doesn't change while the user edits the form. */
interface CheckResult {
  date: Date;
  isoDate: string;
  countryName: string;
  isHoliday: boolean;
  /** Names of the holidays that fall on `date`. */
  holidayNames: string[];
  /** All holidays of the country for that year, sorted by date. */
  yearHolidays: HolidayDate[];
}

@Component({
  selector: 'app-holiday-check',
  imports: [
    DatePipe,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatDatepickerModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatTableModule,
  ],
  templateUrl: './holiday-check.html',
  styleUrl: './holiday-check.scss',
})
export class HolidayCheck implements OnInit {
  private readonly calendarService = inject(CalendarService);
  private readonly countryService = inject(CountryService);
  private readonly notifier = inject(Notifier);

  // Easter calculation in the API is only correct in this range (see docs/API.md)
  protected readonly minDate = new Date(1900, 0, 1);
  protected readonly maxDate = new Date(2099, 11, 31);

  protected readonly countries = signal<CountryOverview[]>([]);
  protected readonly checking = signal(false);
  protected readonly result = signal<CheckResult | null>(null);
  protected readonly columns = ['date', 'holiday'];

  protected readonly form = new FormGroup({
    countryId: new FormControl<number | null>(null, Validators.required),
    date: new FormControl<Date | null>(new Date(), Validators.required),
  });

  ngOnInit(): void {
    this.countryService.getAll().subscribe({
      next: (countries) => this.countries.set(countries),
      error: (error) => this.notifier.error(error),
    });
  }

  protected check(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { countryId, date } = this.form.getRawValue();
    const country = this.countries().find((c) => c.id === countryId)!;
    const selectedDate = date!;

    this.checking.set(true);
    forkJoin({
      isHoliday: this.calendarService.isHoliday(country.id, selectedDate),
      yearHolidays: this.calendarService.getHolidays(country.id, selectedDate.getFullYear()),
    }).subscribe({
      next: ({ isHoliday, yearHolidays }) => {
        const isoDate = toLocalIsoDate(selectedDate);
        // "yyyy-MM-dd" strings sort correctly as text
        const sortedHolidays = [...yearHolidays].sort((a, b) => a.date.localeCompare(b.date));

        this.result.set({
          date: selectedDate,
          isoDate,
          countryName: country.name,
          isHoliday,
          holidayNames: sortedHolidays.filter((h) => h.date === isoDate).map((h) => h.holiday),
          yearHolidays: sortedHolidays,
        });
        this.checking.set(false);
      },
      error: (error) => {
        this.notifier.error(error, {
          409: 'No se puede calcular: el país tiene festivos con un tipo sin regla de cálculo.',
        });
        this.result.set(null);
        this.checking.set(false);
      },
    });
  }
}
