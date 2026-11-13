import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

// Non-leap year
const VALIDATION_YEAR = 2001;

export function daysInMonth(month: number): number {
  // Day 0 of the next month = last day of `month` (JS months are 0-based, so `month` is "next")
  return new Date(VALIDATION_YEAR, month, 0).getDate();
}

export const dayInMonthValidator: ValidatorFn = (
  control: AbstractControl,
): ValidationErrors | null => {
  const day = control.value as number | null;
  if (day === null) return null; // empty is handled by Validators.required

  const month = control.parent?.get('month')?.value as number | null | undefined;
  const maxDay = month ? daysInMonth(month) : 31;

  const isValid = Number.isInteger(day) && day >= 1 && day <= maxDay;
  return isValid ? null : { dayInMonth: { maxDay } };
};
