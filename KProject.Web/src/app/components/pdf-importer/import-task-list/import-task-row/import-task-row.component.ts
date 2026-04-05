import {Component, computed, input, output} from '@angular/core';
import {ImportTask} from '@models/import';

@Component({
  selector: 'app-import-task-row',
  templateUrl: './import-task-row.component.html',
  styleUrl: './import-task-row.component.scss',
})
export class ImportTaskRowComponent {
  task = input.required<ImportTask>();
  rowClick = output<ImportTask>();

  readonly statusIcon = computed(() => {
    const icons: Record<ImportTask['status'], string> = {
      pending:    'schedule',
      processing: 'refresh',
      review:     'rate_review',
      done:       'check_circle',
      error:      'error',
    };
    return icons[this.task().status];
  });

  readonly isClickable = computed(() => this.task().status === 'review');

  readonly formattedDate = computed(() => {
    return new Intl.DateTimeFormat('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }).format(new Date(this.task().createdAt));
  });

  onClick(): void {
    if (this.isClickable()) {
      this.rowClick.emit(this.task());
    }
  }
}
