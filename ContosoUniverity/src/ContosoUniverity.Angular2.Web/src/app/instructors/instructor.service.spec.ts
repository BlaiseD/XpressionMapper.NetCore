/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { InstructorService } from './instructor.service';

describe('InstructorService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [InstructorService]
    });
  });

  it('should ...', inject([InstructorService], (service: InstructorService) => {
    expect(service).toBeTruthy();
  }));
});
