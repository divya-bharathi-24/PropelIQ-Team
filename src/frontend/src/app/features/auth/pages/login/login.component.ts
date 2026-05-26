import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';
import { TokenStorageService } from '@app/core/services/token-storage.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterLink,
    MatFormFieldModule, MatInputModule, MatButtonModule,
    MatCheckboxModule, MatProgressSpinnerModule, MatIconModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  form: FormGroup;
  loading = false;
  errorMessage = '';
  hidePassword = true;
  lockoutSeconds = 0;
  private lockoutInterval?: ReturnType<typeof setInterval>;

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly tokenStorage: TokenStorageService,
    private readonly router: Router,
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
      rememberMe: [false],
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const { email, password, rememberMe } = this.form.value;

    this.authService.login(email, password).subscribe({
      next: (response) => {
        this.loading = false;
        this.tokenStorage.saveTokens(
          response.accessToken, response.refreshToken, rememberMe
        );

        if (response.forcePasswordChange) {
          this.router.navigate(['/auth/force-password-change']);
          return;
        }

        const role = response.roles?.[0]?.toLowerCase() ?? 'patient';
        this.router.navigate([`/${role}`]);
      },
      error: (err: HttpErrorResponse) => {
        this.loading = false;
        if (err.status === 429) {
          const retryAfter = parseInt(err.headers.get('Retry-After') ?? '1800', 10);
          this.startLockoutTimer(retryAfter);
          this.errorMessage = 'Account temporarily locked. Too many failed attempts.';
        } else {
          this.errorMessage = err.error?.message ?? 'Invalid email or password.';
        }
      },
    });
  }

  private startLockoutTimer(seconds: number): void {
    this.lockoutSeconds = seconds;
    clearInterval(this.lockoutInterval);
    this.lockoutInterval = setInterval(() => {
      this.lockoutSeconds--;
      if (this.lockoutSeconds <= 0) {
        clearInterval(this.lockoutInterval);
        this.errorMessage = '';
      }
    }, 1000);
  }

  get lockoutMinutes(): string {
    const m = Math.floor(this.lockoutSeconds / 60);
    const s = this.lockoutSeconds % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
  }
}
