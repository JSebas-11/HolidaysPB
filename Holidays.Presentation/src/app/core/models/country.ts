import { RelatedHolidaySummary } from './holiday';

/** Item of GET /api/paises. */
export interface CountryOverview {
  id: number;
  name: string;
}

/** GET /api/paises/{id}. */
export interface CountryDetails extends CountryOverview {
  holidays: RelatedHolidaySummary[];
}

/** Body of POST and PUT /api/paises. */
export interface CountryRequest {
  name: string;
}
