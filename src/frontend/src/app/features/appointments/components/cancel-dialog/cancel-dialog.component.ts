import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface CancelDialogData {
  appointmentId: string;
  scheduledAt: string;
  isLate: boolean;
}

@Component({
  selector: 'app-cancel-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule],
  template: `
    <h2 mat-dialog-title>Cancel Appointment</h2>
    <mat-dialog-content>
      <p>Appointment on {{ data.scheduledAt | date:'MMM d, y h:mm a' }}</p>
      @if (data.isLate) {
        <div class="warning">
          <mat-icon>warning</mat-icon>
          <span>Late cancellation (&lt;24h) may affect your reliability score.</span>
        </div>
      }
      <p>Are you sure you want to cancel this appointment?</p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Keep Appointment</button>
      <button mat-raised-button color="warn" [mat-dialog-close]="true">Cancel Appointment</button>
    </mat-dialog-actions>
  `,
  styles: [`
    .warning { display: flex; align-items: center; gap: 8px; padding: 12px;
               background: #fff3e0; border-radius: 8px; margin: 12px 0; color: #e65100; }
  `],
})
export class CancelDialogComponent {
  constructor(
    public readonly dialogRef: MatDialogRef<CancelDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public readonly data: CancelDialogData,
  ) {}
}
