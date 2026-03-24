import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { LoteForm } from './lote-form';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

describe('LoteForm', () => {
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      imports: [LoteForm],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('deve renderizar campos de número, validade e quantidade', () => {
    const fixture = TestBed.createComponent(LoteForm);
    fixture.componentRef.setInput('productId', 1);
    fixture.detectChanges();

    const inputs = fixture.nativeElement.querySelectorAll('input');
    expect(inputs.length).toBe(3);
  });

  it('deve emitir loteCriado após submeter com sucesso', () => {
    const fixture = TestBed.createComponent(LoteForm);
    fixture.componentRef.setInput('productId', 1);
    fixture.detectChanges();

    const [numeroInput, validadeInput, quantidadeInput] = fixture.nativeElement.querySelectorAll('input');
    numeroInput.value = '101';
    numeroInput.dispatchEvent(new Event('input'));
    validadeInput.value = '2027-06-30';
    validadeInput.dispatchEvent(new Event('input'));
    quantidadeInput.value = '50';
    quantidadeInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    let emitted = false;
    fixture.componentInstance.loteCriado.subscribe(() => emitted = true);

    fixture.nativeElement.querySelector('.lote-form__submit').click();
    fixture.detectChanges();

    httpTesting.expectOne('/api/lotes').flush({ id: 10 });
    fixture.detectChanges();

    expect(emitted).toBe(true);
  });

  it('deve emitir cancelar ao clicar no botão de voltar', () => {
    const fixture = TestBed.createComponent(LoteForm);
    fixture.componentRef.setInput('productId', 1);
    fixture.detectChanges();

    let emitted = false;
    fixture.componentInstance.cancelar.subscribe(() => emitted = true);

    fixture.nativeElement.querySelector('.lote-form__back').click();
    expect(emitted).toBe(true);
  });
});
