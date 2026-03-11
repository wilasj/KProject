import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter, Router, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { Clientes } from './clientes';
import { ClientesResponse } from '@models/cliente';

const mockResponse: ClientesResponse = {
  items: [
    { id: 1, nome: 'Maria Silva' },
    { id: 2, nome: 'João Santos' },
  ],
  total: 7,
};

describe('Clientes', () => {
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [Clientes],
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
    const fixture = TestBed.createComponent(Clientes);
    fixture.detectChanges();
    return fixture;
  }

  function flush(fixture: ReturnType<typeof setup>, response = mockResponse) {
    httpMock.expectOne(r => r.url === '/api/clientes').flush(response);
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

  it('deve exibir os clientes e o total após receber a resposta', () => {
    const fixture = setup();
    flush(fixture);
    expect(fixture.componentInstance.clients()).toHaveLength(2);
    expect(fixture.componentInstance.total()).toBe(7);
  });

  it('deve buscar a primeira página com parâmetros padrão', () => {
    setup();
    const req = httpMock.expectOne(r => r.url === '/api/clientes');
    expect(req.request.params.get('pagina')).toBe('1');
    expect(req.request.params.get('tamanhoPagina')).toBe('10');
    req.flush(mockResponse);
  });

  it('deve exibir estado vazio quando não há clientes', () => {
    const fixture = setup();
    flush(fixture, { items: [], total: 0 });
    expect(fixture.nativeElement.querySelector('.client-table__empty')).not.toBeNull();
  });

  it('deve abrir o drawer ao clicar em Novo Cliente', () => {
    const fixture = setup();
    flush(fixture);
    fixture.componentInstance.drawerOpen.set(true);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.clientes__drawer--open')).not.toBeNull();
  });

  it('deve fechar o drawer ao definir drawerOpen como false', () => {
    const fixture = setup();
    flush(fixture);
    fixture.componentInstance.drawerOpen.set(true);
    fixture.detectChanges();
    fixture.componentInstance.drawerOpen.set(false);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.clientes__drawer--open')).toBeNull();
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
