import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatBottomSheet, MatBottomSheetModule } from '@angular/material/bottom-sheet';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Subject, takeUntil, timer } from 'rxjs';
import { BookingService, SlotGroup, TimeSlot } from '../../services/booking.service';
import { SlotWebsocketService, SlotUpdate } from '../../services/slot-websocket.service';
import { SlotGridComponent } from '../../components/slot-grid/slot-grid.component';
import { SlotFilterComponent, SlotFilterValues } from '../../components/slot-filter/slot-filter.component';
import { BookingConfirmationComponent, ConfirmationData } from '../../components/booking-confirmation/booking-confirmation.component';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [
    CommonModule, MatCardModule, MatButtonToggleModule, MatBottomSheetModule,
    MatSnackBarModule, MatProgressSpinnerModule,
    SlotGridComponent, SlotFilterComponent,
  ],
  templateUrl: './booking.component.html',
  styleUrl: './booking.component.scss',
})
export class BookingComponent implements OnInit, OnDestroy {
  slotGroups: SlotGroup[] = [];
  selectedSlot: TimeSlot | null = null;
  viewMode: 'day' | 'week' = 'week';
  loading = true;
  private filterValues: SlotFilterValues = {};
  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly bookingService: BookingService,
    private readonly wsService: SlotWebsocketService,
    private readonly bottomSheet: MatBottomSheet,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.loadSlots();

    // WebSocket for real-time updates
    this.wsService.connect().pipe(takeUntil(this.destroy$)).subscribe({
      next: (_update: SlotUpdate) => this.loadSlots(),
      error: () => {},
    });

    // 30-second polling fallback
    timer(30000, 30000).pipe(takeUntil(this.destroy$)).subscribe(() => this.loadSlots());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onFilterChanged(values: SlotFilterValues): void {
    this.filterValues = values;
    this.loadSlots();
  }

  onSlotSelected(slot: TimeSlot): void {
    this.selectedSlot = slot;
    this.openConfirmation(slot);
  }

  private loadSlots(): void {
    this.loading = true;
    this.bookingService.getAvailableSlots(this.filterValues).subscribe({
      next: (groups) => {
        this.slotGroups = groups;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.snackBar.open('Failed to load slots', 'Close', { duration: 3000 });
      },
    });
  }

  private openConfirmation(slot: TimeSlot): void {
    const ref = this.bottomSheet.open(BookingConfirmationComponent, {
      data: { slot } as ConfirmationData,
    });

    ref.afterDismissed().subscribe(result => {
      if (result?.confirmed) {
        this.bookingService.bookSlot(slot.id).subscribe({
          next: () => {
            this.snackBar.open('Appointment booked!', 'Close', { duration: 3000 });
            this.loadSlots();
          },
          error: (err) => {
            if (err.status === 409) {
              this.snackBar.open('Slot no longer available. Refreshing...', 'Close', { duration: 3000 });
              this.loadSlots();
            } else {
              this.snackBar.open('Booking failed', 'Close', { duration: 3000 });
            }
          },
        });
      }
    });
  }
}
