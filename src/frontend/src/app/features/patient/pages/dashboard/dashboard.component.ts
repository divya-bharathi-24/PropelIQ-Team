import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PatientService, DashboardData, AppointmentCard, ActivityItem } from '../../services/patient.service';
import { AppointmentCardComponent } from '../../components/appointment-card/appointment-card.component';
import { ActivityFeedComponent } from '../../components/activity-feed/activity-feed.component';
import { QuickActionsComponent } from '../../components/quick-actions/quick-actions.component';

@Component({
  selector: 'app-patient-dashboard',
  standalone: true,
  imports: [
    CommonModule, RouterLink, MatCardModule, MatButtonModule, MatIconModule,
    MatProgressSpinnerModule, AppointmentCardComponent, ActivityFeedComponent, QuickActionsComponent,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  appointments: AppointmentCard[] | null = null;
  activities: ActivityItem[] | null = null;
  loading = true;
  appointmentsError = false;
  activitiesError = false;

  constructor(private readonly patientService: PatientService) {}

  ngOnInit(): void {
    this.patientService.getDashboard('me').subscribe({
      next: (data: DashboardData) => {
        this.appointments = data.upcomingAppointments;
        this.activities = data.recentActivity;
        this.appointmentsError = data.upcomingAppointments === null;
        this.activitiesError = data.recentActivity === null;
        this.loading = false;
      },
      error: () => {
        this.appointmentsError = true;
        this.activitiesError = true;
        this.loading = false;
      },
    });
  }

  onQuickAction(_key: string): void {
    // Route navigation handled by parent shell or future implementation
  }
}
