// auth.service.ts
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = 'https://localhost:5001/api/auth';

  constructor(private readonly http: HttpClient, private readonly router: Router) {}

  login(username: string, password: string) {
    return this.http.post(`${this.apiUrl}/login`, { username, password });
  }

  logout() {
    localStorage.removeItem('token');
  }

  isAuthenticated() {
    try {
      console.log('Checking authentication status...');
      const token = localStorage.getItem('token');
      if (!token) {
        console.log('No token found. User is not authenticated.');
        return false;
      }
      // Decode the token to check its validity
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiration = payload.exp;
      const now = Math.floor(Date.now() / 1000);
      if (expiration < now) {
        console.log('Token has expired. User is not authenticated.');
        return false;
      }
      console.log('Token is valid. User is authenticated.');
      return true;
    }
    catch (err){
      console.log('Error decoding token:', err);
    }
    return false;
  }

  googleLogin() {
    window.location.href = `${this.apiUrl}/google-login`;
    if (window.location.href.includes('access_token')) {
      const token = window.location.href
        .split('access_token=')[1]
        .split('&')[0];
      this.handleCallback(token);
    } else {
      console.error('Access token not found in URL');
    }
  }

  // auth.service.ts
  handleCallback(token: string) {
    this.http.get<{ token: string }>(`${this.apiUrl}/google-login/callback`, { withCredentials: true })
    .subscribe({
      next: (response) => {
        localStorage.setItem('token', response.token);
        this.router.navigate(['/dashboard']);
        console.log('Google login successful, token stored:', response.token);
      },
      error: (error) => {
        console.error('Error during Google login callback:', error);
      },
    })
  }
}
