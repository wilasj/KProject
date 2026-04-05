import {TestBed} from '@angular/core/testing';
import {HttpTestingController, provideHttpClientTesting} from '@angular/common/http/testing';
import {provideHttpClient} from '@angular/common/http';
import {ImportService} from './import.service';
import {ImportTask} from '@models/import';

const mockTask: ImportTask = {
  id: 'task-1',
  fileName: 'test.pdf',
  status: 'pending',
  createdAt: '2026-04-05T10:00:00Z',
};

describe('ImportService', () => {
  let service: ImportService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ImportService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  describe('getTasks', () => {
    it('deve fazer GET /api/imports e retornar lista de tasks', () => {
      let result: ImportTask[] | undefined;
      service.getTasks().subscribe(r => (result = r));

      const req = httpMock.expectOne('/api/imports');
      expect(req.request.method).toBe('GET');
      req.flush([mockTask]);

      expect(result).toEqual([mockTask]);
    });
  });

  describe('getTask', () => {
    it('deve fazer GET /api/imports/:id', () => {
      let result: ImportTask | undefined;
      service.getTask('task-1').subscribe(r => (result = r));

      const req = httpMock.expectOne('/api/imports/task-1');
      expect(req.request.method).toBe('GET');
      req.flush(mockTask);

      expect(result).toEqual(mockTask);
    });
  });

  describe('uploadFiles', () => {
    it('deve fazer POST /api/imports com FormData contendo os arquivos', () => {
      const file = new File(['content'], 'test.pdf', {type: 'application/pdf'});
      let result: ImportTask[] | undefined;
      service.uploadFiles([file]).subscribe(r => (result = r));

      const req = httpMock.expectOne('/api/imports');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toBeInstanceOf(FormData);
      expect((req.request.body as FormData).getAll('files')).toHaveLength(1);
      req.flush([mockTask]);

      expect(result).toEqual([mockTask]);
    });

    it('deve incluir todos os arquivos no FormData', () => {
      const files = [
        new File(['a'], 'a.pdf', {type: 'application/pdf'}),
        new File(['b'], 'b.pdf', {type: 'application/pdf'}),
      ];
      service.uploadFiles(files).subscribe();

      const req = httpMock.expectOne('/api/imports');
      expect((req.request.body as FormData).getAll('files')).toHaveLength(2);
      req.flush([]);
    });
  });

  describe('confirmTask', () => {
    it('deve fazer POST /api/imports/:id/confirm', () => {
      service.confirmTask('task-3').subscribe();

      const req = httpMock.expectOne('/api/imports/task-3/confirm');
      expect(req.request.method).toBe('POST');
      req.flush(null, {status: 204, statusText: 'No Content'});
    });
  });

  describe('deleteTask', () => {
    it('deve fazer DELETE /api/imports/:id', () => {
      service.deleteTask('task-2').subscribe();

      const req = httpMock.expectOne('/api/imports/task-2');
      expect(req.request.method).toBe('DELETE');
      req.flush(null, {status: 204, statusText: 'No Content'});
    });
  });

  describe('connectSse', () => {
    it('deve retornar um Observable (modo dev usa mockSseSubject)', () => {
      const obs = service.connectSse();
      expect(obs).toBeDefined();
      service.disconnectSse();
    });
  });
});
