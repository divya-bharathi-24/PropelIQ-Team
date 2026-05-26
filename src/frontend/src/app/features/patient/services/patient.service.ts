import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';

export interface PatientProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string | null;
  dateOfBirth: string | null;
  profilePhotoPath: string | null;
}

export interface DashboardData {
  upcomingAppointments: AppointmentCard[] | null;
  recentActivity: ActivityItem[] | null;
  quickActions: QuickAction[];
}

export interface AppointmentCard {
  id: string;
  providerId: string;
  scheduledAt: string;
  status: string;
  reasonForVisit: string | null;
}

export interface ActivityItem {
  type: string;
  description: string;
  timestamp: string;
  id: string;
}

export interface QuickAction {
  key: string;
  label: string;
  available: boolean;
}

@Injectable({ providedIn: 'root' })
export class PatientService {
  private readonly profileUrl = `${environment.apiBaseUrl}/patientprofile`;
  private readonly dashboardUrl = `${environment.apiBaseUrl}/dashboard`;

  constructor(private readonly http: HttpClient) {}

  getProfile(): Observable<PatientProfile> {
    return this.http.get<PatientProfile>(this.profileUrl);
  }

  updateProfile(data: {
    firstName: string;
    lastName: string;
    phone?: string;
    dateOfBirth?: string;
  }): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(this.profileUrl, data);
  }

  uploadPhoto(file: File): Observable<{ photoUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ photoUrl: string }>(`${this.profileUrl}/photo`, formData);
  }

  getDashboard(patientId: string): Observable<DashboardData> {
    return this.http.get<DashboardData>(`${this.dashboardUrl}/${patientId}`);
  }
}
