import { TestBed } from '@angular/core/testing';
import { CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

import { AuthGuard } from './auth.guard';

      TestBed.runInInjectionContext(() => {
        const authService = TestBed.inject(AuthService);
        const router = TestBed.inject(Router);
    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: jasmine.createSpyObj('AuthService', ['isAuthenticated']) },
        { provide: Router, useValue: jasmine.createSpyObj('Router', ['navigate']) }
      ]
        const guardParameters: [ActivatedRouteSnapshot, RouterStateSnapshot] = [new ActivatedRouteSnapshot(), {} as RouterStateSnapshot];
        return executeGuard(guardParameters[0] as ActivatedRouteSnapshot, guardParameters[1] as RouterStateSnapshot);
      });
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => {
        const authService = TestBed.inject(AuthService);
        const router = TestBed.inject(Router);
        return new AuthGuard(authService, router).canActivate();
      });

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
