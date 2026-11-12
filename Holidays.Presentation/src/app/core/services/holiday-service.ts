import { Injectable } from '@angular/core';
import { HolidayDetails, HolidayOverview, HolidayRequest } from '../models/holiday';
import { CrudService } from './crud-service';

@Injectable({ providedIn: 'root' })
export class HolidayService extends CrudService<HolidayOverview, HolidayDetails, HolidayRequest> {
  protected override readonly baseUrl = '/api/festivos';
}
