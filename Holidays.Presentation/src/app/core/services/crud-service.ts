import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';

/**
 * Calls shared by every resource with the same CRUD shape
 * (GET all, GET by id, POST, PUT, DELETE). Same idea as a generic repository in C#.
 *
 * Observables are lazy: no request is sent until a component calls `.subscribe()`.
 */
export abstract class CrudService<TOverview, TDetails, TRequest> {
  protected readonly http = inject(HttpClient);
  protected abstract readonly baseUrl: string;

  getAll(): Observable<TOverview[]> {
    return this.http.get<TOverview[]>(this.baseUrl);
  }

  getById(id: number): Observable<TDetails> {
    return this.http.get<TDetails>(`${this.baseUrl}/${id}`);
  }

  /** Returns the new id. */
  create(request: TRequest): Observable<number> {
    return this.http.post<number>(this.baseUrl, request);
  }

  update(id: number, request: TRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
