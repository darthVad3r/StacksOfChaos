import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AuthGuard } from './auth.guard';

describe('AuthGuard', () => {
  let guard: AuthGuard;
  let routerMock: jasmine.SpyObj<Router>;

  beforeEach(() => {
    // Create a spy object for Router
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        AuthGuard,
        { provide: Router, useValue: routerMock }
      ]
    });

    guard = TestBed.inject(AuthGuard);

    // Clear localStorage before each test
    localStorage.clear();
  });

  afterEach(() => {
    // Clean up localStorage after each test
    localStorage.clear();
  });

  // Guard Instantiation Tests
  describe('Guard Creation', () => {
    it('should be created', () => {
      expect(guard).toBeTruthy();
    });

    it('should be an instance of AuthGuard', () => {
      expect(guard instanceof AuthGuard).toBe(true);
    });

    it('should implement CanActivate interface', () => {
      expect(guard.canActivate).toBeDefined();
    });

    it('should have router injected', () => {
      expect((guard as any).router).toBeDefined();
    });
  });

  // Token Presence Tests
  describe('Token Validation', () => {
    it('should check for jwtToken in localStorage', () => {
      spyOn(window.localStorage, 'getItem').and.returnValue('valid-token');
      spyOn(console, 'log');

      guard.canActivate();

      expect(window.localStorage.getItem).toHaveBeenCalledWith('jwtToken');
    });

    it('should return true when token exists', () => {
      localStorage.setItem('jwtToken', 'valid-token-123');

      const result = guard.canActivate();

      expect(result).toBe(true);
    });

    it('should return false when token does not exist', () => {
      localStorage.removeItem('jwtToken');

      const result = guard.canActivate();

      expect(result).toBe(false);
    });

    it('should return false when token is null', () => {
      localStorage.removeItem('jwtToken');

      const result = guard.canActivate();

      expect(result).toBe(false);
    });

    it('should return false when token is empty string', () => {
      localStorage.setItem('jwtToken', '');

      const result = guard.canActivate();

      expect(result).toBe(false);
    });
  });

  // Authentication Success Tests
  describe('Authenticated User', () => {
    it('should log success message when authenticated', () => {
      localStorage.setItem('jwtToken', 'test-token');
      spyOn(console, 'log');

      guard.canActivate();

      expect(console.log).toHaveBeenCalledWith('User is authenticated');
    });

    it('should not navigate when authenticated', () => {
      localStorage.setItem('jwtToken', 'test-token');

      guard.canActivate();

      expect(routerMock.navigate).not.toHaveBeenCalled();
    });

    it('should allow route activation with valid token', () => {
      localStorage.setItem('jwtToken', 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...');

      const result = guard.canActivate();

      expect(result).toBe(true);
    });

    it('should handle various token formats', () => {
      const tokens = [
        'simple-token',
        'token-with-dashes-123',
        'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIn0.dozjgNryP4J3jVmNHl0w5N_XgL0n3I9PlFUP0THsR8U',
        'x'.repeat(1000)
      ];

      tokens.forEach(token => {
        localStorage.setItem('jwtToken', token);
        const result = guard.canActivate();
        expect(result).toBe(true);
      });
    });
  });

  // Authentication Failure Tests
  describe('Unauthenticated User', () => {
    it('should log token check message', () => {
      localStorage.removeItem('jwtToken');
      spyOn(console, 'log');

      guard.canActivate();

      expect(console.log).toHaveBeenCalledWith('AuthGuard checking token:', null);
    });

    it('should navigate to login when not authenticated', () => {
      localStorage.removeItem('jwtToken');

      guard.canActivate();

      expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
    });

    it('should log unauthenticated message when redirecting', () => {
      localStorage.removeItem('jwtToken');
      spyOn(console, 'log');

      guard.canActivate();

      expect(console.log).toHaveBeenCalledWith('User is not authenticated, redirecting to login');
    });

    it('should block route activation when not authenticated', () => {
      localStorage.removeItem('jwtToken');

      const result = guard.canActivate();

      expect(result).toBe(false);
    });

    it('should navigate exactly once per guard check', () => {
      localStorage.removeItem('jwtToken');

      guard.canActivate();

      expect(routerMock.navigate).toHaveBeenCalledTimes(1);
    });
  });

  // Console Logging Tests
  describe('Console Logging', () => {
    it('should log token value during check', () => {
      const token = 'test-token-xyz';
      localStorage.setItem('jwtToken', token);
      spyOn(console, 'log');

      guard.canActivate();

      expect(console.log).toHaveBeenCalledWith('AuthGuard checking token:', token);
    });

    it('should log correct messages in order', () => {
      localStorage.setItem('jwtToken', 'token');
      spyOn(console, 'log');

      guard.canActivate();

      const logCalls = (console.log as jasmine.Spy).calls.all();
      expect(logCalls[0].args[0]).toContain('AuthGuard checking token');
      expect(logCalls[1].args[0]).toBe('User is authenticated');
    });

    it('should log different messages for unauthenticated users', () => {
      localStorage.removeItem('jwtToken');
      spyOn(console, 'log');

      guard.canActivate();

      const logCalls = (console.log as jasmine.Spy).calls.all();
      expect(logCalls[0].args[0]).toContain('AuthGuard checking token');
      expect(logCalls[1].args[0]).toContain('not authenticated');
    });
  });

  // Multiple Sequential Checks Tests
  describe('Sequential Activation Checks', () => {
    it('should handle multiple sequential authentications', () => {
      localStorage.setItem('jwtToken', 'token-1');
      const result1 = guard.canActivate();
      expect(result1).toBe(true);

      localStorage.setItem('jwtToken', 'token-2');
      const result2 = guard.canActivate();
      expect(result2).toBe(true);
    });

    it('should handle transition from authenticated to unauthenticated', () => {
      localStorage.setItem('jwtToken', 'token');
      const result1 = guard.canActivate();
      expect(result1).toBe(true);

      localStorage.removeItem('jwtToken');
      const result2 = guard.canActivate();
      expect(result2).toBe(false);
      expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
    });

    it('should handle transition from unauthenticated to authenticated', () => {
      localStorage.removeItem('jwtToken');
      const result1 = guard.canActivate();
      expect(result1).toBe(false);

      localStorage.setItem('jwtToken', 'token');
      const result2 = guard.canActivate();
      expect(result2).toBe(true);
    });

    it('should maintain consistent behavior across multiple calls', () => {
      localStorage.setItem('jwtToken', 'stable-token');
      const results = [
        guard.canActivate(),
        guard.canActivate(),
        guard.canActivate()
      ];
      expect(results).toEqual([true, true, true]);
    });
  });

  // Edge Cases Tests
  describe('Edge Cases', () => {
    it('should handle whitespace-only token as invalid', () => {
      localStorage.setItem('jwtToken', '   ');
      spyOn(console, 'log');

      // Whitespace-only tokens should be treated as invalid for authentication
      const result = guard.canActivate();
      expect(result).toBe(false);
      expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
    });

    it('should handle special characters in token', () => {
      const specialToken = 'token!@#$%^&*()_+-=[]{}|;:,.<>?';
      localStorage.setItem('jwtToken', specialToken);

      const result = guard.canActivate();

      expect(result).toBe(true);
    });

    it('should handle very long tokens', () => {
      const longToken = 'x'.repeat(10000);
      localStorage.setItem('jwtToken', longToken);

      const result = guard.canActivate();

      expect(result).toBe(true);
    });

    it('should handle unicode characters in token', () => {
      const unicodeToken = 'token-日本語-español-العربية';
      localStorage.setItem('jwtToken', unicodeToken);

      const result = guard.canActivate();

      expect(result).toBe(true);
    });

    it('should handle case sensitivity for jwtToken key', () => {
      localStorage.setItem('jwtToken', 'test-token');
      const result = guard.canActivate();
      expect(result).toBe(true);

      // Setting with different case should be different key
      localStorage.clear();
      localStorage.setItem('jwttoken', 'test-token'); // lowercase
      const result2 = guard.canActivate();
      expect(result2).toBe(false);
    });
  });

  // Integration Tests
  describe('Router Integration', () => {
    it('should use Router service for navigation', () => {
      localStorage.removeItem('jwtToken');

      guard.canActivate();

      expect(routerMock.navigate).toHaveBeenCalled();
    });

    it('should navigate to /login route path', () => {
      localStorage.removeItem('jwtToken');

      guard.canActivate();

      expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
    });

    it('should not call router when token is valid', () => {
      localStorage.setItem('jwtToken', 'valid-token');

      guard.canActivate();

      expect(routerMock.navigate).not.toHaveBeenCalled();
    });

    it('should call router navigate with array argument', () => {
      localStorage.removeItem('jwtToken');

      guard.canActivate();

      const navigateCall = (routerMock.navigate as jasmine.Spy).calls.mostRecent();
      expect(Array.isArray(navigateCall.args[0])).toBe(true);
    });
  });

  // Return Value Tests
  describe('Return Values', () => {
    it('should return boolean value', () => {
      localStorage.setItem('jwtToken', 'token');
      const result = guard.canActivate();
      expect(typeof result).toBe('boolean');
    });

    it('should return true for authenticated users', () => {
      localStorage.setItem('jwtToken', 'token');
      const result = guard.canActivate();
      expect(result === true).toBe(true);
    });

    it('should return false for unauthenticated users', () => {
      localStorage.removeItem('jwtToken');
      const result = guard.canActivate();
      expect(result === false).toBe(true);
    });

    it('should never return null or undefined', () => {
      const results = [];
      
      localStorage.setItem('jwtToken', 'token');
      results.push(guard.canActivate());
      
      localStorage.removeItem('jwtToken');
      results.push(guard.canActivate());

      results.forEach(result => {
        expect(result).not.toBeNull();
        expect(result).not.toBeUndefined();
      });
    });
  });
});
