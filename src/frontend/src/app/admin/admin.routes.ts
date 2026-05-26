import { Routes } from '@angular/router';
import { AdminShellComponent } from './admin-shell.component';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    component: AdminShellComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./dashboard/admin-dashboard.component').then(
            (m) => m.AdminDashboardComponent
          ),
      },
      {
        path: 'audit-log',
        loadComponent: () =>
          import('../features/admin/pages/audit-log/audit-log.component').then(
            (m) => m.AuditLogComponent
          ),
      },
    ],
  },
];
