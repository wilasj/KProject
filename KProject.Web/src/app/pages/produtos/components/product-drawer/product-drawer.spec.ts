import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { ProductDrawer } from './product-drawer';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';

describe('ProductDrawer', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ProductDrawer],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('deve desabilitar o botão salvar com form inválido', () => {
    const fixture = TestBed.createComponent(ProductDrawer);
    fixture.detectChanges();
    const button = fixture.nativeElement.querySelector('button[type="submit"]');
    expect(button.disabled).toBe(true);
  });

  it('deve habilitar o botão salvar com form válido', () => {
    const fixture = TestBed.createComponent(ProductDrawer);
    const component = fixture.componentInstance;
    component.form.setValue({
      nome: 'Produto Teste',
      referencia: 'REF-001',
      descricao: 'Descrição teste',
      codigoAnvisa: 'ANVISA-001',
    });
    fixture.detectChanges();
    const button = fixture.nativeElement.querySelector('button[type="submit"]');
    expect(button.disabled).toBe(false);
  });

  it('deve emitir productCreated após POST com sucesso', () => {
    vi.useFakeTimers();

    const fixture = TestBed.createComponent(ProductDrawer);
    const http = TestBed.inject(HttpTestingController);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    let emitted = false;
    component.productCreated.subscribe(() => emitted = true);

    component.form.setValue({
      nome: 'Produto Teste',
      referencia: 'REF-001',
      descricao: 'Descrição',
      codigoAnvisa: 'ANVISA-001',
    });
    component.onSubmit();

    const req = http.expectOne('/api/produtos');
    req.flush({ id: 1 });

    expect(component.saved()).toBe(true);
    expect(emitted).toBe(false);

    vi.advanceTimersByTime(1500);
    expect(emitted).toBe(true);
    http.verify();
  });
});
