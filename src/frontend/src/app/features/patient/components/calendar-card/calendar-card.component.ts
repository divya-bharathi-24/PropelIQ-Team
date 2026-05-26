import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CalendarConnectionStatus } from '../../services/calendar.service';

@Component({
  selector: 'app-calendar-card',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule],
  template: `
    <mat-card class="calendar-card">
      <mat-card-header>
        <mat-icon mat-card-avatar [class]="'status-icon status-' + connection.status">
          {{ connection.provider === 'google' ? 'event' : 'rss_feed' }}
        </mat-icon>
        <mat-card-title>{{ getProviderLabel() }}</mat-card-title>
        <mat-card-subtitle>
          <span class="status-indicator" [class]="'status-' + connection.status">
            {{ getStatusLabel() }}
          </span>
        </mat-card-subtitle>
      </mat-card-header>
      <mat-card-actions align="end">
        @if (connection.status === 'active') {
          <button mat-stroked-button color="warn" (click)="disconnectClicked.emit(connection.provider)">
            Disconnect
          </button>
        } @else if (connection.status === 'disconnected') {
          <button mat-raised-button color="primary" (click)="reconnectClicked.emit(connection.provider)">
            Reconnect
          </button>
        }
      </mat-card-actions>
    </mat-card>
  `,
  styles: [`
    .calendar-card { margin-bottom: 12px; }
    .status-icon { font-size: 24px; }
    .status-indicator { padding: 2px 8px; border-radius: 10px; font-size: 12px; }
    .status-active { color: #2e7d32; background: #c8e6c9; }
    .status-delayed { color: #f57f17; background: #fff9c4; }
    .status-disconnected { color: #c62828; background: #ffcdd2; }
  `],
})
export class CalendarCardComponent {
  @Input() connection!: CalendarConnectionStatus;
  @Output() disconnectClicked = new EventEmitter<string>();
  @Output() reconnectClicked = new EventEmitter<string>();

  getProviderLabel(): string {
    return this.connection.provider === 'google' ? 'Google Calendar' : 'ICS Calendar Feed';
  }

  getStatusLabel(): string {
    switch (this.connection.status) {
      case 'active': return 'Syncing';
      case 'delayed': return 'Sync Delayed';
      case 'disconnected': return 'Disconnected';
      default: return this.connection.status;
    }
  }
}
