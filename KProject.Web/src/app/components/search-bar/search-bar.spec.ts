import { TestBed } from '@angular/core/testing';
import { SearchBar } from './search-bar';
import { describe, it, expect, beforeEach } from 'vitest';

describe('SearchBar', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [SearchBar] });
  });

  it('deve emitir o evento search ao pressionar Enter', () => {
    const fixture = TestBed.createComponent(SearchBar);
    fixture.detectChanges();

    let emitted: string | undefined;
    fixture.componentInstance.search.subscribe((v: string) => emitted = v);

    const input = fixture.nativeElement.querySelector('input');
    input.value = 'paracetamol';
    input.dispatchEvent(new Event('input'));
    input.dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter' }));

    expect(emitted).toBe('paracetamol');
  });

  it('deve refletir o initialValue no input', () => {
    const fixture = TestBed.createComponent(SearchBar);
    fixture.componentRef.setInput('initialValue', 'ibuprofeno');
    fixture.detectChanges();

    const input = fixture.nativeElement.querySelector('input');
    expect(input.value).toBe('ibuprofeno');
  });
});
