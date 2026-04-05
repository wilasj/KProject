import { Component, input, output } from '@angular/core';

interface ValidationError { code: string; description: string; }

@Component({
  selector: 'app-drawer',
  templateUrl: './drawer.html',
  styleUrl: './drawer.scss',
})
export class Drawer {
  title    = input.required<string>();
  subtitle = input<string>('');
  errors   = input<ValidationError[]>([]);

  close = output<void>();
}