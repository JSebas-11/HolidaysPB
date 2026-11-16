import { Injectable } from '@angular/core';
import { CountryDetails, CountryOverview, CountryRequest } from '../models/country';
import { CrudService } from './crud-service';

@Injectable({ providedIn: 'root' })
export class CountryService extends CrudService<CountryOverview, CountryDetails, CountryRequest> {
  protected override readonly baseUrl = '/api/paises';
}
