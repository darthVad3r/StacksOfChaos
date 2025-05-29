// create a unit test for the google-callback component
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { GoogleCallbackComponent } from './google-callback.component';
import { AuthService } from '../services/auth.service';
import { WindowLocationService } from '../services/window-location.service';

describe('GoogleCallbackComponent', () => {
    let component: GoogleCallbackComponent;
    let fixture: ComponentFixture<GoogleCallbackComponent>;
    let authService: jasmine.SpyObj<AuthService>;
    let router: jasmine.SpyObj<Router>;
    let windowLocationService: jasmine.SpyObj<WindowLocationService>;
    
    beforeEach(async () => {
        const authServiceSpy = jasmine.createSpyObj('AuthService', ['handleCallback']);
        const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    
        await TestBed.configureTestingModule({
        declarations: [GoogleCallbackComponent],
        providers: [
            { provide: AuthService, useValue: authServiceSpy },
            { provide: Router, useValue: routerSpy },
            { provide: WindowLocationService, useValue: jasmine.createSpyObj('WindowLocationService', ['getLocation']) }
        ]
        }).compileComponents();
    
        fixture = TestBed.createComponent(GoogleCallbackComponent);
        component = fixture.componentInstance;
        authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
        router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
        windowLocationService = TestBed.inject(WindowLocationService) as jasmine.SpyObj<WindowLocationService>;
        fixture.detectChanges();
    });
    
    it('should create', () => {
        expect(component).toBeTruthy();
    });
    
    it('should call handleCallback with token from URL fragment', () => {
        (windowLocationService.getLocation as jasmine.Spy).and.returnValue({
        hash: '#access_token=12345&other_param=value',
        search: ''
        } as Location);
        
        component.ngOnInit();
        
        expect(authService.handleCallback).toHaveBeenCalledWith('12345');
    });
    
    it('should call handleCallback with token from URL query string', () => {
        spyOnProperty(window, 'location', 'get').and.returnValue({
        hash: '',
        search: '?access_token=67890&other_param=value'
        } as Location);
        
        component.ngOnInit();
        
        expect(authService.handleCallback).toHaveBeenCalledWith('67890');
    });
    
    it('should navigate to /login if no token is found', () => {
        spyOnProperty(window, 'location', 'get').and.returnValue({
        hash: '',
        search: ''
        } as Location);
        
        component.ngOnInit();
        
        expect(router.navigate).toHaveBeenCalledWith(['/login']);
    });
    });
// This unit test suite for the GoogleCallbackComponent checks that the component correctly
// extracts the access token from the URL and calls the AuthService's handleCallback method.
// It also verifies that the component navigates to the login page if no token is found.
// The tests cover scenarios for both URL fragment and query string tokens, ensuring the component behaves as expected in different cases.
// The test suite uses Jasmine and Angular's testing utilities to create a test environment,
// mock dependencies, and verify the component's behavior through assertions.
// The tests ensure that the component is created successfully, handles the token extraction correctly,
// and navigates to the login page when no token is present in the URL.
// This code is a complete unit test for the GoogleCallbackComponent, which tests the component's behavior
// when it initializes and processes the Google authentication callback.
// The tests cover the following scenarios:
// 1. The component is created successfully.
// 2. The component extracts the access token from the URL fragment and calls the AuthService's handleCallback method.
// 3. The component extracts the access token from the URL query string and calls the AuthService's handleCallback method.
// 4. The component navigates to the login page if no access token is found in the URL.
// The test suite uses Jasmine's spy functionality to mock the AuthService and Router dependencies,
// allowing for verification of method calls and parameters without needing the actual implementations.
// The tests also use Angular's TestBed to configure the testing module and create the component instance.
// The test suite is structured to run before each test, setting up the component and its dependencies,
// ensuring a clean state for each test case.
