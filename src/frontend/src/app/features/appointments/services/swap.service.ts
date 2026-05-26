import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';

export interface SwapRequestPayload {
  originalSlotId: string;
  providerId: string;
  preferredDate: string;
  preferredTimeStart?: string;
  preferredTimeEnd?: string;
}

export interface SwapResponse {
  swapRequestId: string;
  status: string;
  queuePosition: number | null;
}

@Injectable({ providedIn: 'root' })
export class SwapService {
  private readonly url = `${environment.apiBaseUrl}/swap`;

  constructor(private readonly http: HttpClient) {}

  createSwapRequest(payload: SwapRequestPayload): Observable<SwapResponse> {
    return this.http.post<SwapResponse>(`${this.url}/request`, payload);
  }

  acceptSwap(swapRequestId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.url}/accept`, { swapRequestId });
  }

  cancelSwap(swapRequestId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.url}/${swapRequestId}/cancel`, {});
  }

  getSwapStatus(swapRequestId: string): Observable<{ swapRequestId: string; queuePosition: number }> {
    return this.http.get<{ swapRequestId: string; queuePosition: number }>(`${this.url}/${swapRequestId}/status`);
  }
}
