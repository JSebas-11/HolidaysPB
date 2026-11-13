import { Injectable } from '@angular/core';
import {
  HolidayTypeDetails,
  HolidayTypeOverview,
  HolidayTypeRequest,
} from '../models/holiday-type';
import { CrudService } from './crud-service';

@Injectable({ providedIn: 'root' })
export class HolidayTypeService extends CrudService<
  HolidayTypeOverview,
  HolidayTypeDetails,
  HolidayTypeRequest
> {
  protected override readonly baseUrl = '/api/tipos';
}
