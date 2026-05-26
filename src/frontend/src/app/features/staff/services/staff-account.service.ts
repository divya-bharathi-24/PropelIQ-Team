import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';

export interface CreatePatientResponse {
  patientId: string;
  temporaryPassword: string;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class StaffAccountService {
  private readonly baseUrl = `${environment.apiBaseUrl}/staffaccount`;

  constructor(private readonly http: HttpClient) {}

  createPatient(data: {
    firstName: string;
    lastName: string;
    phone: string;
    dateOfBirth: string;
  }): Observable<CreatePatientResponse> {
    return this.http.post<CreatePatientResponse>(
      `${this.baseUrl}/create-patient`, data
    );
  }
}
