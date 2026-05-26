import { Routes } from '@angular/router';
import { PatientShellComponent } from './patient-shell.component';

export const PATIENT_ROUTES: Routes = [
  {
    path: '',
    component: PatientShellComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./dashboard/patient-dashboard.component').then(
            (m) => m.PatientDashboardComponent
          ),
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('../features/patient/pages/profile/profile.component').then(
            (m) => m.ProfileComponent
          ),
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('../features/patient/pages/dashboard/dashboard.component').then(
            (m) => m.DashboardComponent
          ),
      },
    ],
  },
];
