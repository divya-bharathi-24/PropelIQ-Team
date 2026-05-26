import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';

export interface TimeSlot {
  id: string;
  providerId: string;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
}

export interface SlotGroup {
  date: string;
  slots: TimeSlot[];
}

export interface BookingConfirmation {
  appointmentId: string;
  scheduledAt: string;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class BookingService {
  private readonly url = `${environment.apiBaseUrl}/slots`;
  private readonly bookingUrl = `${environment.apiBaseUrl}/booking`;

  constructor(private readonly http: HttpClient) {}

  getAvailableSlots(params: {
    providerId?: string;
    specialty?: string;
    dateFrom?: string;
    dateTo?: string;
  }): Observable<SlotGroup[]> {
    let httpParams = new HttpParams();
    if (params.providerId) httpParams = httpParams.set('providerId', params.providerId);
    if (params.specialty) httpParams = httpParams.set('specialty', params.specialty);
    if (params.dateFrom) httpParams = httpParams.set('dateFrom', params.dateFrom);
    if (params.dateTo) httpParams = httpParams.set('dateTo', params.dateTo);

    return this.http.get<SlotGroup[]>(this.url, { params: httpParams });
  }

  bookSlot(slotId: string, reasonForVisit?: string): Observable<BookingConfirmation> {
    return this.http.post<BookingConfirmation>(this.bookingUrl, { slotId, reasonForVisit });
  }
}
