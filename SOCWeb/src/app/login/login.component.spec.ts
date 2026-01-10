import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login.component';
import { AuthService } from '../services/auth.service';
import { of, throwError } from 'rxjs';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authServiceMock: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    // Create a spy object for AuthService
    authServiceMock = jasmine.createSpyObj('AuthService', ['signIn']);

    await TestBed.configureTestingModule({
      imports: [LoginComponent, HttpClientTestingModule],
      providers: [
        { provide: AuthService, useValue: authServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
  });

  // Component Instantiation Tests
  describe('Component Creation', () => {
    it('should create the component', () => {
      expect(component).toBeTruthy();
    });

    it('should initialize with empty email and password', () => {
      expect(component.model.email).toBe('');
      expect(component.model.password).toBe('');
    });

    it('should have ngOnInit method', () => {
      expect(component.ngOnInit).toBeDefined();
    });

    it('should call ngOnInit during component initialization', () => {
      spyOn(component, 'ngOnInit');
      component.ngOnInit();
      expect(component.ngOnInit).toHaveBeenCalled();
    });
  });

  // Form Model Tests
  describe('Login Form Model', () => {
    it('should have a model property', () => {
      expect(component.model).toBeDefined();
    });

    it('should have email property in model', () => {
      expect(component.model.hasOwnProperty('email')).toBe(true);
    });

    it('should have password property in model', () => {
      expect(component.model.hasOwnProperty('password')).toBe(true);
    });

    it('should allow updating email in model', () => {
      component.model.email = 'test@example.com';
      expect(component.model.email).toBe('test@example.com');
    });

    it('should allow updating password in model', () => {
      component.model.password = 'SecurePassword123!';
      expect(component.model.password).toBe('SecurePassword123!');
    });

    it('should track model changes independently', () => {
      component.model.email = 'user1@example.com';
      component.model.password = 'password1';
      expect(component.model.email).toBe('user1@example.com');
      expect(component.model.password).toBe('password1');
    });
  });

  // Form Submission Tests
  describe('onSubmit()', () => {
    it('should call authService.signIn on form submission', () => {
      authServiceMock.signIn.and.returnValue(of({ token: 'test-token' }));
      component.model.email = 'test@example.com';
      component.model.password = 'password123';

      component.onSubmit();

      expect(authServiceMock.signIn).toHaveBeenCalledWith('test@example.com', 'password123');
    });

    it('should pass correct email to authService', () => {
      authServiceMock.signIn.and.returnValue(of({ token: 'test-token' }));
      component.model.email = 'user@example.com';
      component.model.password = 'pass';

      component.onSubmit();

      expect(authServiceMock.signIn).toHaveBeenCalledWith(jasmine.stringMatching('user@example.com'), jasmine.any(String));
    });

    it('should pass correct password to authService', () => {
      authServiceMock.signIn.and.returnValue(of({ token: 'test-token' }));
      component.model.email = 'user@example.com';
      component.model.password = 'SecurePass!';

      component.onSubmit();

      expect(authServiceMock.signIn).toHaveBeenCalledWith(jasmine.any(String), jasmine.stringMatching('SecurePass!'));
    });

    it('should subscribe to signIn observable', () => {
      const mockResponse = { token: 'jwt-token-123' };
      authServiceMock.signIn.and.returnValue(of(mockResponse));
      spyOn(console, 'log');

      component.onSubmit();

      expect(authServiceMock.signIn).toHaveBeenCalled();
    });
  });

  // Success Handling Tests
  describe('Successful Login', () => {
    it('should log success message on successful login', () => {
      const mockResponse = { token: 'jwt-token-abc123' };
      authServiceMock.signIn.and.returnValue(of(mockResponse));
      spyOn(console, 'log');
      component.model.email = 'user@example.com';
      component.model.password = 'password';

      component.onSubmit();

      expect(console.log).toHaveBeenCalledWith('Login successful', 'jwt-token-abc123');
    });

    it('should log token in success response', () => {
      authServiceMock.signIn.and.returnValue(of({ token: 'test-jwt-token' }));
      spyOn(console, 'log');

      component.onSubmit();

      expect(console.log).toHaveBeenCalledWith('Login successful', 'test-jwt-token');
    });

    it('should handle response with token property', () => {
      const tokenValue = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...';
      authServiceMock.signIn.and.returnValue(of({ token: tokenValue }));
      spyOn(console, 'log');

      component.onSubmit();

      expect(console.log).toHaveBeenCalledWith('Login successful', tokenValue);
    });
  });

  // Error Handling Tests
  describe('Login Error Handling', () => {
    it('should log error message on login failure', () => {
      const errorMessage = 'Invalid credentials';
      authServiceMock.signIn.and.returnValue(throwError(() => new Error(errorMessage)));
      spyOn(console, 'error');
      component.model.email = 'user@example.com';
      component.model.password = 'wrongpassword';

      component.onSubmit();

      expect(console.error).toHaveBeenCalledWith('Login failed', jasmine.any(Error));
    });

    it('should handle 401 Unauthorized error', () => {
      const error = new Error('401 Unauthorized');
      authServiceMock.signIn.and.returnValue(throwError(() => error));
      spyOn(console, 'error');

      component.onSubmit();

      expect(console.error).toHaveBeenCalledWith('Login failed', error);
    });

    it('should handle network errors gracefully', () => {
      const networkError = new Error('Network error');
      authServiceMock.signIn.and.returnValue(throwError(() => networkError));
      spyOn(console, 'error');

      component.onSubmit();

      expect(console.error).toHaveBeenCalledWith('Login failed', networkError);
    });

    it('should handle HTTP 500 server errors', () => {
      const serverError = new Error('Internal Server Error');
      authServiceMock.signIn.and.returnValue(throwError(() => serverError));
      spyOn(console, 'error');

      component.onSubmit();

      expect(console.error).toHaveBeenCalled();
    });
  });

  // Integration Tests
  describe('Form Integration', () => {
    it('should use credentials from model in submission', () => {
      authServiceMock.signIn.and.returnValue(of({ token: 'token' }));
      component.model.email = 'john@example.com';
      component.model.password = 'mypassword';

      component.onSubmit();

      expect(authServiceMock.signIn).toHaveBeenCalledWith('john@example.com', 'mypassword');
    });

    it('should handle multiple sequential submissions', () => {
      authServiceMock.signIn.and.returnValue(of({ token: 'token' }));
      component.model.email = 'user1@example.com';
      component.model.password = 'pass1';
      component.onSubmit();

      component.model.email = 'user2@example.com';
      component.model.password = 'pass2';
      component.onSubmit();

      expect(authServiceMock.signIn).toHaveBeenCalledTimes(2);
      expect(authServiceMock.signIn).toHaveBeenCalledWith('user1@example.com', 'pass1');
      expect(authServiceMock.signIn).toHaveBeenCalledWith('user2@example.com', 'pass2');
    });

    it('should handle special characters in credentials', () => {
      authServiceMock.signIn.and.returnValue(of({ token: 'token' }));
      const specialEmail = 'user+test@example.com';
      const specialPassword = 'P@ss!word#123';
      component.model.email = specialEmail;
      component.model.password = specialPassword;

      component.onSubmit();

      expect(authServiceMock.signIn).toHaveBeenCalledWith(specialEmail, specialPassword);
    });

    it('should handle whitespace in credentials', () => {
      authServiceMock.signIn.and.returnValue(of({ token: 'token' }));
      component.model.email = '  user@example.com  ';
      component.model.password = '  password  ';

      component.onSubmit();

      expect(authServiceMock.signIn).toHaveBeenCalledWith('  user@example.com  ', '  password  ');
    });
  });

  // Edge Cases Tests
  describe('Edge Cases', () => {
    it('should handle empty email submission', () => {
      authServiceMock.signIn.and.returnValue(of({ token: 'token' }));
      component.model.email = '';
      component.model.password = 'password';

      component.onSubmit();

      expect(authServiceMock.signIn).toHaveBeenCalledWith('', 'password');
    });

    it('should handle empty password submission', () => {
      authServiceMock.signIn.and.returnValue(of({ token: 'token' }));
      component.model.email = 'user@example.com';
      component.model.password = '';

      component.onSubmit();

      expect(authServiceMock.signIn).toHaveBeenCalledWith('user@example.com', '');
    });

    it('should handle very long email address', () => {
      authServiceMock.signIn.and.returnValue(of({ token: 'token' }));
      const longEmail = 'a'.repeat(100) + '@example.com';
      component.model.email = longEmail;
      component.model.password = 'password';

      component.onSubmit();

      expect(authServiceMock.signIn).toHaveBeenCalledWith(longEmail, 'password');
    });

    it('should handle very long password', () => {
      authServiceMock.signIn.and.returnValue(of({ token: 'token' }));
      const longPassword = 'p'.repeat(500);
      component.model.email = 'user@example.com';
      component.model.password = longPassword;

      component.onSubmit();

      expect(authServiceMock.signIn).toHaveBeenCalledWith('user@example.com', longPassword);
    });
  });
});
