import { TestBed } from '@angular/core/testing';

import { CroplistingsService } from './croplistings.service';

describe('CroplistingsService', () => {
  let service: CroplistingsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CroplistingsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
