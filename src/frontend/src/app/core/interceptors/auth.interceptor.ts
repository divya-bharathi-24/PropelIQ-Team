import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { TokenStorageService } from '../services/token-storage.service';
import { catchError, switchMap, throwError } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';

let isRefreshing = false;

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const tokenStorage = inject(TokenStorageService);
  const router = inject(Router);
  const http = inject(HttpClient);

  const token = tokenStorage.getAccessToken();
  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !req.url.includes('/auth/refresh') && !isRefreshing) {
        isRefreshing = true;
        const accessToken = tokenStorage.getAccessToken();
        const refreshToken = tokenStorage.getRefreshToken();

        if (accessToken && refreshToken) {
          return http.post<{ accessToken: string; refreshToken: string }>(
            `${environment.apiBaseUrl}/auth/refresh`,
            { accessToken, refreshToken }
          ).pipe(
            switchMap(response => {
              isRefreshing = false;
              tokenStorage.saveTokens(response.accessToken, response.refreshToken, true);
              const retryReq = req.clone({
                setHeaders: { Authorization: `Bearer ${response.accessToken}` }
              });
              return next(retryReq);
            }),
            catchError(refreshError => {
              isRefreshing = false;
              tokenStorage.clearTokens();
              router.navigate(['/auth/login']);
              return throwError(() => refreshError);
            })
          );
        }

        tokenStorage.clearTokens();
        router.navigate(['/auth/login']);
      }

      return throwError(() => error);
    })
  );
};
