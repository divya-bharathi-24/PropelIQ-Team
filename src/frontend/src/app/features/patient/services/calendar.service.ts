import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';

export interface CalendarConnectionStatus {
  provider: string;
  status: string;
  icsFeedUrl: string | null;
}

export interface CalendarSyncResponse {
  status: string;
  icsFeedUrl: string | null;
  googleCalendarStatus: string | null;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class CalendarService {
  private readonly url = `${environment.apiBaseUrl}/calendar`;

  constructor(private readonly http: HttpClient) {}

  enableIcsFeed(): Observable<CalendarSyncResponse> {
    return this.http.post<CalendarSyncResponse>(`${this.url}/enable-ics`, {});
  }

  getGoogleAuthUrl(): Observable<{ authUrl: string }> {
    return this.http.get<{ authUrl: string }>(`${this.url}/google/auth`);
  }

  disconnect(provider: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.url}/disconnect/${provider}`, {});
  }

  getSyncStatus(): Observable<CalendarConnectionStatus[]> {
    return this.http.get<CalendarConnectionStatus[]>(`${this.url}/status`);
  }
}
