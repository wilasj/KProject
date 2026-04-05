import {ComponentFixture, TestBed} from '@angular/core/testing';
import {By} from '@angular/platform-browser';
import {ImportTaskListComponent} from './import-task-list.component';
import {ImportTask} from '@models/import';

const tasks: ImportTask[] = [
  {id: 'task-1', fileName: 'a.pdf', status: 'done',   createdAt: '2026-04-01T10:00:00Z'},
  {id: 'task-2', fileName: 'b.pdf', status: 'review', createdAt: '2026-04-02T10:00:00Z'},
  {id: 'task-3', fileName: 'c.pdf', status: 'error',  createdAt: '2026-04-03T10:00:00Z', errorMessage: 'Erro'},
];

describe('ImportTaskListComponent', () => {
  let component: ImportTaskListComponent;
  let fixture: ComponentFixture<ImportTaskListComponent>;

  beforeEach(() => TestBed.resetTestingModule());

  async function setup(taskList: ImportTask[]) {
    await TestBed.configureTestingModule({
      imports: [ImportTaskListComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ImportTaskListComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('tasks', taskList);
    fixture.detectChanges();
  }

  it('deve criar o componente', async () => {
    await setup(tasks);
    expect(component).toBeTruthy();
  });

  it('deve renderizar uma row por task', async () => {
    await setup(tasks);
    const rows = fixture.debugElement.queryAll(By.css('app-import-task-row'));
    expect(rows).toHaveLength(3);
  });

  it('deve exibir mensagem vazia quando nao ha tasks', async () => {
    await setup([]);
    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('.import-task-list__empty')).toBeTruthy();
  });

  it('deve emitir taskClick ao receber rowClick de uma task review', async () => {
    await setup(tasks);
    const spy = vi.spyOn(component.taskClick, 'emit');

    const rows = fixture.debugElement.queryAll(By.css('app-import-task-row'));
    const reviewRow = rows[1].componentInstance;
    reviewRow.rowClick.emit(tasks[1]);

    expect(spy).toHaveBeenCalledWith(tasks[1]);
  });
});
