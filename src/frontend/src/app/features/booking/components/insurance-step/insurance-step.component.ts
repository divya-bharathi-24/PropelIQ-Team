import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { InsuranceService, InsuranceCheckResponse } from '../../services/insurance.service';
import { CoverageDisplayComponent } from '../coverage-display/coverage-display.component';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-insurance-step',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatProgressSpinnerModule, CoverageDisplayComponent,
  ],
  templateUrl: './insurance-step.component.html',
  styleUrl: './insurance-step.component.scss',
})
export class InsuranceStepComponent {
  @Output() coverageChecked = new EventEmitter<InsuranceCheckResponse | null>();

  insuranceForm: FormGroup;
  loading = false;
  coverageResult: InsuranceCheckResponse | null = null;
  showSecondary = false;

  constructor(
    private readonly fb: FormBuilder,
    private readonly insuranceService: InsuranceService,
  ) {
    this.insuranceForm = this.fb.group({
      insuranceProvider: ['', [Validators.required, Validators.maxLength(200)]],
      policyNumber: ['', [Validators.required, Validators.maxLength(50)]],
      groupNumber: ['', [Validators.maxLength(50)]],
      memberId: ['', [Validators.required, Validators.maxLength(50)]],
    });
  }

  checkEligibility(): void {
    if (this.insuranceForm.invalid) return;

    this.loading = true;
    this.coverageResult = null;

    this.insuranceService.checkEligibility({
      ...this.insuranceForm.value,
      patientId: 'me',
    }).pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (result) => {
          this.coverageResult = result;
          this.coverageChecked.emit(result);
        },
        error: () => {
          this.coverageResult = {
            coverageStatus: 'Verification Pending',
            copayEstimate: null,
            limitations: null,
            verifiedAt: new Date().toISOString(),
          };
          this.coverageChecked.emit(this.coverageResult);
        },
      });
  }

  toggleSecondary(): void {
    this.showSecondary = !this.showSecondary;
  }
}
