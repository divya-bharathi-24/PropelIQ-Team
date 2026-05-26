import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { TimeSlot, SlotGroup } from '../../services/booking.service';

@Component({
  selector: 'app-slot-grid',
  standalone: true,
  imports: [CommonModule, MatChipsModule, MatIconModule],
  template: `
    <div class="slot-grid">
      @for (group of slotGroups; track group.date) {
        <div class="date-group">
          <h3 class="date-heading">{{ group.date | date:'EEEE, MMM d' }}</h3>
          <div class="chips-row">
            @for (slot of group.slots; track slot.id) {
              <mat-chip-option
                [class.available]="slot.isAvailable"
                [class.booked]="!slot.isAvailable"
                [disabled]="!slot.isAvailable"
                [selected]="selectedSlotId === slot.id"
                (selectionChange)="slot.isAvailable && slotSelected.emit(slot)">
                {{ slot.startTime | date:'h:mm a' }}
              </mat-chip-option>
            }
          </div>
        </div>
      } @empty {
        <div class="empty-state">
          <mat-icon>event_busy</mat-icon>
          <p>No available slots for the selected criteria.</p>
          <p class="suggestion">Try adjusting your date range or provider filter.</p>
        </div>
      }
    </div>
  `,
  styles: [`
    .date-group { margin-bottom: 16px; }
    .date-heading { font-size: 14px; font-weight: 500; color: #555; margin-bottom: 8px; }
    .chips-row { display: flex; flex-wrap: wrap; gap: 8px; }
    .available { background-color: #c8e6c9 !important; }
    .booked { background-color: #eee !important; color: #999 !important; }
    .empty-state { text-align: center; padding: 40px; color: #666; }
    .empty-state mat-icon { font-size: 48px; width: 48px; height: 48px; }
    .suggestion { font-size: 13px; color: #999; }
  `],
})
export class SlotGridComponent {
  @Input() slotGroups: SlotGroup[] = [];
  @Input() selectedSlotId: string | null = null;
  @Output() slotSelected = new EventEmitter<TimeSlot>();
}
