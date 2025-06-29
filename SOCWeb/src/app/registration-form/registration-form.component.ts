import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registration-form',
  standalone: false,
  templateUrl: './registration-form.component.html',
  styleUrl: './registration-form.component.css'
})
export class RegistrationFormComponent {

  username: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  constructor(private http: HttpClient, private router: Router) {}

  register(): void {
    if (this.password !== this.confirmPassword) {
      alert('Passwords do not match');
      return;
    }

    // Only send email and username, as backend expects those
    const user = {
      email: this.email,
      name: this.username,
      password: this.password
    };

    // Send as JSON, Angular will set Content-Type automatically
    this.http.post('https://localhost:5001/api/auth/register-or-get-user', user)
      .subscribe({
        next: () => {
          alert('User registered successfully');
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          alert('An error occurred while registering the user: ' + error.message.toString());
          console.error('Error registering user', error);
        }
      });
  }
}
