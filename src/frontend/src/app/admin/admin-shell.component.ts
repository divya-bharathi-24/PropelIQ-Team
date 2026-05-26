import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-admin-shell',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <section class="admin-shell">
      <router-outlet />
    </section>
  `,
})
export class AdminShellComponent {}
