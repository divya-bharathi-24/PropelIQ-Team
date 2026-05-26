import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { ActivityItem } from '../../services/patient.service';

@Component({
  selector: 'app-activity-feed',
  standalone: true,
  imports: [CommonModule, MatListModule, MatIconModule],
  template: `
    <mat-list>
      @for (item of activities; track item.id) {
        <mat-list-item>
          <mat-icon matListItemIcon>{{ getIcon(item.type) }}</mat-icon>
          <span matListItemTitle>{{ item.description }}</span>
          <span matListItemLine class="timestamp">{{ getRelativeTime(item.timestamp) }}</span>
        </mat-list-item>
      } @empty {
        <mat-list-item>
          <span matListItemTitle>No recent activity</span>
        </mat-list-item>
      }
    </mat-list>
  `,
  styles: [`.timestamp { color: #666; font-size: 12px; }`],
})
export class ActivityFeedComponent {
  @Input() activities: ActivityItem[] = [];

  getIcon(type: string): string {
    const icons: Record<string, string> = {
      appointment: 'event',
      document: 'description',
      message: 'chat',
      prescription: 'medication',
    };
    return icons[type] ?? 'info';
  }

  getRelativeTime(timestamp: string): string {
    const diff = Date.now() - new Date(timestamp).getTime();
    const minutes = Math.floor(diff / 60000);
    if (minutes < 1) return 'Just now';
    if (minutes < 60) return `${minutes}m ago`;
    const hours = Math.floor(minutes / 60);
    if (hours < 24) return `${hours}h ago`;
    const days = Math.floor(hours / 24);
    return `${days}d ago`;
  }
}
