import { Component, viewChild } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter, Router, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { PageLayout, PageLayoutConfig, PageLayoutContentDef, PageLayoutDrawerDef } from './page-layout';

interface MockItem {
  id: number;
  name: string;
}

@Component({
  imports: [PageLayout, PageLayoutContentDef, PageLayoutDrawerDef],
  template: `
    <app-page-layout [config]="config">
      <ng-template pageLayoutContent let-items let-total="total">
        <div class="test-content">
          @for (item of items; track item.id) {
            <div class="test-row">{{ item.name }}</div>
          }
        </div>
      </ng-template>
      <ng-template pageLayoutDrawer>
        <div class="test-drawer">Drawer</div>
      </ng-template>
    </app-page-layout>
  `,
})
class TestHost {
  layout = viewChild.required(PageLayout);
  config: PageLayoutConfig = {
    title: 'Testes',
    buttonLabel: 'Novo Teste',
    searchPlaceholder: 'Buscar testes...',
    endpoint: '/api/testes',
    pageSize: 10,
  };
}

describe('PageLayout', () => {
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      imports: [TestHost],
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

  function setup(response = { items: [] as MockItem[], total: 0 }) {
    const fixture = TestBed.createComponent(TestHost);
    fixture.detectChanges();
    httpMock.expectOne(r => r.url === '/api/testes').flush(response);
    fixture.detectChanges();
    return fixture;
  }

  it('deve renderizar o título e o botão', () => {
    const fixture = setup();
    expect(fixture.nativeElement.querySelector('.page-layout__title').textContent).toContain('Testes');
    expect(fixture.nativeElement.querySelector('.page-layout__btn-new').textContent).toContain('Novo Teste');
  });

  it('deve exibir loading enquanto aguarda resposta', () => {
    const fixture = TestBed.createComponent(TestHost);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.page-layout__loading')).not.toBeNull();
    httpMock.expectOne(r => r.url === '/api/testes').flush({ items: [], total: 0 });
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.page-layout__loading')).toBeNull();
  });

  it('deve renderizar o conteúdo com os items da resposta', () => {
    const fixture = setup({
      items: [{ id: 1, name: 'A' }, { id: 2, name: 'B' }],
      total: 2,
    });
    const rows = fixture.nativeElement.querySelectorAll('.test-row');
    expect(rows.length).toBe(2);
    expect(rows[0].textContent).toContain('A');
  });

  it('deve abrir o drawer ao clicar no botão', () => {
    const fixture = setup();
    fixture.nativeElement.querySelector('.page-layout__btn-new').click();
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.page-layout__drawer--open')).not.toBeNull();
  });

  it('deve fechar o drawer ao chamar closeDrawer', () => {
    const fixture = setup();
    fixture.componentInstance.layout().drawerOpen.set(true);
    fixture.detectChanges();
    fixture.componentInstance.layout().closeDrawer();
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.page-layout__drawer--open')).toBeNull();
  });

  it('deve buscar com parâmetros de busca e página', () => {
    const fixture = setup();
    const router = TestBed.inject(Router);
    const spy = vi.spyOn(router, 'navigate');
    fixture.componentInstance.layout().onSearch('teste');
    expect(spy).toHaveBeenCalledWith([], expect.objectContaining({
      queryParams: expect.objectContaining({ busca: 'teste', pagina: 1 }),
    }));
  });

  it('deve navegar para a página solicitada ao mudar de página', () => {
    const fixture = setup();
    const router = TestBed.inject(Router);
    const spy = vi.spyOn(router, 'navigate');
    fixture.componentInstance.layout().onPageChange(3);
    expect(spy).toHaveBeenCalledWith([], expect.objectContaining({
      queryParams: expect.objectContaining({ pagina: 3 }),
    }));
  });

  it('deve fechar o drawer e refazer o fetch ao chamar onItemSaved', () => {
    const fixture = setup();
    fixture.componentInstance.layout().drawerOpen.set(true);
    fixture.componentInstance.layout().onItemSaved();
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.page-layout__drawer--open')).toBeNull();
    httpMock.expectOne(r => r.url === '/api/testes').flush({ items: [], total: 0 });
  });
});
