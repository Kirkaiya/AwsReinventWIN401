import { TestBed } from '@angular/core/testing';

import { IdentityService } from './identity.service';

describe('IdentityService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: IdentityService = TestBed.get(IdentityService);
    expect(service).toBeTruthy();
  });
});
