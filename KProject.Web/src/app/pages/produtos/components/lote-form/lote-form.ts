import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-lote-form',
  imports: [FormsModule],
  templateUrl: './lote-form.html',
  styleUrl: './lote-form.scss',
})
export class LoteForm {
  private http = inject(HttpClient);

  productId = input.required<number>();

  loteCriado = output<void>();
  cancelar = output<void>();

  numero = signal('');
  validade = signal('');
  quantidade = signal('');
  submitting = signal(false);

  onSubmit() {
    if (this.submitting()) return;
    this.submitting.set(true);
    this.http.post('/api/lotes', {
      produtoId: this.productId(),
      numero: Number(this.numero()),
      validade: this.validade(),
      quantidadeInicial: Number(this.quantidade()),
    }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.loteCriado.emit();
      },
      error: () => this.submitting.set(false),
    });
  }

  onCancelar() {
    this.cancelar.emit();
  }
}
