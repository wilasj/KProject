import { Component, effect, input, output } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-bar',
  imports: [ReactiveFormsModule],
  templateUrl: './search-bar.html',
  styleUrl: './search-bar.scss',
})
export class SearchBar {
  placeholder = input<string>('Buscar...');
  initialValue = input<string>('');

  search = output<string>();

  protected control = new FormControl('', { nonNullable: true });

  constructor() {
    effect(() => {
      this.control.setValue(this.initialValue(), { emitEvent: false });
    });
  }

  onKeydown(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      this.search.emit(this.control.value);
    }
  }
}
