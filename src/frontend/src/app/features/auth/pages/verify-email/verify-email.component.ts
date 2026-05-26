import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule, RouterLink, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  template: `
    <div class="verify-email-page">
      <div class="verify-card">
        <mat-icon class="email-icon">mail_outline</mat-icon>
        <h1>Check Your Email</h1>
        <p>We've sent a verification link to <strong>{{ email }}</strong></p>
        <p class="hint">Click the link in the email to activate your account. The link expires in 24 hours.</p>

        @if (message) {
          <div class="message" [class.error]="isError">{{ message }}</div>
        }

        <button mat-stroked-button (click)="resend()" [disabled]="cooldown > 0 || resending">
          @if (resending) {
            <mat-spinner diameter="18"></mat-spinner>
          } @else if (cooldown > 0) {
            Resend in {{ cooldown }}s
          } @else {
            Resend Verification Email
          }
        </button>

        <p class="back-link"><a routerLink="/auth/login">Back to Login</a></p>
      </div>
    </div>
  `,
  styles: [`
    .verify-email-page { display: flex; justify-content: center; align-items: center; min-height: 100vh; padding: 24px; }
    .verify-card { max-width: 480px; width: 100%; text-align: center; padding: 32px; }
    .email-icon { font-size: 64px; width: 64px; height: 64px; color: #1565c0; margin-bottom: 16px; }
    .hint { color: #666; font-size: 14px; }
    .message { padding: 12px; border-radius: 4px; margin: 16px 0; background: #e8f5e9; color: #1b5e20; }
    .message.error { background: #fdecea; color: #611a15; }
    .back-link { margin-top: 24px; }
    .back-link a { color: #1565c0; }
  `],
})
export class VerifyEmailComponent implements OnInit {
  email = '';
  cooldown = 0;
  resending = false;
  message = '';
  isError = false;
  private intervalId?: ReturnType<typeof setInterval>;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.email = this.route.snapshot.queryParams['email'] ?? '';
  }

  resend(): void {
    if (!this.email || this.cooldown > 0) return;

    this.resending = true;
    this.authService.resendVerification(this.email).subscribe({
      next: (res) => {
        this.resending = false;
        this.message = res.message;
        this.isError = false;
        this.startCooldown();
      },
      error: () => {
        this.resending = false;
        this.message = 'Please wait before requesting another email.';
        this.isError = true;
        this.startCooldown();
      },
    });
  }

  private startCooldown(): void {
    this.cooldown = 60;
    this.intervalId = setInterval(() => {
      this.cooldown--;
      if (this.cooldown <= 0) clearInterval(this.intervalId);
    }, 1000);
  }
}
