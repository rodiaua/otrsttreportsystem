import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TtReportComponent } from './tt-report/tt-report.component';
import { AppComponent } from './app.component';
import { UserComponent } from './user/user.component';
import { RegistrationComponent } from './user/registration/registration.component';
import { LoginComponent } from './login/login.component';
import {  AuthGuard} from "./auth/auth.guard";
import { UserProfileComponent } from './user-profile/user-profile.component';
import { ReportCreatorComponent } from './report-creator/report-creator.component';
import { PendingTicketsComponent } from './pending-tickets/pending-tickets.component';

const routes: Routes = [
  // {path: 'user', component: UserComponent, children:[
  //   {path: 'registration', component: RegistrationComponent}
  // ]},
  {path: 'users', component: UserComponent, canActivate: [AuthGuard], data: {permittedRoles: ["Admin", "User"]}},
  {path: 'login', component: LoginComponent},
  {path: 'users/registration', component: RegistrationComponent, canActivate: [AuthGuard], data: {permittedRoles: ["Admin", "User"]}},
  {path: 'reportTable', component: TtReportComponent, canActivate: [AuthGuard], data: {permittedRoles: ["Admin", "User"]}},
  {path: 'user/profile', component: UserProfileComponent, canActivate: [AuthGuard], data: {permittedRoles: ["Admin", "User"]}},
  {path: 'report-creator', component: ReportCreatorComponent, canActivate: [AuthGuard], data: {permittedRoles: ["Admin", "User"]}},
  {path: 'pending-tickets', component: PendingTicketsComponent, canActivate: [AuthGuard], data: {permittedRoles: ["Admin", "User"]}},

  
  {path: '**', redirectTo: 'user/profile'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
