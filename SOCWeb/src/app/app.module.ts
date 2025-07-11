import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { RegistrationFormComponent } from './registration-form/registration-form.component';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AuthCallbackComponent } from './auth-callback/auth-callback.component';
import { TitleSearchFormComponent } from './title-search-form/title-search-form.component';

import { MatButtonModule } from '@angular/material/button';

@NgModule({
  declarations: [
    AppComponent,
    RegistrationFormComponent,
    NavBarComponent,
    DashboardComponent
  ],
  imports: [
    BrowserModule,
    MatButtonModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    TitleSearchFormComponent,
    AuthCallbackComponent,
    LoginComponent
  ],
  providers: [provideHttpClient(withInterceptorsFromDi())],
  bootstrap: [AppComponent]
})
export class AppModule { }