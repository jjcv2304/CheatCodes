import { TestBed } from '@angular/core/testing';

import { LogedInGuard } from './logged-in.guard';

describe('LogedInGuard', () => {
  let guard: LogedInGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(LogedInGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
