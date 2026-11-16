import { Component, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { filter, switchMap } from 'rxjs';
import { HolidayTypeOverview } from '../../../core/models/holiday-type';
import { HolidayTypeService } from '../../../core/services/holiday-type-service';
import { Confirmation } from '../../../shared/confirm-dialog/confirmation';
import { EmptyState } from '../../../shared/empty-state/empty-state';
import { Notifier } from '../../../shared/notifier';

@Component({
  selector: 'app-holiday-type-list',
  imports: [
    RouterLink,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatTooltipModule,
    EmptyState,
  ],
  templateUrl: './holiday-type-list.html',
  styleUrl: './holiday-type-list.scss',
})
export class HolidayTypeList implements OnInit {
  private readonly holidayTypeService = inject(HolidayTypeService);
  private readonly confirmation = inject(Confirmation);
  private readonly notifier = inject(Notifier);

  protected readonly holidayTypes = signal<HolidayTypeOverview[]>([]);
  protected readonly loading = signal(true);
  protected readonly columns = ['id', 'type', 'actions'];

  ngOnInit(): void {
    this.loadHolidayTypes();
  }

  protected deleteHolidayType(holidayType: HolidayTypeOverview): void {
    this.confirmation
      .ask({
        title: 'Eliminar tipo de festivo',
        message: `¿Seguro que desea eliminar "${holidayType.type}"?`,
        confirmText: 'Eliminar',
        destructive: true,
      })
      .pipe(
        filter((confirmed) => confirmed),
        switchMap(() => this.holidayTypeService.delete(holidayType.id)),
      )
      .subscribe({
        next: () => {
          this.notifier.success('Tipo de festivo eliminado.');
          this.loadHolidayTypes();
        },
        error: (error) =>
          this.notifier.error(error, {
            409: 'No se puede eliminar: el tipo tiene festivos asociados.',
          }),
      });
  }

  private loadHolidayTypes(): void {
    this.loading.set(true);
    this.holidayTypeService.getAll().subscribe({
      next: (holidayTypes) => {
        this.holidayTypes.set(holidayTypes);
        this.loading.set(false);
      },
      error: (error) => {
        this.notifier.error(error);
        this.loading.set(false);
      },
    });
  }
}
