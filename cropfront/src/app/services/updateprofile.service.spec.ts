import { TestBed } from '@angular/core/testing';

import { UpdateprofileService } from './updateprofile.service';

describe('UpdateprofileService', () => {
  let service: UpdateprofileService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UpdateprofileService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
