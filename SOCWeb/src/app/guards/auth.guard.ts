import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(private router: Router) {}

  canActivate(): boolean {
    const token = localStorage.getItem('jwtToken');
    // Check if the token exists in localStorage
    // Optionally, you can add more checks here (e.g., token expiration)
    // or use a service to validate the token
    // For simplicity, we just check if the token is present
    console.log('AuthGuard checking token:', token);
    if (token) {
      console.log('User is authenticated');
      // Optionally, you can redirect to a specific route if authenticated
      // this.router.navigate(['/dashboard']);
      return true;
    }
    this.router.navigate(['/login']);
    console.log('User is not authenticated, redirecting to login');
    return false;
  }
}