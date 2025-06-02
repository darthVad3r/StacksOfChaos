import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    console.log('AuthGuard checking authentication status');
    if (this.authService.isLoggedIn()) {
        // Optionally, you can redirect to a specific route if authenticated
        // this.router.navigate(['/dashboard']);
    //   return true;
        console.log('User is authenticated');
        this.router.navigate(['/dashboard']);
        return true;
    }
    this.router.navigate(['/login']);
    return false;
  }
}
