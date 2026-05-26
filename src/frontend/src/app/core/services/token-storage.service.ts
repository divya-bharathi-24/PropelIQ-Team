import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  private readonly ACCESS_TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';

  private get storage(): Storage {
    return localStorage.getItem('remember_me') === 'true'
      ? localStorage
      : sessionStorage;
  }

  saveTokens(accessToken: string, refreshToken: string, rememberMe: boolean): void {
    localStorage.setItem('remember_me', String(rememberMe));
    this.storage.setItem(this.ACCESS_TOKEN_KEY, accessToken);
    this.storage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY)
      ?? sessionStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY)
      ?? sessionStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  clearTokens(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    sessionStorage.removeItem(this.ACCESS_TOKEN_KEY);
    sessionStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem('remember_me');
  }

  isAuthenticated(): boolean {
    return this.getAccessToken() !== null;
  }
}
