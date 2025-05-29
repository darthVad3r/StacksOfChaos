import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-dashboard',
  styleUrls: ['./dashboard.component.css'],
  standalone: false,
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  userName: string | null = null;
  userEmail: string | null = null;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    const jwtHelper = new JwtHelperService();
    this.route.queryParams.subscribe(params => {
      const token = params['token'];
      if (token) {
        localStorage.setItem('token', token);
        const decodedToken = jwtHelper.decodeToken(token);
        this.userName = decodedToken['name'];
        this.userEmail = decodedToken['email'];
      }
    });
  }
}
