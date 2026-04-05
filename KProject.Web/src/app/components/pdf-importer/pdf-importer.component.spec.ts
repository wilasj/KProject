import {ComponentFixture, TestBed} from '@angular/core/testing';
import {of, Subject} from 'rxjs';
import {PdfImporterComponent} from './pdf-importer.component';
import {ImportService} from '@core/import.service';
import {ImportTask} from '@models/import';

const mockTask: ImportTask = {
  id: 'task-1',
  fileName: 'test.pdf',
  status: 'pending',
  createdAt: '2026-04-05T10:00:00Z',
};

describe('PdfImporterComponent', () => {
  let component: PdfImporterComponent;
  let fixture: ComponentFixture<PdfImporterComponent>;
  let sseSubject: Subject<Partial<ImportTask>>;

  const mockService = {
    getTasks: vi.fn(),
    uploadFiles: vi.fn(),
    connectSse: vi.fn(),
    disconnectSse: vi.fn(),
  };

  beforeEach(() => {
    sseSubject = new Subject();
    mockService.getTasks.mockReset().mockReturnValue(of([]));
    mockService.uploadFiles.mockReset().mockReturnValue(of([]));
    mockService.connectSse.mockReset().mockReturnValue(sseSubject.asObservable());
    mockService.disconnectSse.mockReset();
  });

  beforeEach(() => TestBed.resetTestingModule());

  async function setup() {
    await TestBed.configureTestingModule({
      imports: [PdfImporterComponent],
      providers: [{provide: ImportService, useValue: mockService}],
    }).compileComponents();

    fixture = TestBed.createComponent(PdfImporterComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  }

  it('deve criar o componente', async () => {
    await setup();
    expect(component).toBeTruthy();
  });

  it('deve emitir close ao clicar no backdrop', async () => {
    await setup();
    const spy = vi.spyOn(component.close, 'emit');
    fixture.nativeElement.querySelector('.pdf-importer__backdrop').click();
    expect(spy).toHaveBeenCalledTimes(1);
  });

  it('deve emitir close ao clicar no botão de fechar', async () => {
    await setup();
    const spy = vi.spyOn(component.close, 'emit');
    fixture.nativeElement.querySelector('.pdf-importer__close').click();
    expect(spy).toHaveBeenCalledTimes(1);
  });

  it('deve carregar tasks ao inicializar', async () => {
    mockService.getTasks.mockReturnValue(of([mockTask]));
    await setup();
    expect(component.tasks()).toEqual([mockTask]);
  });

  it('deve conectar SSE ao inicializar', async () => {
    await setup();
    expect(mockService.connectSse).toHaveBeenCalledTimes(1);
  });

  it('deve desconectar SSE ao destruir', async () => {
    await setup();
    fixture.destroy();
    expect(mockService.disconnectSse).toHaveBeenCalledTimes(1);
  });

  it('deve atualizar task existente ao receber evento SSE', async () => {
    mockService.getTasks.mockReturnValue(of([mockTask]));
    await setup();

    sseSubject.next({id: 'task-1', status: 'processing'});
    fixture.detectChanges();

    expect(component.tasks()[0].status).toBe('processing');
  });

  it('deve re-fetch a lista apos upload com sucesso', async () => {
    const newTask: ImportTask = {...mockTask, id: 'task-2', fileName: 'new.pdf'};
    mockService.uploadFiles.mockReturnValue(of([newTask]));
    mockService.getTasks.mockReturnValueOnce(of([])).mockReturnValueOnce(of([mockTask, newTask]));
    await setup();

    const file = new File(['content'], 'new.pdf', {type: 'application/pdf'});
    component['uploadFiles']([file]);
    fixture.detectChanges();

    expect(mockService.getTasks).toHaveBeenCalledTimes(2);
    expect(component.tasks()).toContainEqual(newTask);
  });

  it('deve exibir erro ao tentar upload de mais de 10 arquivos', async () => {
    await setup();

    const files = Array.from({length: 11}, (_, i) =>
      new File(['x'], `file${i}.pdf`, {type: 'application/pdf'})
    );
    component['uploadFiles'](files);
    fixture.detectChanges();

    expect(component.fileError()).toContain('10');
    expect(mockService.uploadFiles).not.toHaveBeenCalled();
  });

  it('deve ignorar arquivos com nome ja existente na lista', async () => {
    mockService.getTasks.mockReturnValue(of([mockTask]));
    await setup();

    const duplicate = new File(['x'], 'test.pdf', {type: 'application/pdf'});
    component['uploadFiles']([duplicate]);
    fixture.detectChanges();

    expect(component.skippedFiles()).toContain('test.pdf');
    expect(mockService.uploadFiles).not.toHaveBeenCalled();
  });

  it('deve enviar apenas arquivos novos quando ha duplicatas misturadas', async () => {
    const newTask: ImportTask = {...mockTask, id: 'task-2', fileName: 'new.pdf'};
    mockService.getTasks
      .mockReturnValueOnce(of([mockTask]))
      .mockReturnValueOnce(of([mockTask, newTask]));
    mockService.uploadFiles.mockReturnValue(of([newTask]));
    await setup();

    const duplicate = new File(['x'], 'test.pdf', {type: 'application/pdf'});
    const newFile  = new File(['y'], 'new.pdf',  {type: 'application/pdf'});
    component['uploadFiles']([duplicate, newFile]);
    fixture.detectChanges();

    expect(component.skippedFiles()).toContain('test.pdf');
    const uploaded = mockService.uploadFiles.mock.calls[0][0] as File[];
    expect(uploaded.map((f: File) => f.name)).toEqual(['new.pdf']);
  });
});
