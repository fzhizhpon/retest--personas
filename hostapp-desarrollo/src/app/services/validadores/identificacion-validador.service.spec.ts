import { TestBed } from '@angular/core/testing';

import { IdentificacionValidadorService } from './identificacion-validador.service';

describe('IdentificacionValidadorService', () => {
  let service: IdentificacionValidadorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(IdentificacionValidadorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
