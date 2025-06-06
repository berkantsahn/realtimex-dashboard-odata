import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  template: `
    <router-outlet></router-outlet>
  `,
  styles: [`
    :host {
      display: block;
      height: 100vh;
    }
  `],
  standalone: false
})
export class AppComponent {
  title = 'realtimex-dashboard';
} 