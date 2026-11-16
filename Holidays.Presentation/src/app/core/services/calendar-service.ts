import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HolidayDate } from '../models/calendar';

@Injectable({ providedIn: 'root' })
export class CalendarService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/calendario';

  getHolidays(countryId: number, year: number): Observable<HolidayDate[]> {
    return this.http.get<HolidayDate[]>(`${this.baseUrl}/festivos/${countryId}/${year}`);
  }

  isHoliday(countryId: number, date: Date): Observable<boolean> {
    // Local getters on purpose: toISOString() converts to UTC and can shift the day
    const year = date.getFullYear();
    const month = date.getMonth() + 1; // JS months are 0-based
    const day = date.getDate();
    return this.http.get<boolean>(`${this.baseUrl}/verificar/${countryId}/${year}/${month}/${day}`);
  }
}
