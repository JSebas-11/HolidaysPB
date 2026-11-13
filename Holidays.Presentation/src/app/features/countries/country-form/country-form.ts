import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { RelatedHolidaySummary } from '../../../core/models/holiday';
import { CountryRequest } from '../../../core/models/country';
import { CountryService } from '../../../core/services/country-service';
import { Notifier } from '../../../shared/notifier';

@Component({
  selector: 'app-country-form',
  imports: [ReactiveFormsModule, RouterLink, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './country-form.html',
  styleUrl: './country-form.scss',
})
export class CountryForm implements OnInit {
  /** Route param `:id`, filled by the router. Undefined on /paises/nuevo. */
  readonly id = input<string>();

  private readonly countryService = inject(CountryService);
  private readonly notifier = inject(Notifier);
  private readonly router = inject(Router);

  protected readonly countryId = computed(() => (this.id() ? Number(this.id()) : null));
  protected readonly isEdit = computed(() => this.countryId() !== null);
  protected readonly saving = signal(false);
  protected readonly holidays = signal<RelatedHolidaySummary[]>([]);

  protected readonly form = new FormGroup({
    name: new FormControl('', {
      nonNullable: true,
      // pattern(/\S/): at least one non-space character ("   " is not a name)
      validators: [Validators.required, Validators.pattern(/\S/), Validators.maxLength(100)],
    }),
  });

  ngOnInit(): void {
    const id = this.countryId();
    if (id !== null) this.loadCountry(id);
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched(); // show the field errors
      return;
    }

    const request: CountryRequest = { name: this.form.controls.name.value.trim() };
    const id = this.countryId();
    const save$: Observable<unknown> =
      id === null ? this.countryService.create(request) : this.countryService.update(id, request);

    this.saving.set(true);
    save$.subscribe({
      next: () => {
        this.notifier.success('País guardado.');
        this.router.navigate(['/paises']);
      },
      error: (error) => {
        this.notifier.error(error);
        this.saving.set(false);
      },
    });
  }

  private loadCountry(id: number): void {
    this.countryService.getById(id).subscribe({
      next: (country) => {
        this.form.setValue({ name: country.name });
        this.holidays.set(country.holidays);
      },
      error: (error) => {
        this.notifier.error(error);
        this.router.navigate(['/paises']);
      },
    });
  }
}
