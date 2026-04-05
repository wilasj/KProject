import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ImportTaskRowComponent} from './import-task-row.component';
import {ImportTask} from '@models/import';

const baseTask: ImportTask = {
  id: 'task-1',
  fileName: 'test.pdf',
  status: 'pending',
  createdAt: '2026-04-05T10:00:00Z',
};

describe('ImportTaskRowComponent', () => {
  let component: ImportTaskRowComponent;
  let fixture: ComponentFixture<ImportTaskRowComponent>;

  beforeEach(() => TestBed.resetTestingModule());

  async function setup(task: ImportTask) {
    await TestBed.configureTestingModule({
      imports: [ImportTaskRowComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ImportTaskRowComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('task', task);
    fixture.detectChanges();
  }

  it('deve criar o componente', async () => {
    await setup(baseTask);
    expect(component).toBeTruthy();
  });

  it('deve exibir o nome do arquivo', async () => {
    await setup(baseTask);
    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('.import-task-row__filename')?.textContent?.trim()).toBe('test.pdf');
  });

  it('deve usar icone schedule para status pending', async () => {
    await setup(baseTask);
    expect(component.statusIcon()).toBe('schedule');
  });

  it('deve usar icone refresh para status processing', async () => {
    await setup({...baseTask, status: 'processing'});
    expect(component.statusIcon()).toBe('refresh');
  });

  it('deve usar icone rate_review para status review', async () => {
    await setup({...baseTask, status: 'review'});
    expect(component.statusIcon()).toBe('rate_review');
  });

  it('deve usar icone check_circle para status done', async () => {
    await setup({...baseTask, status: 'done'});
    expect(component.statusIcon()).toBe('check_circle');
  });

  it('deve usar icone error para status error', async () => {
    await setup({...baseTask, status: 'error'});
    expect(component.statusIcon()).toBe('error');
  });

  it('nao deve ser clicavel quando status nao e review', async () => {
    await setup(baseTask);
    expect(component.isClickable()).toBe(false);
  });

  it('deve ser clicavel quando status e review', async () => {
    await setup({...baseTask, status: 'review'});
    expect(component.isClickable()).toBe(true);
  });

  it('deve emitir rowClick ao clicar em row com status review', async () => {
    const task = {...baseTask, status: 'review'} as ImportTask;
    await setup(task);
    const spy = vi.spyOn(component.rowClick, 'emit');

    fixture.nativeElement.querySelector('.import-task-row').click();

    expect(spy).toHaveBeenCalledWith(task);
  });

  it('nao deve emitir rowClick ao clicar em row com status pending', async () => {
    await setup(baseTask);
    const spy = vi.spyOn(component.rowClick, 'emit');

    fixture.nativeElement.querySelector('.import-task-row').click();

    expect(spy).not.toHaveBeenCalled();
  });

  it('deve exibir errorMessage quando status e error', async () => {
    await setup({...baseTask, status: 'error', errorMessage: 'Falha ao processar'});
    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('.import-task-row__error')?.textContent?.trim()).toBe('Falha ao processar');
  });
});
