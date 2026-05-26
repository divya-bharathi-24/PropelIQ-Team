import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';

export interface AppointmentListItem {
  id: string;
  providerId: string;
  scheduledAt: string;
  status: string;
  reasonForVisit: string | null;
  rescheduleCount: number;
}

export interface AppointmentListResponse {
  items: AppointmentListItem[];
  totalCount: number;
  page: number;
  pageSize: number;
}

@Injectable({ providedIn: 'root' })
export class AppointmentMgmtService {
  private readonly url = `${environment.apiBaseUrl}/appointments`;

  constructor(private readonly http: HttpClient) {}

  getAppointments(
    patientId: string,
    page: number = 1,
    status?: string,
    sortBy: string = 'date',
    descending: boolean = false,
  ): Observable<AppointmentListResponse> {
    let params = new HttpParams().set('page', page).set('sortBy', sortBy).set('descending', descending);
    if (status) params = params.set('status', status);

    return this.http.get<AppointmentListResponse>(`${this.url}/${patientId}`, { params });
  }

  reschedule(appointmentId: string, newSlotId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.url}/reschedule`, {
      appointmentId,
      newTimeSlotId: newSlotId,
    });
  }

  cancel(appointmentId: string, reason?: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.url}/${appointmentId}/cancel`, { reason });
  }

  downloadPdf(appointmentId: string): Observable<Blob> {
    return this.http.get(`${this.url}/${appointmentId}/pdf`, { responseType: 'blob' });
  }

  downloadIcs(appointmentId: string): Observable<Blob> {
    return this.http.get(`${this.url}/${appointmentId}/ics`, { responseType: 'blob' });
  }
}
