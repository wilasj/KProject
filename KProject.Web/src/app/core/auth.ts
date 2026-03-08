import {inject, Injectable, signal} from '@angular/core';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {catchError, map, Observable, of} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private httpClient = inject(HttpClient);
  public isLoggedIn = signal<boolean>(false);
  public email = signal<string | null>(null);

  register(email: string, password: string, conviteToken: string): Observable<Result<void>>{
    return this.httpClient.post<Result<void>>('/api/users/register', {email, password, conviteToken}).pipe(
      map(() => ({success: true as const})),
      catchError((err: HttpErrorResponse) => of({success: false as const, errors: err.error}))
    );
  }

  criaInvite(): Observable<string> {
    return this.httpClient.post<{ token: string }>('/api/convites', {}).pipe(
      map(res => res.token)
    );
  }

  login(email: string, password: string): Observable<Result<void>>{
    return this.httpClient.post<Result<void>>('/api/users/login', {email, password}).pipe(
      map(() => {
        this.isLoggedIn.set(true);
        this.email.set(email);
        return {success: true as const};
      }),
      catchError((err: HttpErrorResponse) => of({success: false as const, errors: err.error}))
    );
  }

  me(): Observable<void> {
    return this.httpClient.get<{ email: string }>('/api/users/me').pipe(
      map((res) => {
        this.isLoggedIn.set(true);
        this.email.set(res.email);
      }),
      catchError(() => {
        this.isLoggedIn.set(false);
        return of(undefined);
      })
    );
  }

  logout(): Observable<void> {
    return this.httpClient.post<void>('/api/users/logout', {}).pipe(
      map(() => {
        this.isLoggedIn.set(false);
        this.email.set(null);
      }),
      catchError(() => {
        this.isLoggedIn.set(false);
        this.email.set(null);
        return of(undefined);
      })
    );
  }
}
