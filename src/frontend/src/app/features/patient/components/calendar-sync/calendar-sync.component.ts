import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Clipboard, ClipboardModule } from '@angular/cdk/clipboard';
import { CalendarService, CalendarConnectionStatus } from '../../services/calendar.service';
import { CalendarCardComponent } from '../calendar-card/calendar-card.component';

@Component({
  selector: 'app-calendar-sync',
  standalone: true,
  imports: [
    CommonModule, MatButtonModule, MatIconModule, MatSnackBarModule,
    MatProgressSpinnerModule, ClipboardModule, CalendarCardComponent,
  ],
  templateUrl: './calendar-sync.component.html',
  styleUrl: './calendar-sync.component.scss',
})
export class CalendarSyncComponent implements OnInit {
  connections: CalendarConnectionStatus[] = [];
  loading = true;
  icsFeedUrl: string | null = null;

  get showGoogleConnect(): boolean {
    return !this.connections.length || !this.connections.some(c => c.provider === 'google' && c.status === 'active');
  }

  get showIcsConnect(): boolean {
    return !this.connections.length || !this.connections.some(c => c.provider === 'ics' && c.status === 'active');
  }

  constructor(
    private readonly calendarService: CalendarService,
    private readonly snackBar: MatSnackBar,
    private readonly clipboard: Clipboard,
  ) {}

  ngOnInit(): void {
    this.loadStatus();
  }

  loadStatus(): void {
    this.loading = true;
    this.calendarService.getSyncStatus().subscribe({
      next: (connections) => {
        this.connections = connections;
        const icsConn = connections.find(c => c.provider === 'ics');
        this.icsFeedUrl = icsConn?.icsFeedUrl ?? null;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  enableIcsFeed(): void {
    this.calendarService.enableIcsFeed().subscribe({
      next: (res) => {
        this.icsFeedUrl = res.icsFeedUrl;
        this.snackBar.open('ICS feed enabled', 'Close', { duration: 3000 });
        this.loadStatus();
      },
      error: () => this.snackBar.open('Failed to enable ICS feed', 'Close', { duration: 3000 }),
    });
  }

  connectGoogle(): void {
    this.calendarService.getGoogleAuthUrl().subscribe({
      next: (res) => {
        window.location.href = res.authUrl;
      },
      error: () => this.snackBar.open('Failed to initiate Google Calendar connection', 'Close', { duration: 3000 }),
    });
  }

  onDisconnect(provider: string): void {
    if (!confirm('Disconnect calendar? Existing entries will remain but no longer update.')) return;

    this.calendarService.disconnect(provider).subscribe({
      next: (res) => {
        this.snackBar.open(res.message, 'Close', { duration: 5000 });
        this.loadStatus();
        if (provider === 'ics') this.icsFeedUrl = null;
      },
      error: () => this.snackBar.open('Disconnect failed', 'Close', { duration: 3000 }),
    });
  }

  onReconnect(provider: string): void {
    if (provider === 'google') {
      this.connectGoogle();
    } else {
      this.enableIcsFeed();
    }
  }

  copyFeedUrl(): void {
    if (this.icsFeedUrl) {
      this.clipboard.copy(this.icsFeedUrl);
      this.snackBar.open('Feed URL copied', 'Close', { duration: 2000 });
    }
  }
}
