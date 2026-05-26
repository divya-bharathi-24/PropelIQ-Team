import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AppointmentMgmtService, AppointmentListItem } from '../../services/appointment-mgmt.service';
import { ApptCardComponent } from '../../components/appointment-card/appointment-card.component';
import { CancelDialogComponent, CancelDialogData } from '../../components/cancel-dialog/cancel-dialog.component';
import { RescheduleDialogComponent, RescheduleDialogData } from '../../components/reschedule-dialog/reschedule-dialog.component';

@Component({
  selector: 'app-appointment-list',
  standalone: true,
  imports: [
    CommonModule, MatDialogModule, MatPaginatorModule, MatFormFieldModule,
    MatSelectModule, MatProgressSpinnerModule, MatSnackBarModule, ApptCardComponent,
  ],
  templateUrl: './appointment-list.component.html',
  styleUrl: './appointment-list.component.scss',
})
export class AppointmentListComponent implements OnInit {
  appointments: AppointmentListItem[] = [];
  totalCount = 0;
  page = 1;
  statusFilter = '';
  loading = true;
  private readonly patientId = 'me';

  constructor(
    private readonly apptService: AppointmentMgmtService,
    private readonly dialog: MatDialog,
    private readonly snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.loadAppointments();
  }

  loadAppointments(): void {
    this.loading = true;
    this.apptService.getAppointments(this.patientId, this.page, this.statusFilter || undefined)
      .subscribe({
        next: res => {
          this.appointments = res.items;
          this.totalCount = res.totalCount;
          this.loading = false;
        },
        error: () => { this.loading = false; },
      });
  }

  onPageChange(event: PageEvent): void {
    this.page = event.pageIndex + 1;
    this.loadAppointments();
  }

  onStatusFilterChange(status: string): void {
    this.statusFilter = status;
    this.page = 1;
    this.loadAppointments();
  }

  onReschedule(appt: AppointmentListItem): void {
    const hoursUntil = (new Date(appt.scheduledAt).getTime() - Date.now()) / 3600000;
    const ref = this.dialog.open(RescheduleDialogComponent, {
      data: {
        appointmentId: appt.id,
        currentScheduledAt: appt.scheduledAt,
        isLate: hoursUntil < 24,
      } as RescheduleDialogData,
      width: '600px',
    });

    ref.afterClosed().subscribe(newSlotId => {
      if (newSlotId) {
        this.apptService.reschedule(appt.id, newSlotId).subscribe({
          next: (res) => {
            this.snackBar.open(res.message, 'Close', { duration: 3000 });
            this.loadAppointments();
          },
          error: (err) => this.snackBar.open(err.error?.message ?? 'Reschedule failed', 'Close', { duration: 3000 }),
        });
      }
    });
  }

  onCancel(appt: AppointmentListItem): void {
    const hoursUntil = (new Date(appt.scheduledAt).getTime() - Date.now()) / 3600000;
    const ref = this.dialog.open(CancelDialogComponent, {
      data: {
        appointmentId: appt.id,
        scheduledAt: appt.scheduledAt,
        isLate: hoursUntil < 24,
      } as CancelDialogData,
    });

    ref.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.apptService.cancel(appt.id).subscribe({
          next: (res) => {
            this.snackBar.open(res.message, 'Close', { duration: 3000 });
            this.loadAppointments();
          },
          error: (err) => this.snackBar.open(err.error?.message ?? 'Cancellation failed', 'Close', { duration: 3000 }),
        });
      }
    });
  }

  onDownloadPdf(appt: AppointmentListItem): void {
    this.apptService.downloadPdf(appt.id).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `confirmation-${appt.id}.pdf`;
        a.click();
        URL.revokeObjectURL(url);
      },
      error: () => this.snackBar.open('PDF will be available shortly', 'Close', { duration: 3000 }),
    });
  }
}
