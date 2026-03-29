import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { LoteHistory } from './lote-history';
import { Lote, HistoricoPage } from '@models/lote';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

const mockLote: Lote = { id: 1, numero: 101, validade: '2027-06-30', quantidadeTotal: 43 };

const mockPage1: HistoricoPage = {
  items: [
    { id: 1, tipo: 'Entrada', deltaQuantidade: 50, criadoEm: '2026-03-01T10:00:00Z' },
    { id: 2, tipo: 'SaidaConsignacao', deltaQuantidade: -7, criadoEm: '2026-03-02T14:00:00Z', vendaId: 1042 },
  ],
  hasMore: true,
};

const mockPage2: HistoricoPage = {
  items: [
    { id: 3, tipo: 'Ajuste', deltaQuantidade: 5, criadoEm: '2026-03-03T09:00:00Z' },
  ],
  hasMore: false,
};

describe('LoteHistory', () => {
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.resetTestingModule();
    (window as unknown as Record<string, unknown>)['IntersectionObserver'] = class {
      observe() {}
      disconnect() {}
    };

    TestBed.configureTestingModule({
      imports: [LoteHistory],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  function setup(lote = mockLote) {
    const fixture = TestBed.createComponent(LoteHistory);
    fixture.componentRef.setInput('lote', lote);
    fixture.detectChanges();
    return fixture;
  }

  function flush(fixture: ReturnType<typeof setup>) {
    httpTesting.expectOne('/api/lotes/1/historico?pagina=1&tamanhoPagina=10').flush(mockPage1);
    fixture.detectChanges();
  }

  it('deve fazer uma requisição ao inicializar', () => {
    setup();
    httpTesting.expectOne('/api/lotes/1/historico?pagina=1&tamanhoPagina=10');
  });

  it('deve exibir skeleton enquanto carrega', () => {
    const fixture = setup();
    httpTesting.expectOne('/api/lotes/1/historico?pagina=1&tamanhoPagina=10');

    expect(fixture.nativeElement.querySelector('.lote-history__skeleton')).not.toBeNull();
  });

  it('deve exibir as movimentações após carregar', () => {
    const fixture = setup();
    flush(fixture);

    const rows = fixture.nativeElement.querySelectorAll('.lote-history__row');
    expect(rows.length).toBe(2);
  });

  it('deve exibir delta negativo com sinal de menos', () => {
    const fixture = setup();
    flush(fixture);

    const deltas = fixture.nativeElement.querySelectorAll('.lote-history__delta');
    expect(deltas[1].textContent).toContain('-7');
  });

  it('deve carregar próxima página ao chamar loadMore', () => {
    const fixture = setup();
    flush(fixture);

    fixture.componentInstance.loadMore();
    fixture.detectChanges();

    httpTesting.expectOne('/api/lotes/1/historico?pagina=2&tamanhoPagina=10').flush(mockPage2);
    fixture.detectChanges();

    const rows = fixture.nativeElement.querySelectorAll('.lote-history__row');
    expect(rows.length).toBe(3);
  });

  it('não deve carregar mais quando hasMore é false', () => {
    const fixture = setup();
    httpTesting.expectOne('/api/lotes/1/historico?pagina=1&tamanhoPagina=10').flush({ ...mockPage1, hasMore: false });
    fixture.detectChanges();

    fixture.componentInstance.loadMore();
    httpTesting.expectNone('/api/lotes/1/historico?pagina=2&tamanhoPagina=10');
  });

  it('deve reiniciar ao mudar de lote', () => {
    const fixture = setup();
    flush(fixture);

    const lote2: Lote = { ...mockLote, id: 2, numero: 202 };
    fixture.componentRef.setInput('lote', lote2);
    fixture.detectChanges();

    httpTesting.expectOne('/api/lotes/2/historico?pagina=1&tamanhoPagina=10').flush({ items: [], hasMore: false });
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelectorAll('.lote-history__row').length).toBe(0);
  });

  it('deve exibir link da venda quando vendaId está presente', () => {
    const fixture = setup();
    flush(fixture);

    const vendaLinks = fixture.nativeElement.querySelectorAll('.lote-history__venda');
    expect(vendaLinks.length).toBe(1);
    expect(vendaLinks[0].textContent).toContain('#1042');
  });

  it('não deve exibir link da venda quando vendaId está ausente', () => {
    const fixture = setup();
    httpTesting.expectOne('/api/lotes/1/historico?pagina=1&tamanhoPagina=10').flush({
      items: [
        { id: 1, tipo: 'Entrada', deltaQuantidade: 50, criadoEm: '2026-03-01T10:00:00Z' },
      ],
      hasMore: false,
    });
    fixture.detectChanges();

    const vendaLinks = fixture.nativeElement.querySelectorAll('.lote-history__venda');
    expect(vendaLinks.length).toBe(0);
  });
});
