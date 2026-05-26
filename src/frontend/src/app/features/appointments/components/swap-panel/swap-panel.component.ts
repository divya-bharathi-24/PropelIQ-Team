import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SwapService } from '../../services/swap.service';
import { SwapCountdownComponent } from '../swap-countdown/swap-countdown.component';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-swap-panel',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatSidenavModule, MatFormFieldModule,
    MatInputModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule,
    MatSnackBarModule, SwapCountdownComponent,
  ],
  templateUrl: './swap-panel.component.html',
  styleUrl: './swap-panel.component.scss',
})
export class SwapPanelComponent {
  @Input() opened = false;
  @Input() originalSlotId: string | null = null;
  @Input() providerId: string | null = null;
  @Output() closed = new EventEmitter<void>();

  swapForm: FormGroup;
  submitting = false;
  swapResult: { swapRequestId: string; queuePosition: number | null } | null = null;
  matchedExpiresAt: string | null = null;
  accepting = false;

  constructor(
    private readonly fb: FormBuilder,
    private readonly swapService: SwapService,
    private readonly snackBar: MatSnackBar,
  ) {
    this.swapForm = this.fb.group({
      preferredDate: ['', Validators.required],
      preferredTimeStart: [''],
      preferredTimeEnd: [''],
    });
  }

  submitRequest(): void {
    if (this.swapForm.invalid || !this.originalSlotId || !this.providerId) return;

    this.submitting = true;
    this.swapService.createSwapRequest({
      originalSlotId: this.originalSlotId,
      providerId: this.providerId,
      ...this.swapForm.value,
    }).pipe(finalize(() => (this.submitting = false)))
      .subscribe({
        next: (result) => {
          this.swapResult = result;
          this.snackBar.open(`Swap requested. Queue position: ${result.queuePosition}`, 'Close', { duration: 3000 });
        },
        error: (err) => {
          this.snackBar.open(err.error?.message ?? 'Swap request failed', 'Close', { duration: 3000 });
        },
      });
  }

  acceptSwap(): void {
    if (!this.swapResult) return;
    this.accepting = true;
    this.swapService.acceptSwap(this.swapResult.swapRequestId)
      .pipe(finalize(() => (this.accepting = false)))
      .subscribe({
        next: () => {
          this.snackBar.open('Swap completed!', 'Close', { duration: 3000 });
          this.close();
        },
        error: (err) => {
          this.snackBar.open(err.error?.message ?? 'Swap acceptance failed', 'Close', { duration: 3000 });
        },
      });
  }

  declineSwap(): void {
    if (!this.swapResult) return;
    this.swapService.cancelSwap(this.swapResult.swapRequestId).subscribe({
      next: () => {
        this.snackBar.open('Swap declined', 'Close', { duration: 3000 });
        this.swapResult = null;
      },
    });
  }

  onCountdownExpired(): void {
    this.snackBar.open('Swap acceptance window expired', 'Close', { duration: 3000 });
    this.swapResult = null;
    this.matchedExpiresAt = null;
  }

  close(): void {
    this.opened = false;
    this.swapResult = null;
    this.matchedExpiresAt = null;
    this.closed.emit();
  }
}
