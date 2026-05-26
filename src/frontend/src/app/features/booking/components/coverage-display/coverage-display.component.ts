import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { InsuranceCheckResponse } from '../../services/insurance.service';

@Component({
  selector: 'app-coverage-display',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatTooltipModule],
  template: `
    <div class="coverage-result" [class]="getBadgeClass()">
      <div class="status-row">
        <span class="badge" [class]="getBadgeClass()">{{ coverage.coverageStatus }}</span>
        @if (coverage.coverageStatus === 'Unable to Verify') {
          <mat-icon matTooltip="We couldn't verify your insurance. Booking will proceed — verification will retry automatically.">help_outline</mat-icon>
        }
      </div>
      @if (coverage.copayEstimate !== null) {
        <p class="copay">Estimated Copay: <strong>\${{ coverage.copayEstimate }}</strong></p>
      }
      @if (coverage.limitations) {
        <p class="limitations">{{ coverage.limitations }}</p>
      }
    </div>
  `,
  styles: [`
    .coverage-result { padding: 16px; border-radius: 8px; margin-top: 16px; }
    .status-row { display: flex; align-items: center; gap: 8px; }
    .badge { padding: 6px 12px; border-radius: 16px; font-weight: 500; font-size: 14px; }
    .badge.status-covered { background: #c8e6c9; color: #2e7d32; }
    .badge.status-partial { background: #fff9c4; color: #f57f17; }
    .badge.status-inactive { background: #ffcdd2; color: #c62828; }
    .badge.status-pending, .badge.status-unknown { background: #e0e0e0; color: #616161; }
    .copay { margin-top: 8px; font-size: 16px; }
    .limitations { font-size: 13px; color: #666; margin-top: 4px; }
  `],
})
export class CoverageDisplayComponent {
  @Input() coverage!: InsuranceCheckResponse;

  getBadgeClass(): string {
    const status = this.coverage.coverageStatus.toLowerCase();
    if (status.includes('covered') && !status.includes('partial')) return 'status-covered';
    if (status.includes('partial')) return 'status-partial';
    if (status.includes('inactive')) return 'status-inactive';
    if (status.includes('pending')) return 'status-pending';
    return 'status-unknown';
  }
}
