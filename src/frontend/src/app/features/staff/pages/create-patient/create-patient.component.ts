import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { Clipboard, ClipboardModule } from '@angular/cdk/clipboard';
import { StaffAccountService } from '../../services/staff-account.service';

@Component({
  selector: 'app-create-patient',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule,
    MatDatepickerModule, MatNativeDateModule,
    MatProgressSpinnerModule, MatIconModule,
    MatSnackBarModule, ClipboardModule,
  ],
  template: `
    <div class="create-patient-page">
      <h1>Create Patient Account</h1>

      @if (!tempPassword) {
        <form [formGroup]="form" (ngSubmit)="onSubmit()">
          <mat-form-field appearance="outline">
            <mat-label>First Name</mat-label>
            <input matInput formControlName="firstName">
            @if (form.get('firstName')?.hasError('required') && form.get('firstName')?.touched) {
              <mat-error>Required</mat-error>
            }
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Last Name</mat-label>
            <input matInput formControlName="lastName">
            @if (form.get('lastName')?.hasError('required') && form.get('lastName')?.touched) {
              <mat-error>Required</mat-error>
            }
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Phone</mat-label>
            <input matInput formControlName="phone" type="tel">
            @if (form.get('phone')?.hasError('required') && form.get('phone')?.touched) {
              <mat-error>Required</mat-error>
            }
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Date of Birth</mat-label>
            <input matInput [matDatepicker]="picker" formControlName="dateOfBirth">
            <mat-datepicker-toggle matIconSuffix [for]="picker"></mat-datepicker-toggle>
            <mat-datepicker #picker></mat-datepicker>
            @if (form.get('dateOfBirth')?.hasError('required') && form.get('dateOfBirth')?.touched) {
              <mat-error>Required</mat-error>
            }
          </mat-form-field>

          @if (errorMessage) {
            <div class="error-banner">{{ errorMessage }}</div>
          }

          <button mat-raised-button color="primary" type="submit" [disabled]="loading">
            @if (loading) { <mat-spinner diameter="20"></mat-spinner> }
            @else { Create Account }
          </button>
        </form>
      } @else {
        <div class="success-panel">
          <mat-icon class="success-icon">check_circle</mat-icon>
          <h2>Patient Account Created</h2>
          <p>Temporary password (shown only once):</p>
          <div class="password-display">
            <code>{{ tempPassword }}</code>
            <button mat-icon-button (click)="copyPassword()"
                    aria-label="Copy password">
              <mat-icon>content_copy</mat-icon>
            </button>
          </div>
          <p class="hint">The patient must change this password on first login.</p>
          <button mat-stroked-button (click)="reset()">Create Another</button>
        </div>
      }
    </div>
  `,
  styles: [`
    .create-patient-page { max-width: 480px; margin: 32px auto; padding: 24px; }
    form { display: flex; flex-direction: column; gap: 4px; }
    mat-form-field { width: 100%; }
    .error-banner { background: #fdecea; color: #611a15; padding: 12px; border-radius: 4px; margin-bottom: 8px; }
    .success-panel { text-align: center; }
    .success-icon { font-size: 64px; width: 64px; height: 64px; color: #388e3c; }
    .password-display { display: flex; align-items: center; justify-content: center; gap: 8px; background: #f5f5f5; padding: 12px; border-radius: 4px; margin: 16px 0; }
    .password-display code { font-size: 18px; font-weight: 600; letter-spacing: 1px; }
    .hint { color: #666; font-size: 14px; }
  `],
})
export class CreatePatientComponent {
  form: FormGroup;
  loading = false;
  errorMessage = '';
  tempPassword = '';

  constructor(
    private readonly fb: FormBuilder,
    private readonly staffService: StaffAccountService,
    private readonly clipboard: Clipboard,
    private readonly snackBar: MatSnackBar,
  ) {
    this.form = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      phone: ['', [Validators.required, Validators.pattern(/^\+?[\d\s\-()]{7,20}$/)]],
      dateOfBirth: ['', Validators.required],
    });
  }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    this.errorMessage = '';

    const dob = this.form.value.dateOfBirth;
    const dateStr = dob instanceof Date ? dob.toISOString().split('T')[0] : dob;

    this.staffService.createPatient({ ...this.form.value, dateOfBirth: dateStr }).subscribe({
      next: (res) => {
        this.loading = false;
        this.tempPassword = res.temporaryPassword;
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message ?? 'Failed to create patient account.';
      },
    });
  }

  copyPassword(): void {
    this.clipboard.copy(this.tempPassword);
    this.snackBar.open('Password copied!', 'OK', { duration: 2000 });
  }

  reset(): void {
    this.tempPassword = '';
    this.form.reset();
  }
}
