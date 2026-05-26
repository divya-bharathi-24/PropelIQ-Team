import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  roles: string[];
  forcePasswordChange?: boolean;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly baseUrl = `${environment.apiBaseUrl}/auth`;
  private readonly registrationUrl = `${environment.apiBaseUrl}/registration`;

  constructor(private readonly http: HttpClient) {}

  register(data: {
    firstName: string;
    lastName: string;
    email: string;
    phone: string;
    dateOfBirth: string;
    password: string;
  }): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.registrationUrl}/register`, data
    );
  }

  activate(token: string): Observable<{ message: string }> {
    return this.http.get<{ message: string }>(
      `${this.registrationUrl}/activate`, { params: { token } }
    );
  }

  resendVerification(email: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.registrationUrl}/resend-verification`, { email }
    );
  }

  login(email: string, password: string): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(
      `${this.baseUrl}/login`, { email, password }
    );
  }

  refresh(accessToken: string, refreshToken: string): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(
      `${this.baseUrl}/refresh`, { accessToken, refreshToken }
    );
  }

  logout(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/logout`, {});
  }

  changePassword(currentPassword: string, newPassword: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.baseUrl}/change-password`, { currentPassword, newPassword }
    );
  }
}
