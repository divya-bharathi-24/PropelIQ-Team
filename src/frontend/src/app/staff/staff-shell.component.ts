import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-staff-shell',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <section class="staff-shell">
      <router-outlet />
    </section>
  `,
})
export class StaffShellComponent {}
