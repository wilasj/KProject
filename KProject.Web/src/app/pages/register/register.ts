import {Component, inject, signal} from '@angular/core';
import {Router, RouterLink} from '@angular/router';
import {Auth} from '@core/auth';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';

@Component({
  selector: 'app-register',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  private authService = inject(Auth);
  private router = inject(Router);
  public errors = signal<ValidationError[]>([]);
  public loading = signal(false);

  registerForm = new FormGroup({
    email: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.email]}),
    password: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
  });

  onSubmit() {
    if (this.registerForm.valid) {
      const {email, password} = this.registerForm.getRawValue();
      this.loading.set(true);
      this.errors.set([]);

      this.authService.register(email, password).subscribe({
        next: (result) => {
          if (result.success) {
            this.router.navigate(['/login']);
          } else {
            this.loading.set(false);
            this.errors.set(result.errors);
          }
        },
        error: () => this.loading.set(false),
      });
    }
  }
}
