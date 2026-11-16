/** Holiday's computed date from: GET /api/calendario/festivos/{countryId}/{year} */
export interface HolidayDate {
  id: number;
  holiday: string;
  date: string;
}
