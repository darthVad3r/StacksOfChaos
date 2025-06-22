import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

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

constructor(private http: HttpClient) {}

  register(): void {
    if (this.password !== this.confirmPassword) {
      alert('Passwords do not match');
      return;
    };

  const user = {
    username: this.username,
    email: this.email,
    password: this.password
  }

  // create a new user or get existing user
  this.http.post('https://localhost:5001/api/auth/register-or-get-user', user)
    .subscribe(() => {
      alert('User registered successfully');
      // Optionally, redirect to login or home page
      // this.router.navigate(['/login']);
    }, (error) => {
      alert('An error occurred while registering the user ' +  error.message.toString());
      console.error('Error registering user', error);
    });
  }

  // this.http.post('https://localhost:5001/api/auth/register-or-get-user', user)
  //   .subscribe(() => {
  //     alert('User registered successfully');
  //   }, (error) => {
  //     console.error('Error registering user', error);
  //   });
  // }
}
