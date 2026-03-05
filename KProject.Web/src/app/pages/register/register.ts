import {Component, inject, OnInit, signal} from '@angular/core';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {Auth} from '@core/auth';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';

@Component({
  selector: 'app-register',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register implements OnInit {
  private authService = inject(Auth);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  public errors = signal<ValidationError[]>([]);
  public loading = signal(false);

  private inviteToken = '';

  registerForm = new FormGroup({
    email: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.email]}),
    password: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
  });

  ngOnInit() {
    const token = this.route.snapshot.queryParamMap.get('token');
    if (!token) {
      this.router.navigate(['/login']);
      return;
    }
    this.inviteToken = token;
  }

  onSubmit() {
    if (this.registerForm.valid) {
      const {email, password} = this.registerForm.getRawValue();
      this.loading.set(true);
      this.errors.set([]);

      this.authService.register(email, password, this.inviteToken).subscribe({
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
