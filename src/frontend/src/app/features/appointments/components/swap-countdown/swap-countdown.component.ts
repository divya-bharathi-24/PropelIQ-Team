import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Subject, timer, takeUntil, map, takeWhile } from 'rxjs';

@Component({
  selector: 'app-swap-countdown',
  standalone: true,
  imports: [CommonModule, MatProgressBarModule],
  template: `
    <div class="countdown-container">
      <mat-progress-bar mode="determinate" [value]="progressPercent"></mat-progress-bar>
      <p class="timer-text">
        @if (remainingSeconds > 0) {
          {{ minutes }}:{{ paddedSeconds }} remaining
        } @else {
          Expired
        }
      </p>
    </div>
  `,
  styles: [`
    .countdown-container { padding: 8px 0; }
    .timer-text { text-align: center; font-size: 14px; font-weight: 500; margin-top: 4px; }
  `],
})
export class SwapCountdownComponent implements OnInit, OnDestroy {
  @Input() expiresAt!: string;
  @Output() expired = new EventEmitter<void>();

  remainingSeconds = 0;
  progressPercent = 100;

  private totalSeconds = 15 * 60;
  private readonly destroy$ = new Subject<void>();

  get minutes(): number {
    return Math.floor(this.remainingSeconds / 60);
  }

  get paddedSeconds(): string {
    return (this.remainingSeconds % 60).toString().padStart(2, '0');
  }

  ngOnInit(): void {
    const expiresMs = new Date(this.expiresAt).getTime();
    this.totalSeconds = Math.max(0, Math.floor((expiresMs - Date.now()) / 1000));
    this.remainingSeconds = this.totalSeconds;

    timer(0, 1000).pipe(
      takeUntil(this.destroy$),
      map(tick => this.totalSeconds - tick),
      takeWhile(remaining => remaining >= 0),
    ).subscribe(remaining => {
      this.remainingSeconds = remaining;
      this.progressPercent = this.totalSeconds > 0 ? (remaining / this.totalSeconds) * 100 : 0;

      if (remaining <= 0) {
        this.expired.emit();
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
