import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter, Router, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { Vendas } from './vendas';
import { SalesResponse } from '@models/venda';

const mockResponse: SalesResponse = {
    items: [
        { id: 1042, clienteNome: 'Maria Silva',  criadaEm: '2026-02-27T10:00:00Z', status: 'Aberta',  totalItens: 8 },
        { id: 1041, clienteNome: 'João Santos',  criadaEm: '2026-02-26T09:00:00Z', status: 'Fechada', totalItens: 5 },
    ],
    total: 12,
};

describe('Vendas', () => {
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [Vendas],
            providers: [
                provideHttpClient(),
                provideHttpClientTesting(),
                provideRouter([]),
                { provide: ActivatedRoute, useValue: { queryParams: of({}) } },
            ],
        });
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => httpMock.verify());

    function setup() {
        const fixture = TestBed.createComponent(Vendas);
        fixture.detectChanges();
        return fixture;
    }

    function flush(fixture: ReturnType<typeof setup>, response = mockResponse) {
        httpMock.expectOne(r => r.url === '/api/vendas').flush(response);
        fixture.detectChanges();
    }

    it('deve criar o componente', () => {
        const fixture = setup();
        flush(fixture);
        expect(fixture.componentInstance).toBeTruthy();
    });

    it('deve exibir loading enquanto aguarda resposta', () => {
        const fixture = setup();
        expect(fixture.componentInstance.loading()).toBe(true);
        flush(fixture);
    });

    it('deve exibir as vendas e o total após receber a resposta', () => {
        const fixture = setup();
        flush(fixture);
        expect(fixture.componentInstance.sales()).toHaveLength(2);
        expect(fixture.componentInstance.total()).toBe(12);
    });

    it('deve buscar a primeira página com parâmetros padrão', () => {
        setup();
        const req = httpMock.expectOne(r => r.url === '/api/vendas');
        expect(req.request.params.get('pagina')).toBe('1');
        expect(req.request.params.get('tamanhoPagina')).toBe('10');
        req.flush(mockResponse);
    });

    it('deve exibir estado vazio quando não há vendas', () => {
        const fixture = setup();
        flush(fixture, { items: [], total: 0 });
        expect(fixture.nativeElement.querySelector('.sale-table__empty')).not.toBeNull();
    });

    it('deve abrir o drawer ao selecionar uma venda', () => {
        const fixture = setup();
        flush(fixture);
        fixture.componentInstance.onSaleSelect(1042);
        fixture.detectChanges();
        expect(fixture.componentInstance.selectedSaleId()).toBe(1042);
        expect(fixture.nativeElement.querySelector('.vendas__drawer--open')).not.toBeNull();
    });

    it('deve fechar o drawer ao limpar o selectedSaleId', () => {
        const fixture = setup();
        flush(fixture);
        fixture.componentInstance.onSaleSelect(1042);
        fixture.detectChanges();
        fixture.componentInstance.selectedSaleId.set(null);
        fixture.detectChanges();
        expect(fixture.nativeElement.querySelector('.vendas__drawer--open')).toBeNull();
    });

    it('deve navegar para página 1 com o termo de busca ao pesquisar', () => {
        const fixture = setup();
        flush(fixture);
        const router = TestBed.inject(Router);
        const spy = vi.spyOn(router, 'navigate');
        fixture.componentInstance.onSearch('maria');
        expect(spy).toHaveBeenCalledWith([], expect.objectContaining({
            queryParams: expect.objectContaining({ busca: 'maria', pagina: 1 }),
        }));
    });

    it('deve passar busca como null ao pesquisar com string vazia', () => {
        const fixture = setup();
        flush(fixture);
        const router = TestBed.inject(Router);
        const spy = vi.spyOn(router, 'navigate');
        fixture.componentInstance.onSearch('');
        expect(spy).toHaveBeenCalledWith([], expect.objectContaining({
            queryParams: expect.objectContaining({ busca: null }),
        }));
    });

    it('deve navegar para a página solicitada ao mudar de página', () => {
        const fixture = setup();
        flush(fixture);
        const router = TestBed.inject(Router);
        const spy = vi.spyOn(router, 'navigate');
        fixture.componentInstance.onPageChange(3);
        expect(spy).toHaveBeenCalledWith([], expect.objectContaining({
            queryParams: expect.objectContaining({ pagina: 3 }),
        }));
    });
});
