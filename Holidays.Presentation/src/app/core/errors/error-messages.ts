import { HttpErrorResponse } from '@angular/common/http';

/** Per-call messages by HTTP status, e.g. `{ 409: 'No se puede eliminar...' }`. */
export type ErrorMessageOverrides = Partial<Record<number, string>>;

const CONNECTION_ERROR = 'No se pudo conectar con el servidor.';
const UNEXPECTED_ERROR = 'Ocurrió un error inesperado.';

const DEFAULT_MESSAGES: Record<number, string> = {
  0: CONNECTION_ERROR, // browser couldn't reach the server
  400: 'Los datos enviados no son válidos.',
  404: 'El registro no existe.',
  409: 'La operación no es posible porque entra en conflicto con otros datos.',
  500: 'Ocurrió un error en el servidor.',
  502: CONNECTION_ERROR, // dev proxy: API is not running
  504: CONNECTION_ERROR,
};

/** Spanish message for a failed API call. API messages are in English, so they're not shown. */
export function toErrorMessage(error: unknown, overrides: ErrorMessageOverrides = {}): string {
  if (!(error instanceof HttpErrorResponse)) return UNEXPECTED_ERROR;

  return overrides[error.status] ?? DEFAULT_MESSAGES[error.status] ?? UNEXPECTED_ERROR;
}
