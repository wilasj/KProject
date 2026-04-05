import {inject, Injectable, isDevMode} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, Subject, fromEvent, takeUntil} from 'rxjs';
import {map} from 'rxjs/operators';
import {ImportTask} from '@models/import';
import {mockSseSubject} from './mock.interceptor';

@Injectable({
  providedIn: 'root',
})
export class ImportService {
  private http = inject(HttpClient);
  private sseDisconnect$ = new Subject<void>();
  private eventSource: EventSource | null = null;

  getTasks(): Observable<ImportTask[]> {
    return this.http.get<ImportTask[]>('/api/imports');
  }

  getTask(id: string): Observable<ImportTask> {
    return this.http.get<ImportTask>(`/api/imports/${id}`);
  }

  uploadFiles(files: File[]): Observable<ImportTask[]> {
    const formData = new FormData();
    for (const file of files) {
      formData.append('files', file);
    }
    return this.http.post<ImportTask[]>('/api/imports', formData);
  }

  confirmTask(id: string): Observable<void> {
    return this.http.post<void>(`/api/imports/${id}/confirm`, {});
  }

  deleteTask(id: string): Observable<void> {
    return this.http.delete<void>(`/api/imports/${id}`);
  }

  connectSse(): Observable<Partial<ImportTask>> {
    this.sseDisconnect$ = new Subject<void>();

    if (isDevMode()) {
      return mockSseSubject.asObservable().pipe(takeUntil(this.sseDisconnect$));
    }

    this.eventSource = new EventSource('/api/imports/events');
    return fromEvent<MessageEvent>(this.eventSource, 'message').pipe(
      map(event => JSON.parse(event.data) as Partial<ImportTask>),
      takeUntil(this.sseDisconnect$),
    );
  }

  disconnectSse(): void {
    this.sseDisconnect$.next();
    this.sseDisconnect$.complete();
    this.eventSource?.close();
    this.eventSource = null;
  }
}
