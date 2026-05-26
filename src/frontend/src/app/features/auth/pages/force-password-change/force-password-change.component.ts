import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-force-password-change',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule, MatProgressSpinnerModule,
  ],
  template: `
    <div class="change-password-page">
      <div class="change-card">
        <h1>Change Your Password</h1>
        <p>You must change your temporary password before continuing.</p>

        @if (errorMessage) {
          <div class="error-banner">{{ errorMessage }}</div>
        }

        <form [formGroup]="form" (ngSubmit)="onSubmit()">
          <mat-form-field appearance="outline">
            <mat-label>Current Password</mat-label>
            <input matInput formControlName="currentPassword" type="password">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>New Password</mat-label>
            <input matInput formControlName="newPassword" type="password">
            @if (form.get('newPassword')?.touched && form.get('newPassword')?.invalid) {
              <mat-error>Min 8 chars with uppercase, number, and special character</mat-error>
            }
          </mat-form-field>

          <button mat-raised-button color="primary" type="submit" [disabled]="loading">
            @if (loading) { <mat-spinner diameter="20"></mat-spinner> }
            @else { Change Password }
          </button>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .change-password-page { display: flex; justify-content: center; align-items: center; min-height: 100vh; padding: 24px; }
    .change-card { max-width: 420px; width: 100%; padding: 32px; }
    .change-card form { display: flex; flex-direction: column; gap: 4px; }
    .change-card mat-form-field { width: 100%; }
    .error-banner { background: #fdecea; color: #611a15; padding: 12px; border-radius: 4px; margin-bottom: 16px; }
    button { height: 48px; }
  `],
})
export class ForcePasswordChangeComponent {
  form: FormGroup;
  loading = false;
  errorMessage = '';

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router,
  ) {
    this.form = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{}|;:',.<>?/`~])/)
      ]],
    });
  }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    this.authService.changePassword(
      this.form.value.currentPassword,
      this.form.value.newPassword
    ).subscribe({
      next: () => { this.router.navigate(['/auth/login']); },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message ?? 'Failed to change password.';
      },
    });
  }
}
