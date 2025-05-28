import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginComponent } from './login.component';
import { of, throwError } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../services/auth.service';

describe('LoginComponent (custom)', () => {
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let activatedRouteStub: any;

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['handleCallback', 'googleLogin']);
  });

  it('should call handleCallback with token from queryParams', () => {
    activatedRouteStub = {
      queryParams: of({ token: 'abc123' })
    };
    const component = new LoginComponent(authServiceSpy, activatedRouteStub as ActivatedRoute);
    expect(authServiceSpy.handleCallback).toHaveBeenCalledWith('abc123');
  });

  it('should not call handleCallback if token is not present in queryParams', () => {
    activatedRouteStub = {
      queryParams: of({ foo: 'bar' })
    };
    const component = new LoginComponent(authServiceSpy, activatedRouteStub as ActivatedRoute);
    expect(authServiceSpy.handleCallback).not.toHaveBeenCalled();
  });

  it('should call googleLogin when loginWithGoogle is called', () => {
    activatedRouteStub = {
      queryParams: of({})
    };
    const component = new LoginComponent(authServiceSpy, activatedRouteStub as ActivatedRoute);
    component.loginWithGoogle();
    expect(authServiceSpy.googleLogin).toHaveBeenCalled();
  });

  it('should log error if queryParams observable errors', () => {
    activatedRouteStub = {
      queryParams: throwError(() => new Error('test error'))
    };
    spyOn(console, 'error');
    new LoginComponent(authServiceSpy, activatedRouteStub as ActivatedRoute);
    expect(console.error).toHaveBeenCalledWith(
      'Error occurred while processing query params:',
      jasmine.any(Error)
    );
  });

  it('should log complete when queryParams observable completes', () => {
    let observer: any;
    activatedRouteStub = {
      queryParams: new Observable(obs => {
        observer = obs;
      })
    };
    spyOn(console, 'log');
    new LoginComponent(authServiceSpy, activatedRouteStub as ActivatedRoute);
    observer.complete();
    expect(console.log).toHaveBeenCalledWith('Query params processing completed.');
  });
});
