import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { LoteHistory } from './lote-history';
import { LoteDetail } from '@models/lote';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

const mockDetail: LoteDetail = {
  id: 1,
  numero: 101,
  validade: '2027-06-30',
  quantidadeTotal: 43,
  historico: [
    { id: 1, tipo: 'Entrada', deltaQuantidade: 50, criadoEm: '2026-03-01T10:00:00Z' },
    { id: 2, tipo: 'SaidaConsignacao', deltaQuantidade: -7, criadoEm: '2026-03-02T14:00:00Z' },
  ],
};

describe('LoteHistory', () => {
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [LoteHistory],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('deve buscar o detalhe do lote ao inicializar', () => {
    const fixture = TestBed.createComponent(LoteHistory);
    fixture.componentRef.setInput('loteId', 1);
    fixture.detectChanges();

    httpTesting.expectOne('/api/lotes/1');
  });

  it('deve exibir skeleton enquanto carrega', () => {
    const fixture = TestBed.createComponent(LoteHistory);
    fixture.componentRef.setInput('loteId', 1);
    fixture.detectChanges();

    httpTesting.expectOne('/api/lotes/1');

    const skeleton = fixture.nativeElement.querySelector('.lote-history__skeleton');
    expect(skeleton).not.toBeNull();
  });

  it('deve exibir as movimentações após carregar', () => {
    const fixture = TestBed.createComponent(LoteHistory);
    fixture.componentRef.setInput('loteId', 1);
    fixture.detectChanges();

    httpTesting.expectOne('/api/lotes/1').flush(mockDetail);
    fixture.detectChanges();

    const rows = fixture.nativeElement.querySelectorAll('.lote-history__row');
    expect(rows.length).toBe(2);
  });

  it('deve exibir delta negativo com sinal de menos', () => {
    const fixture = TestBed.createComponent(LoteHistory);
    fixture.componentRef.setInput('loteId', 1);
    fixture.detectChanges();

    httpTesting.expectOne('/api/lotes/1').flush(mockDetail);
    fixture.detectChanges();

    const deltas = fixture.nativeElement.querySelectorAll('.lote-history__delta');
    expect(deltas[1].textContent).toContain('-7');
  });

  it('deve recarregar ao mudar de lote', () => {
    const fixture = TestBed.createComponent(LoteHistory);
    fixture.componentRef.setInput('loteId', 1);
    fixture.detectChanges();
    httpTesting.expectOne('/api/lotes/1').flush(mockDetail);
    fixture.detectChanges();

    fixture.componentRef.setInput('loteId', 2);
    fixture.detectChanges();

    httpTesting.expectOne('/api/lotes/2').flush({ ...mockDetail, id: 2, historico: [] });
    fixture.detectChanges();

    const rows = fixture.nativeElement.querySelectorAll('.lote-history__row');
    expect(rows.length).toBe(0);
  });
});
