import { Routes } from '@angular/router';

export const AUTH_ROUTES: Routes = [
  {
    path: 'register',
    loadComponent: () =>
      import('./pages/registration/registration.component').then(m => m.RegistrationComponent),
  },
  {
    path: 'verify-email',
    loadComponent: () =>
      import('./pages/verify-email/verify-email.component').then(m => m.VerifyEmailComponent),
  },
  {
    path: 'activate',
    loadComponent: () =>
      import('./pages/activate/activate.component').then(m => m.ActivateComponent),
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login/login.component').then(m => m.LoginComponent),
  },
  {
    path: 'force-password-change',
    loadComponent: () =>
      import('./pages/force-password-change/force-password-change.component')
        .then(m => m.ForcePasswordChangeComponent),
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
