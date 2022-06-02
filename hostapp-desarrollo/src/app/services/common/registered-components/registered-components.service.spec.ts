/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { RegisteredComponentsService } from './registered-components.service';

describe('Service: RegisteredComponents', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RegisteredComponentsService]
    });
  });

  it('should ...', inject([RegisteredComponentsService], (service: RegisteredComponentsService) => {
    expect(service).toBeTruthy();
  }));
});
