import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { describe, it, expect, beforeEach } from 'vitest';
import { DataTable, DataTableRowDef } from './data-table';

interface MockItem {
  id: number;
  name: string;
}

@Component({
  imports: [DataTable, DataTableRowDef],
  template: `
    <app-data-table
      [items]="items"
      [total]="total"
      [currentPage]="1"
      [emptyIcon]="'group'"
      [emptyMessage]="'Nenhum encontrado'"
      (pageChange)="lastPage = $event"
    >
      <ng-template dataTableRow let-item>
        <div class="mock-row">{{ item.name }}</div>
      </ng-template>
    </app-data-table>
  `,
})
class TestHost {
  items: MockItem[] = [];
  total = 0;
  lastPage?: number;
}

describe('DataTable', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [TestHost] });
  });

  function setup(items: MockItem[] = [], total = items.length) {
    const fixture = TestBed.createComponent(TestHost);
    fixture.componentInstance.items = items;
    fixture.componentInstance.total = total;
    fixture.detectChanges();
    return fixture;
  }

  it('deve exibir empty state quando não há items', () => {
    const fixture = setup();
    const empty = fixture.nativeElement.querySelector('.data-table__empty');
    expect(empty).not.toBeNull();
    expect(empty.textContent).toContain('Nenhum encontrado');
  });

  it('deve exibir o ícone correto no empty state', () => {
    const fixture = setup();
    const icon = fixture.nativeElement.querySelector('.data-table__empty .material-symbols-outlined');
    expect(icon.textContent.trim()).toBe('group');
  });

  it('deve renderizar os rows via template', () => {
    const fixture = setup([
      { id: 1, name: 'Item A' },
      { id: 2, name: 'Item B' },
    ]);
    const rows = fixture.nativeElement.querySelectorAll('.mock-row');
    expect(rows.length).toBe(2);
    expect(rows[0].textContent).toContain('Item A');
    expect(rows[1].textContent).toContain('Item B');
  });

  it('deve emitir pageChange via pagination', () => {
    const fixture = setup(
      [{ id: 1, name: 'A' }, { id: 2, name: 'B' }],
      25,
    );
    const nextBtn = fixture.nativeElement.querySelector('.pagination__page-btn--next');
    nextBtn.click();
    expect(fixture.componentInstance.lastPage).toBe(2);
  });

  it('não deve renderizar pagination quando há empty state', () => {
    const fixture = setup();
    const pagination = fixture.nativeElement.querySelector('app-pagination');
    expect(pagination).toBeNull();
  });
});
