import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ErrorMessageOverrides, toErrorMessage } from '../core/errors/error-messages';

/** Short messages at the bottom of the screen (Material snackbar). */
@Injectable({ providedIn: 'root' })
export class Notifier {
  private readonly snackBar = inject(MatSnackBar);

  success(message: string): void {
    this.snackBar.open(message, 'Cerrar', { duration: 3000 });
  }

  error(error: unknown, overrides?: ErrorMessageOverrides): void {
    console.error(error); // full details for debugging in the browser console
    this.snackBar.open(toErrorMessage(error, overrides), 'Cerrar', { duration: 6000 });
  }
}
