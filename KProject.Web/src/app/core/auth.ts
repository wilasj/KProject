import {inject, Injectable} from '@angular/core';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {catchError, map, Observable, of} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private httpClient = inject(HttpClient);

  register(email: string, password: string): Observable<Result<void>>{
    return this.httpClient.post<Result<void>>('/api/users/register', {email, password}).pipe(
      map(() => ({success: true as const})),
      catchError((err: HttpErrorResponse) => of({success: false as const, errors: err.error}))
    );
  }
}
