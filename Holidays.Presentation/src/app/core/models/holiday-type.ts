import { RelatedHolidaySummary } from './holiday';

/** Item of GET /api/tipos. */
export interface HolidayTypeOverview {
  id: number;
  type: string;
}

/** GET /api/tipos/{id}. */
export interface HolidayTypeDetails extends HolidayTypeOverview {
  holidays: RelatedHolidaySummary[];
}

/** Body of POST and PUT /api/tipos. */
export interface HolidayTypeRequest {
  type: string;
}

/**
 * Seed type ids. The API only knows how to compute dates for these
 * (mirrors HolidayTypeIds in the backend).
 */
export enum HolidayTypeId {
  Fixed = 1,
  BridgeLaw = 2,
  EasterBased = 3,
  EasterBasedAndBridgeLaw = 4,
  FridayBridgeLaw = 5,
}

/** Types a holiday can be assigned to. Types created later have no date rule. */
export const SUPPORTED_HOLIDAY_TYPE_IDS: readonly number[] = [
  HolidayTypeId.Fixed,
  HolidayTypeId.BridgeLaw,
  HolidayTypeId.EasterBased,
  HolidayTypeId.EasterBasedAndBridgeLaw,
  HolidayTypeId.FridayBridgeLaw,
];

export function isEasterBased(typeId: number): boolean {
  return typeId === HolidayTypeId.EasterBased || typeId === HolidayTypeId.EasterBasedAndBridgeLaw;
}
