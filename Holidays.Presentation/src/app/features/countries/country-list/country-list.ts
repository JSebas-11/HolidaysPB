import { Component, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { filter, switchMap } from 'rxjs';
import { CountryOverview } from '../../../core/models/country';
import { CountryService } from '../../../core/services/country-service';
import { Confirmation } from '../../../shared/confirm-dialog/confirmation';
import { EmptyState } from '../../../shared/empty-state/empty-state';
import { Notifier } from '../../../shared/notifier';

@Component({
  selector: 'app-country-list',
  imports: [
    RouterLink,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatTooltipModule,
    EmptyState,
  ],
  templateUrl: './country-list.html',
  styleUrl: './country-list.scss',
})
export class CountryList implements OnInit {
  private readonly countryService = inject(CountryService);
  private readonly confirmation = inject(Confirmation);
  private readonly notifier = inject(Notifier);

  protected readonly countries = signal<CountryOverview[]>([]);
  protected readonly loading = signal(true);
  protected readonly columns = ['id', 'name', 'actions'];

  ngOnInit(): void {
    this.loadCountries();
  }

  protected deleteCountry(country: CountryOverview): void {
    this.confirmation
      .ask({
        title: 'Eliminar país',
        message: `¿Seguro que desea eliminar "${country.name}"?`,
        confirmText: 'Eliminar',
        destructive: true,
      })
      .pipe(
        filter((confirmed) => confirmed), // stop here if the user cancelled
        switchMap(() => this.countryService.delete(country.id)), // then call the API
      )
      .subscribe({
        next: () => {
          this.notifier.success('País eliminado.');
          this.loadCountries();
        },
        error: (error) =>
          this.notifier.error(error, {
            409: 'No se puede eliminar: el país tiene festivos asociados.',
          }),
      });
  }

  private loadCountries(): void {
    this.loading.set(true);
    this.countryService.getAll().subscribe({
      next: (countries) => {
        this.countries.set(countries);
        this.loading.set(false);
      },
      error: (error) => {
        this.notifier.error(error);
        this.loading.set(false);
      },
    });
  }
}
