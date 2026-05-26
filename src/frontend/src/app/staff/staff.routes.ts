import { Routes } from '@angular/router';
import { StaffShellComponent } from './staff-shell.component';

export const STAFF_ROUTES: Routes = [
  {
    path: '',
    component: StaffShellComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./dashboard/staff-dashboard.component').then(
            (m) => m.StaffDashboardComponent
          ),
      },
      {
        path: 'create-patient',
        loadComponent: () =>
          import('../features/staff/pages/create-patient/create-patient.component').then(
            (m) => m.CreatePatientComponent
          ),
      },
    ],
  },
];
