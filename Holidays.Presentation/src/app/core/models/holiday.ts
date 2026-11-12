import { CountryOverview } from './country';
import { HolidayTypeOverview } from './holiday-type';

/** Holiday id + name, used inside CountryDetails and HolidayTypeDetails. */
export interface RelatedHolidaySummary {
  id: number;
  name: string;
}

/** Item of GET /api/festivos. */
export interface HolidayOverview {
  id: number;
  name: string;
  day: number;
  month: number;
  easterDays: number;
  countryId: number;
  typeId: number;
}

/** GET /api/festivos/{id}. */
export interface HolidayDetails {
  id: number;
  name: string;
  day: number;
  month: number;
  easterDays: number;
  country: CountryOverview;
  type: HolidayTypeOverview;
}

/**
 * Body of POST and PUT /api/festivos.
 * Date-based types use day/month; Easter-based types use easterDays (see HolidayTypeId).
 */
export interface HolidayRequest {
  name: string;
  day: number;
  month: number;
  easterDays: number;
  countryId: number;
  typeId: number;
}
