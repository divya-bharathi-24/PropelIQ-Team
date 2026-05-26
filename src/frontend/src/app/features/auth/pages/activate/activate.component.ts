import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-activate',
  standalone: true,
  imports: [CommonModule, RouterLink, MatProgressSpinnerModule, MatIconModule],
  template: `
    <div class="activate-page">
      <div class="activate-card">
        @if (loading) {
          <mat-spinner diameter="48"></mat-spinner>
          <p>Verifying your account...</p>
        } @else if (success) {
          <mat-icon class="success-icon">check_circle</mat-icon>
          <h1>Account Activated!</h1>
          <p>Your email has been verified. You can now sign in.</p>
          <a routerLink="/auth/login" class="login-link">Go to Login</a>
        } @else {
          <mat-icon class="error-icon">error_outline</mat-icon>
          <h1>Verification Failed</h1>
          <p>{{ errorMessage }}</p>
          <a routerLink="/auth/login" class="login-link">Go to Login</a>
        }
      </div>
    </div>
  `,
  styles: [`
    .activate-page { display: flex; justify-content: center; align-items: center; min-height: 100vh; padding: 24px; }
    .activate-card { max-width: 480px; width: 100%; text-align: center; padding: 32px; }
    .success-icon { font-size: 64px; width: 64px; height: 64px; color: #388e3c; }
    .error-icon { font-size: 64px; width: 64px; height: 64px; color: #d32f2f; }
    .login-link { color: #1565c0; display: inline-block; margin-top: 24px; font-weight: 500; }
  `],
})
export class ActivateComponent implements OnInit {
  loading = true;
  success = false;
  errorMessage = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly authService: AuthService,
  ) {}

  ngOnInit(): void {
    const token = this.route.snapshot.queryParams['token'];
    if (!token) {
      this.loading = false;
      this.errorMessage = 'Invalid verification link.';
      return;
    }

    this.authService.activate(token).subscribe({
      next: () => {
        this.loading = false;
        this.success = true;
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message ?? 'Verification failed. Please try again.';
      },
    });
  }
}
