import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TitleSearchFormComponent } from './title-search-form/title-search-form.component';
import { RegistrationFormComponent } from './registration-form/registration-form.component';
import { AuthGuard } from './guards/auth.guard';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';

const routes: Routes = [
  { path: 'title-search', component: TitleSearchFormComponent },
  { path: 'register', component: RegistrationFormComponent },
  { path: 'login', component: LoginComponent },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
