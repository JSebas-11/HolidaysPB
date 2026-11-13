/** "yyyy-MM-dd" from the local calendar date. toISOString() converts to UTC and can shift the day. */
export function toLocalIsoDate(date: Date): string {
  const month = String(date.getMonth() + 1).padStart(2, '0'); // JS months are 0-based
  const day = String(date.getDate()).padStart(2, '0');
  return `${date.getFullYear()}-${month}-${day}`;
}
