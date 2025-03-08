import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TitleSearchFormComponent } from './title-search-form/title-search-form.component';
import { RegistrationFormComponent } from './registration-form/registration-form.component';

const routes: Routes = [
  { path: 'title-search', component: TitleSearchFormComponent },
  { path: 'register', component: RegistrationFormComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
