import { Component, inject, output, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';

interface ValidationError { code: string; description: string; }

@Component({
  selector: 'app-product-drawer',
  imports: [ReactiveFormsModule],
  templateUrl: './product-drawer.html',
  styleUrl: './product-drawer.scss',
})
export class ProductDrawer {
  private http = inject(HttpClient);

  close = output<void>();
  productCreated = output<void>();

  errors = signal<ValidationError[]>([]);
  saving = signal(false);

  form = new FormGroup({
    nome:         new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(100)] }),
    referencia:   new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(100)] }),
    descricao:    new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(300)] }),
    codigoAnvisa: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(100)] }),
  });

  onSubmit() {
    if (this.form.invalid) return;
    this.saving.set(true);
    this.errors.set([]);

    this.http.post<{ id: number }>('/api/produtos', this.form.getRawValue()).subscribe({
      next: () => {
        this.saving.set(false);
        this.productCreated.emit();
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        this.errors.set(err.error ?? []);
      },
    });
  }
}
