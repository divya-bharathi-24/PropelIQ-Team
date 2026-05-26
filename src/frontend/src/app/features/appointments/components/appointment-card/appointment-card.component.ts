import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatBadgeModule } from '@angular/material/badge';
import { MatMenuModule } from '@angular/material/menu';
import { AppointmentListItem } from '../../services/appointment-mgmt.service';

@Component({
  selector: 'app-appt-card',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatBadgeModule, MatMenuModule],
  template: `
    <mat-card class="appt-card">
      <mat-card-header>
        <mat-card-title>
          {{ appointment.scheduledAt | date:'MMM d, y h:mm a' }}
          @if (queuePosition) {
            <span class="queue-badge" [matBadge]="queuePosition" matBadgeColor="accent" matBadgeOverlap="false"></span>
          }
        </mat-card-title>
        <span class="status-badge" [class]="'status-' + appointment.status.toLowerCase()">
          {{ appointment.status }}
        </span>
      </mat-card-header>
      <mat-card-content>
        <p>{{ appointment.reasonForVisit ?? 'General consultation' }}</p>
      </mat-card-content>
      <mat-card-actions align="end">
        <button mat-icon-button [matMenuTriggerFor]="menu">
          <mat-icon>more_vert</mat-icon>
        </button>
        <mat-menu #menu="matMenu">
          <button mat-menu-item (click)="rescheduleClicked.emit(appointment)">
            <mat-icon>schedule</mat-icon> Reschedule
          </button>
          <button mat-menu-item (click)="cancelClicked.emit(appointment)">
            <mat-icon>cancel</mat-icon> Cancel
          </button>
          <button mat-menu-item (click)="downloadPdf.emit(appointment)">
            <mat-icon>picture_as_pdf</mat-icon> Download PDF
          </button>
        </mat-menu>
      </mat-card-actions>
    </mat-card>
  `,
  styles: [`
    .appt-card { margin-bottom: 12px; }
    .status-badge { padding: 4px 8px; border-radius: 12px; font-size: 12px; font-weight: 500; }
    .status-scheduled { background: #c8e6c9; color: #2e7d32; }
    .status-completed { background: #bbdefb; color: #1565c0; }
    .status-cancelled { background: #eee; color: #666; }
    .status-noshow { background: #ffcdd2; color: #c62828; }
    .queue-badge { margin-left: 8px; }
    mat-card-header { display: flex; justify-content: space-between; align-items: center; }
  `],
})
export class ApptCardComponent {
  @Input() appointment!: AppointmentListItem;
  @Input() queuePosition: number | null = null;
  @Output() rescheduleClicked = new EventEmitter<AppointmentListItem>();
  @Output() cancelClicked = new EventEmitter<AppointmentListItem>();
  @Output() downloadPdf = new EventEmitter<AppointmentListItem>();
}
