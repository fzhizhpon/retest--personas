import { TestBed } from '@angular/core/testing';

import { EdadService } from './edad.service';

describe('EdadService', () => {
  let service: EdadService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EdadService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
