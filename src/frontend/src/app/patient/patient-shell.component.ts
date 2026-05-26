import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-patient-shell',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <section class="patient-shell">
      <router-outlet />
    </section>
  `,
})
export class PatientShellComponent {}
