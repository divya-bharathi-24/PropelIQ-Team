import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SlotGridComponent } from '../../../booking/components/slot-grid/slot-grid.component';
import { SlotFilterComponent, SlotFilterValues } from '../../../booking/components/slot-filter/slot-filter.component';
import { BookingService, SlotGroup, TimeSlot } from '../../../booking/services/booking.service';

export interface RescheduleDialogData {
  appointmentId: string;
  currentScheduledAt: string;
  isLate: boolean;
}

@Component({
  selector: 'app-reschedule-dialog',
  standalone: true,
  imports: [
    CommonModule, MatDialogModule, MatButtonModule, MatIconModule,
    MatProgressSpinnerModule, SlotGridComponent, SlotFilterComponent,
  ],
  template: `
    <h2 mat-dialog-title>Reschedule Appointment</h2>
    <mat-dialog-content>
      <p>Current: {{ data.currentScheduledAt | date:'MMM d, y h:mm a' }}</p>
      @if (data.isLate) {
        <div class="warning">
          <mat-icon>warning</mat-icon>
          <span>Late reschedule may affect your reliability score.</span>
        </div>
      }
      <app-slot-filter (filterChanged)="onFilterChanged($event)"></app-slot-filter>
      @if (loading) {
        <div class="loading"><mat-spinner diameter="30"></mat-spinner></div>
      } @else {
        <app-slot-grid
          [slotGroups]="slotGroups"
          [selectedSlotId]="selectedSlot?.id ?? null"
          (slotSelected)="onSlotSelected($event)">
        </app-slot-grid>
      }
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-raised-button color="primary"
        [disabled]="!selectedSlot"
        [mat-dialog-close]="selectedSlot?.id">
        Confirm Reschedule
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .warning { display: flex; align-items: center; gap: 8px; padding: 12px;
               background: #fff3e0; border-radius: 8px; margin: 12px 0; color: #e65100; }
    .loading { display: flex; justify-content: center; padding: 24px; }
    mat-dialog-content { min-width: 400px; max-height: 60vh; }
  `],
})
export class RescheduleDialogComponent {
  slotGroups: SlotGroup[] = [];
  selectedSlot: TimeSlot | null = null;
  loading = true;

  constructor(
    public readonly dialogRef: MatDialogRef<RescheduleDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public readonly data: RescheduleDialogData,
    private readonly bookingService: BookingService,
  ) {
    this.loadSlots({});
  }

  onFilterChanged(values: SlotFilterValues): void {
    this.loadSlots(values);
  }

  onSlotSelected(slot: TimeSlot): void {
    this.selectedSlot = slot;
  }

  private loadSlots(params: SlotFilterValues): void {
    this.loading = true;
    this.bookingService.getAvailableSlots(params).subscribe({
      next: groups => { this.slotGroups = groups; this.loading = false; },
      error: () => { this.loading = false; },
    });
  }
}
