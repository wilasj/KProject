import {Component, ElementRef, OnDestroy, OnInit, inject, output, signal, viewChild} from '@angular/core';
import {ImportService} from '@core/import.service';
import {ImportTask} from '@models/import';
import {ImportTaskListComponent} from './import-task-list/import-task-list.component';

const MAX_FILES = 10;

@Component({
  selector: 'app-pdf-importer',
  imports: [ImportTaskListComponent],
  templateUrl: './pdf-importer.component.html',
  styleUrl: './pdf-importer.component.scss',
})
export class PdfImporterComponent implements OnInit, OnDestroy {
  private service = inject(ImportService);

  close = output<void>();

  tasks = signal<ImportTask[]>([]);
  dragOver = signal(false);
  fileError = signal<string | null>(null);
  skippedFiles = signal<string[]>([]);
  uploading = signal(false);

  private fileInput = viewChild.required<ElementRef<HTMLInputElement>>('fileInput');

  ngOnInit(): void {
    this.service.getTasks().subscribe(tasks => this.tasks.set(tasks));
    this.service.connectSse().subscribe(update => {
      this.tasks.update(list =>
        list.map(t => t.id === update.id ? {...t, ...update} : t)
      );
    });
  }

  ngOnDestroy(): void {
    this.service.disconnectSse();
  }

  openFilePicker(): void {
    this.fileInput().nativeElement.click();
  }

  onFilesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.uploadFiles(Array.from(input.files));
      input.value = '';
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.dragOver.set(true);
  }

  onDragLeave(): void {
    this.dragOver.set(false);
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    this.dragOver.set(false);
    const files = Array.from(event.dataTransfer?.files ?? []).filter(f => f.type === 'application/pdf');
    this.uploadFiles(files);
  }

  onTaskClick(task: ImportTask): void {
    // review panel wired in T7
    void task;
  }

  private uploadFiles(files: File[]): void {
    this.fileError.set(null);
    this.skippedFiles.set([]);

    if (files.length === 0) {
      return;
    }

    const existingNames = new Set(this.tasks().map(t => t.fileName));
    const skipped = files.filter(f => existingNames.has(f.name)).map(f => f.name);
    const toUpload = files.filter(f => !existingNames.has(f.name));

    if (skipped.length > 0) {
      this.skippedFiles.set(skipped);
    }

    if (toUpload.length === 0) {
      return;
    }

    if (toUpload.length > MAX_FILES) {
      this.fileError.set(`Máximo de ${MAX_FILES} arquivos por vez.`);
      return;
    }

    this.uploading.set(true);
    this.service.uploadFiles(toUpload).subscribe({
      next: () => {
        this.service.getTasks().subscribe(tasks => this.tasks.set(tasks));
        this.uploading.set(false);
      },
      error: () => {
        this.fileError.set('Erro ao enviar arquivos.');
        this.uploading.set(false);
      },
    });
  }
}
