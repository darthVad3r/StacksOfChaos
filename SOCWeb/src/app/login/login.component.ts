import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginError: string = '';
  username: string = '';
  password: string = '';
  loginForm: any;

  constructor(private authService: AuthService, private router: Router) {}
  onLogin(): void {
    this.authService.login(this.username, this.password).subscribe(
      (data) => {
        this.authService.setToken(data.access_token);
        this.router.navigate(['/dashboard']);
      },
      (error) => {
        console.error('Login failed', error);
      }
    );
  }
}
