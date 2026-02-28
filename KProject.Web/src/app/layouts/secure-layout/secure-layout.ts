import {Component, inject} from '@angular/core';
import {Router, RouterOutlet} from '@angular/router';
import {Auth} from '@core/auth';
import {Sidebar} from './components/sidebar/sidebar';

@Component({
  selector: 'app-secure-layout',
  imports: [RouterOutlet, Sidebar],
  templateUrl: './secure-layout.html',
  styleUrl: './secure-layout.scss',
})
export class SecureLayout {
  private auth = inject(Auth);
  private router = inject(Router);

  onLogout() {
    this.auth.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }
}
