import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-quick-actions',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule],
  template: `
    <div class="quick-actions-grid">
      @for (action of actions; track action.label) {
        <button mat-raised-button [color]="action.color" (click)="actionClicked.emit(action.key)">
          <mat-icon>{{ action.icon }}</mat-icon>
          {{ action.label }}
        </button>
      }
    </div>
  `,
  styles: [`
    .quick-actions-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 12px;
    }
    button { display: flex; align-items: center; gap: 8px; justify-content: center; min-height: 48px; }
  `],
})
export class QuickActionsComponent {
  @Output() actionClicked = new EventEmitter<string>();

  actions = [
    { key: 'book', label: 'Book Appointment', icon: 'calendar_today', color: 'primary' as const },
    { key: 'upload', label: 'Upload Document', icon: 'upload_file', color: 'accent' as const },
    { key: 'history', label: 'Medical History', icon: 'history', color: '' as const },
    { key: 'support', label: 'Contact Support', icon: 'support_agent', color: '' as const },
  ];
}
