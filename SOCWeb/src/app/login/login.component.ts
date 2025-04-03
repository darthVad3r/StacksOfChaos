import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  constructor(
    private readonly authService: AuthService,
    private readonly route: ActivatedRoute
  ) {
    this.route.queryParams.subscribe({
      next: (params) => {
        if (params['token']) {
          this.authService.handleCallback(params['token']);
        }
      },
      error: (err) => {
        console.error('Error occurred while processing query params:', err);
      },
      complete: () => {
        console.log('Query params processing completed.');
      },
    });
  }

  loginWithGoogle(): void {
    this.authService.googleLogin();
  }
}
