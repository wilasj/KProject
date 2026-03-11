import { TestBed } from '@angular/core/testing';
import { ClientTable } from './client-table';
import { Cliente } from '@models/cliente';
import { describe, it, expect, beforeEach } from 'vitest';

const mockClients: Cliente[] = [
  { id: 1, nome: 'Cliente A' },
  { id: 2, nome: 'Cliente B' },
];

describe('ClientTable', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [ClientTable] });
  });

  it('deve renderizar os clientes recebidos', () => {
    const fixture = TestBed.createComponent(ClientTable);
    fixture.componentRef.setInput('clients', mockClients);
    fixture.componentRef.setInput('total', 2);
    fixture.componentRef.setInput('currentPage', 1);
    fixture.detectChanges();
    const rows = fixture.nativeElement.querySelectorAll('.client-table__row');
    expect(rows.length).toBe(2);
  });

  it('deve exibir estado vazio quando não há clientes', () => {
    const fixture = TestBed.createComponent(ClientTable);
    fixture.componentRef.setInput('clients', []);
    fixture.componentRef.setInput('total', 0);
    fixture.componentRef.setInput('currentPage', 1);
    fixture.detectChanges();
    const empty = fixture.nativeElement.querySelector('.client-table__empty');
    expect(empty).not.toBeNull();
  });

  it('deve emitir pageChange ao clicar em próxima página', () => {
    const fixture = TestBed.createComponent(ClientTable);
    fixture.componentRef.setInput('clients', mockClients);
    fixture.componentRef.setInput('total', 25);
    fixture.componentRef.setInput('currentPage', 1);
    fixture.detectChanges();

    let emittedPage: number | undefined;
    fixture.componentInstance.pageChange.subscribe((p: number) => emittedPage = p);

    const nextBtn = fixture.nativeElement.querySelector('.pagination__page-btn--next');
    nextBtn.click();
    expect(emittedPage).toBe(2);
  });
});
