import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';

export interface InsuranceCheckRequest {
  insuranceProvider: string;
  policyNumber: string;
  groupNumber?: string;
  memberId: string;
  patientId: string;
}

export interface InsuranceCheckResponse {
  coverageStatus: string;
  copayEstimate: number | null;
  limitations: string | null;
  verifiedAt: string;
}

@Injectable({ providedIn: 'root' })
export class InsuranceService {
  private readonly url = `${environment.apiBaseUrl}/insurance`;

  constructor(private readonly http: HttpClient) {}

  checkEligibility(request: InsuranceCheckRequest): Observable<InsuranceCheckResponse> {
    return this.http.post<InsuranceCheckResponse>(`${this.url}/check`, request);
  }
}
