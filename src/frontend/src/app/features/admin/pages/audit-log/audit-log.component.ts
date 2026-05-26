import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSortModule, MatSort, Sort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuditService, AuditLogEntry } from '../../services/audit.service';

@Component({
  selector: 'app-audit-log',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatTableModule, MatPaginatorModule, MatSortModule,
    MatFormFieldModule, MatInputModule, MatSelectModule,
    MatDatepickerModule, MatNativeDateModule,
    MatButtonModule, MatChipsModule, MatProgressSpinnerModule,
  ],
  template: `
    <div class="audit-log-page">
      <h1>Authentication Audit Log</h1>

      <div class="filter-panel">
        <form [formGroup]="filterForm" (ngSubmit)="applyFilters()">
          <mat-form-field appearance="outline">
            <mat-label>User ID</mat-label>
            <input matInput formControlName="userId" placeholder="UUID">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Event Type</mat-label>
            <mat-select formControlName="eventType">
              <mat-option value="">All</mat-option>
              <mat-option value="login">Login</mat-option>
              <mat-option value="logout">Logout</mat-option>
              <mat-option value="token_refresh">Token Refresh</mat-option>
              <mat-option value="password_change">Password Change</mat-option>
              <mat-option value="staff_create_patient">Staff Create Patient</mat-option>
              <mat-option value="refresh_token_reuse">Replay Attack</mat-option>
            </mat-select>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Start Date</mat-label>
            <input matInput [matDatepicker]="startPicker" formControlName="startDate">
            <mat-datepicker-toggle matIconSuffix [for]="startPicker"></mat-datepicker-toggle>
            <mat-datepicker #startPicker></mat-datepicker>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>End Date</mat-label>
            <input matInput [matDatepicker]="endPicker" formControlName="endDate">
            <mat-datepicker-toggle matIconSuffix [for]="endPicker"></mat-datepicker-toggle>
            <mat-datepicker #endPicker></mat-datepicker>
          </mat-form-field>

          <button mat-raised-button color="primary" type="submit">Search</button>
          <button mat-stroked-button type="button" (click)="clearFilters()">Clear</button>
        </form>
      </div>

      @if (loading) {
        <div class="loading"><mat-spinner diameter="40"></mat-spinner></div>
      }

      <table mat-table [dataSource]="dataSource" matSort (matSortChange)="onSort($event)">
        <ng-container matColumnDef="eventType">
          <th mat-header-cell *matHeaderCellDef mat-sort-header>Event Type</th>
          <td mat-cell *matCellDef="let row">
            <span class="event-chip" [class.success]="row.successful" [class.failure]="!row.successful">
              {{ row.eventType }}
            </span>
          </td>
        </ng-container>

        <ng-container matColumnDef="email">
          <th mat-header-cell *matHeaderCellDef mat-sort-header>Email</th>
          <td mat-cell *matCellDef="let row">{{ row.email }}</td>
        </ng-container>

        <ng-container matColumnDef="successful">
          <th mat-header-cell *matHeaderCellDef mat-sort-header>Status</th>
          <td mat-cell *matCellDef="let row">
            {{ row.successful ? 'Success' : 'Failed' }}
            @if (row.failureReason) { <span class="reason"> — {{ row.failureReason }}</span> }
          </td>
        </ng-container>

        <ng-container matColumnDef="ipAddress">
          <th mat-header-cell *matHeaderCellDef>IP Address</th>
          <td mat-cell *matCellDef="let row">{{ row.ipAddress }}</td>
        </ng-container>

        <ng-container matColumnDef="attemptedAt">
          <th mat-header-cell *matHeaderCellDef mat-sort-header>Timestamp</th>
          <td mat-cell *matCellDef="let row">{{ row.attemptedAt | date:'medium' }}</td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
      </table>

      <mat-paginator [length]="totalCount" [pageSize]="50"
                     [pageSizeOptions]="[25, 50, 100]"
                     (page)="onPage($event)">
      </mat-paginator>
    </div>
  `,
  styles: [`
    .audit-log-page { padding: 24px; max-width: 1200px; margin: 0 auto; }
    .filter-panel form { display: flex; flex-wrap: wrap; gap: 12px; align-items: flex-end; margin-bottom: 16px; }
    .filter-panel mat-form-field { flex: 1; min-width: 180px; }
    .loading { display: flex; justify-content: center; padding: 32px; }
    .event-chip { padding: 4px 8px; border-radius: 4px; font-size: 12px; }
    .event-chip.success { background: #e8f5e9; color: #1b5e20; }
    .event-chip.failure { background: #fdecea; color: #611a15; }
    .reason { color: #666; font-size: 12px; }
    table { width: 100%; }
  `],
})
export class AuditLogComponent implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  filterForm: FormGroup;
  dataSource = new MatTableDataSource<AuditLogEntry>([]);
  displayedColumns = ['eventType', 'email', 'successful', 'ipAddress', 'attemptedAt'];
  totalCount = 0;
  loading = false;
  private currentPage = 1;
  private currentSort = 'attemptedAt';
  private currentDirection = 'desc';

  constructor(
    private readonly fb: FormBuilder,
    private readonly auditService: AuditService,
  ) {
    this.filterForm = this.fb.group({
      userId: [''],
      eventType: [''],
      startDate: [null],
      endDate: [null],
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  applyFilters(): void {
    this.currentPage = 1;
    this.loadData();
  }

  clearFilters(): void {
    this.filterForm.reset();
    this.currentPage = 1;
    this.loadData();
  }

  onSort(sort: Sort): void {
    this.currentSort = sort.active;
    this.currentDirection = sort.direction || 'desc';
    this.loadData();
  }

  onPage(event: PageEvent): void {
    this.currentPage = event.pageIndex + 1;
    this.loadData();
  }

  private loadData(): void {
    this.loading = true;
    const f = this.filterForm.value;

    this.auditService.getAuditLog({
      userId: f.userId || undefined,
      eventType: f.eventType || undefined,
      startDate: f.startDate ? new Date(f.startDate).toISOString() : undefined,
      endDate: f.endDate ? new Date(f.endDate).toISOString() : undefined,
      page: this.currentPage,
      pageSize: 50,
      sortBy: this.currentSort,
      sortDirection: this.currentDirection,
    }).subscribe({
      next: (res) => {
        this.loading = false;
        this.dataSource.data = res.items;
        this.totalCount = res.totalCount;
      },
      error: () => {
        this.loading = false;
      },
    });
  }
}
