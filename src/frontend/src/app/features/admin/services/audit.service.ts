import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';

export interface AuditLogEntry {
  id: string;
  eventType: string;
  email: string;
  userId: string;
  ipAddress: string;
  userAgent: string;
  successful: boolean;
  failureReason: string | null;
  attemptedAt: string;
}

export interface AuditLogResponse {
  items: AuditLogEntry[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

@Injectable({ providedIn: 'root' })
export class AuditService {
  private readonly baseUrl = `${environment.apiBaseUrl}/audit`;

  constructor(private readonly http: HttpClient) {}

  getAuditLog(params: {
    userId?: string;
    startDate?: string;
    endDate?: string;
    eventType?: string;
    page?: number;
    pageSize?: number;
    sortBy?: string;
    sortDirection?: string;
  }): Observable<AuditLogResponse> {
    let httpParams = new HttpParams();
    if (params.userId) httpParams = httpParams.set('userId', params.userId);
    if (params.startDate) httpParams = httpParams.set('startDate', params.startDate);
    if (params.endDate) httpParams = httpParams.set('endDate', params.endDate);
    if (params.eventType) httpParams = httpParams.set('eventType', params.eventType);
    if (params.page) httpParams = httpParams.set('page', params.page.toString());
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    if (params.sortBy) httpParams = httpParams.set('sortBy', params.sortBy);
    if (params.sortDirection) httpParams = httpParams.set('sortDirection', params.sortDirection);

    return this.http.get<AuditLogResponse>(this.baseUrl, { params: httpParams });
  }
}
