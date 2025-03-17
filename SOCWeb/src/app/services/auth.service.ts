import { Injectable, NgZone } from '@angular/core';
import { OAuthService, AuthConfig } from 'angular-oauth2-oidc';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'https://localhost:5001/api';
  private token: string | null = null;
  private userSubject = new BehaviorSubject<any>(null);
  user$ = this.userSubject.asObservable();

  constructor(private oauthService: OAuthService, private ngZone: NgZone, private http: HttpClient, private router: Router) {
    this.configureGoogle();
  }

  // Google OAuth Configuration
  private configureGoogle() {
    const googleConfig: AuthConfig = {
      issuer: 'https://accounts.google.com',
      strictDiscoveryDocumentValidation: false,
      clientId: 'YOUR_GOOGLE_CLIENT_ID', // Replace with your Google Client ID
      redirectUri: window.location.origin + '/auth/callback',
      scope: 'openid profile email',
      responseType: 'code', // Use Authorization Code Flow with PKCE
      useSilentRefresh: true,
    };
    this.oauthService.configure(googleConfig);
    this.oauthService.loadDiscoveryDocumentAndTryLogin().then(() => {
      if (this.oauthService.hasValidAccessToken()) {
        this.loadUserProfile();
      }
    });
  }

  private loadUserProfile() {
    this.oauthService.loadUserProfile().then((userInfo) => {
      this.userSubject.next(userInfo);
    });
  }

  // Google Login
  loginWithGoogle() {
    this.oauthService.initCodeFlow();
  }

  // Logout
  logout() {
    this.oauthService.logOut();
    this.userSubject.next(null);
  }

  // Check if user is logged in
  isLoggedIn(): boolean {
    return this.oauthService.hasValidAccessToken();
  }
}

  login(username: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, { username, password });
  }
  setToken(token: string): void {
    this.token = token;
    localStorage.setItem('access_token', token);
  }
  getToken(): string | null {
    if (!this.token) {
      this.token = localStorage.getItem('access_token');
    }
    return this.token;
  }
  logout(): void {
    this.token = null;
    localStorage.removeItem('access_token');
    this.router.navigate(['/login']);
  }
  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
