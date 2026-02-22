import {inject, Injectable, signal} from '@angular/core';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {catchError, map, Observable, of} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private httpClient = inject(HttpClient);
  public isLoggedIn = signal<boolean>(false);

  register(email: string, password: string): Observable<Result<void>>{
    return this.httpClient.post<Result<void>>('/api/users/register', {email, password}).pipe(
      map(() => ({success: true as const})),
      catchError((err: HttpErrorResponse) => of({success: false as const, errors: err.error}))
    );
  }

  login(email: string, password: string): Observable<Result<void>>{
    return this.httpClient.post<Result<void>>('/api/users/login', {email, password}).pipe(
      map(() => {
        this.isLoggedIn.set(true);

        return {success: true as const};
      }),
      catchError((err: HttpErrorResponse) => of({success: false as const, errors: err.error}))
    );
  }

  me(): Observable<void> {
    return this.httpClient.get<void>('/api/users/me').pipe(
      map(() => {
        this.isLoggedIn.set(true);
      }),
      catchError(() => {
        this.isLoggedIn.set(false);
        return of(undefined);
      })
    );
  }
}
