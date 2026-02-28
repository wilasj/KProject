import {Component, inject, signal} from '@angular/core';
import {Router, RouterLink} from '@angular/router';
import {Auth} from '@core/auth';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';

@Component({
  selector: 'app-login',
  imports: [
    RouterLink,
    ReactiveFormsModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private authService = inject(Auth);
  private router = inject(Router);
  public errors = signal<ValidationError[]>([]);

  loginForm = new FormGroup({
    email: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.email]}),
    password: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
  });

  onSubmit() {
    if (this.loginForm.valid) {
      const {email, password} = this.loginForm.getRawValue();

      this.authService
        .login(email, password)
        .subscribe({
          next: (result) => {
            if (result.success) {
              this.router.navigate(['/vendas']);
            } else {
              this.errors.set(result.errors);
            }
          }
        });
    }
  }
}
