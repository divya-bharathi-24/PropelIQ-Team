import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTabsModule } from '@angular/material/tabs';
import { PatientService, PatientProfile } from '../../services/patient.service';
import { CalendarSyncComponent } from '../../components/calendar-sync/calendar-sync.component';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatCardModule, MatFormFieldModule,
    MatInputModule, MatButtonModule, MatIconModule, MatExpansionModule,
    MatProgressSpinnerModule, MatSnackBarModule, MatTabsModule, CalendarSyncComponent,
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})
export class ProfileComponent implements OnInit {
  profileForm!: FormGroup;
  profile: PatientProfile | null = null;
  loading = true;
  saving = false;
  editing = false;
  photoPreview: string | null = null;
  uploadingPhoto = false;

  private readonly maxPhotoSizeBytes = 2 * 1024 * 1024;

  constructor(
    private readonly fb: FormBuilder,
    private readonly patientService: PatientService,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.profileForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      phone: ['', [Validators.pattern(/^\+?[1-9]\d{1,14}$/)]],
      dateOfBirth: [''],
    });
    this.profileForm.disable();
    this.loadProfile();
  }

  loadProfile(): void {
    this.loading = true;
    this.patientService.getProfile().pipe(finalize(() => (this.loading = false))).subscribe({
      next: (p) => {
        this.profile = p;
        this.photoPreview = p.profilePhotoPath;
        this.profileForm.patchValue({
          firstName: p.firstName,
          lastName: p.lastName,
          phone: p.phone ?? '',
          dateOfBirth: p.dateOfBirth ?? '',
        });
      },
      error: () => this.snackBar.open('Failed to load profile', 'Close', { duration: 3000 }),
    });
  }

  toggleEdit(): void {
    this.editing = !this.editing;
    if (this.editing) {
      this.profileForm.enable();
    } else {
      this.profileForm.disable();
      if (this.profile) {
        this.profileForm.patchValue({
          firstName: this.profile.firstName,
          lastName: this.profile.lastName,
          phone: this.profile.phone ?? '',
          dateOfBirth: this.profile.dateOfBirth ?? '',
        });
      }
    }
  }

  save(): void {
    if (this.profileForm.invalid) return;
    this.saving = true;
    this.patientService.updateProfile(this.profileForm.value)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: () => {
          this.editing = false;
          this.profileForm.disable();
          this.snackBar.open('Profile updated', 'Close', { duration: 3000 });
          this.loadProfile();
        },
        error: () => this.snackBar.open('Failed to update profile', 'Close', { duration: 3000 }),
      });
  }

  onPhotoSelected(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    if (file.size > this.maxPhotoSizeBytes) {
      this.snackBar.open('Photo must be under 2 MB', 'Close', { duration: 3000 });
      return;
    }
    if (!['image/jpeg', 'image/png'].includes(file.type)) {
      this.snackBar.open('Only JPEG and PNG allowed', 'Close', { duration: 3000 });
      return;
    }

    const reader = new FileReader();
    reader.onload = () => (this.photoPreview = reader.result as string);
    reader.readAsDataURL(file);

    this.uploadingPhoto = true;
    this.patientService.uploadPhoto(file)
      .pipe(finalize(() => (this.uploadingPhoto = false)))
      .subscribe({
        next: (res) => {
          this.snackBar.open('Photo uploaded', 'Close', { duration: 3000 });
          if (this.profile) this.profile.profilePhotoPath = res.photoUrl;
        },
        error: () => this.snackBar.open('Photo upload failed', 'Close', { duration: 3000 }),
      });
  }
}
