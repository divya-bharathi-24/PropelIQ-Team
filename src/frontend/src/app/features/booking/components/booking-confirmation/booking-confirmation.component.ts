import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatBottomSheetRef, MAT_BOTTOM_SHEET_DATA } from '@angular/material/bottom-sheet';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TimeSlot } from '../../services/booking.service';

export interface ConfirmationData {
  slot: TimeSlot;
}

@Component({
  selector: 'app-booking-confirmation',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  template: `
    <div class="confirmation-sheet">
      <h3>Confirm Booking</h3>
      <div class="slot-details">
        <p><mat-icon>schedule</mat-icon> {{ data.slot.startTime | date:'EEEE, MMM d, y' }}</p>
        <p><mat-icon>access_time</mat-icon> {{ data.slot.startTime | date:'h:mm a' }} - {{ data.slot.endTime | date:'h:mm a' }}</p>
      </div>
      <div class="actions">
        <button mat-button (click)="bottomSheetRef.dismiss()">Cancel</button>
        <button mat-raised-button color="primary" (click)="confirm()" [disabled]="confirming">
          @if (confirming) {
            <mat-spinner diameter="20"></mat-spinner>
          } @else {
            Confirm Booking
          }
        </button>
      </div>
      @if (verifying) {
        <p class="verifying">Verifying availability...</p>
      }
    </div>
  `,
  styles: [`
    .confirmation-sheet { padding: 24px; }
    h3 { margin: 0 0 16px; }
    .slot-details p { display: flex; align-items: center; gap: 8px; margin: 8px 0; }
    .actions { display: flex; justify-content: flex-end; gap: 8px; margin-top: 24px; }
    .verifying { text-align: center; color: #666; font-size: 13px; margin-top: 8px; }
  `],
})
export class BookingConfirmationComponent {
  confirming = false;
  verifying = false;

  constructor(
    public readonly bottomSheetRef: MatBottomSheetRef<BookingConfirmationComponent>,
    @Inject(MAT_BOTTOM_SHEET_DATA) public readonly data: ConfirmationData,
  ) {}

  confirm(): void {
    this.confirming = true;
    this.verifying = true;
    this.bottomSheetRef.dismiss({ confirmed: true, slot: this.data.slot });
  }
}
