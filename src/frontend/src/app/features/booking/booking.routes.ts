import { Routes } from '@angular/router';

export const BOOKING_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/booking/booking.component').then(m => m.BookingComponent),
  },
];
