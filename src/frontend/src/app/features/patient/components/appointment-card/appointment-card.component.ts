import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { AppointmentCard } from '../../services/patient.service';

@Component({
  selector: 'app-appointment-card',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatChipsModule, MatIconModule],
  template: `
    <mat-card class="appointment-card">
      <mat-card-header>
        <mat-icon mat-card-avatar>event</mat-icon>
        <mat-card-title>{{ appointment.scheduledAt | date:'mediumDate' }}</mat-card-title>
        <mat-card-subtitle>{{ appointment.scheduledAt | date:'shortTime' }}</mat-card-subtitle>
      </mat-card-header>
      <mat-card-content>
        @if (appointment.reasonForVisit) {
          <p>{{ appointment.reasonForVisit }}</p>
        }
        <span class="status-badge" [class]="appointment.status.toLowerCase()">
          {{ appointment.status }}
        </span>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .appointment-card { margin-bottom: 12px; }
    .status-badge { padding: 4px 8px; border-radius: 12px; font-size: 12px; font-weight: 500; }
    .status-badge.scheduled { background: #e3f2fd; color: #1565c0; }
    .status-badge.completed { background: #e8f5e9; color: #2e7d32; }
    .status-badge.cancelled { background: #fce4ec; color: #c62828; }
  `],
})
export class AppointmentCardComponent {
  @Input({ required: true }) appointment!: AppointmentCard;
}
