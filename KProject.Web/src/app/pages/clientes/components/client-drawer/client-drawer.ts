import { Component, effect, inject, input, output, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';

interface ValidationError { code: string; description: string; }

@Component({
  selector: 'app-client-drawer',
  imports: [ReactiveFormsModule],
  templateUrl: './client-drawer.html',
  styleUrl: './client-drawer.scss',
})
export class ClientDrawer {
  private http = inject(HttpClient);

  open = input<boolean>(false);
  close = output<void>();
  clientCreated = output<void>();

  errors = signal<ValidationError[]>([]);
  saving = signal(false);
  saved  = signal(false);

  constructor() {
    effect(() => {
      if (this.open()) {
        this.form.reset();
        this.errors.set([]);
        this.saved.set(false);
      }
    });
  }

  form = new FormGroup({
    nome: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(200)] }),
  });

  onSubmit() {
    if (this.form.invalid) return;
    this.saving.set(true);
    this.errors.set([]);

    this.http.post<{ id: number }>('/api/clientes', this.form.getRawValue()).subscribe({
      next: () => {
        this.saving.set(false);
        this.saved.set(true);
        setTimeout(() => this.clientCreated.emit(), 1500);
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        this.errors.set(err.error ?? []);
      },
    });
  }
}
