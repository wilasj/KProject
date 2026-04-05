import {Component, input, output} from '@angular/core';
import {ImportTask} from '@models/import';
import {ImportTaskRowComponent} from './import-task-row/import-task-row.component';

@Component({
  selector: 'app-import-task-list',
  imports: [ImportTaskRowComponent],
  templateUrl: './import-task-list.component.html',
  styleUrl: './import-task-list.component.scss',
})
export class ImportTaskListComponent {
  tasks = input.required<ImportTask[]>();
  taskClick = output<ImportTask>();
}
