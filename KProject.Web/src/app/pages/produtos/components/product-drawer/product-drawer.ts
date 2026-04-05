import { Component, effect, inject, input, output, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Drawer } from '@components/drawer/drawer';

interface ValidationError { code: string; description: string; }

@Component({
  selector: 'app-product-drawer',
  imports: [ReactiveFormsModule, Drawer],
  templateUrl: './product-drawer.html',
  styleUrl: './product-drawer.scss',
})
export class ProductDrawer {
  private http = inject(HttpClient);

  open = input<boolean>(false);
  close = output<void>();
  productCreated = output<void>();

  errors = signal<ValidationError[]>([]);
  saving = signal(false);
  saved = signal(false);

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
        this.saved.set(true);
        setTimeout(() => this.productCreated.emit(), 1500);
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        this.errors.set(err.error ?? []);
      },
    });
  }
}
