import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import {TableModule} from 'primeng/table';
import {MenubarModule} from 'primeng/menubar';
import {CalendarModule} from 'primeng/calendar';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {ButtonModule} from 'primeng/button';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import {MessagesModule} from 'primeng/messages';
import {ToastModule} from 'primeng/toast';
import {DropdownModule} from 'primeng/dropdown';
import {MultiSelectModule} from 'primeng/multiselect';
import {ToggleButtonModule} from 'primeng/togglebutton';
import { ReactiveFormsModule } from "@angular/forms";
import {PasswordModule} from 'primeng/password';
import { JwtInterceptor } from "./auth/jwt.interceptor";
import { ErrorInterceptor } from "./auth/error.interceptor";
import {CheckboxModule} from 'primeng/checkbox';
import {DynamicDialogModule} from 'primeng/dynamicdialog';
import {PickListModule} from 'primeng/picklist';
import {TabViewModule} from 'primeng/tabview';



import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TtReportComponent } from './tt-report/tt-report.component';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { HomeComponent } from './home/home.component';
import { UserComponent } from './user/user.component';
import { RegistrationComponent } from './user/registration/registration.component';
import { LoginComponent } from './login/login.component';
import {  UserService } from "./services/user.service";
import { UserProfileComponent } from './user-profile/user-profile.component';
import { ChangePasswordComponent } from './user-profile/change-password/change-password.component';
import { ResetPasswordComponent } from './user/reset-password/reset-password.component';
import { TwoFactorAuthenticationComponent } from './login/two-factor-authentication/two-factor-authentication.component';
import { ReportCreatorComponent } from './report-creator/report-creator.component';
import { PendingTicketsComponent } from './pending-tickets/pending-tickets.component';


@NgModule({
  declarations: [
    AppComponent,
    TtReportComponent,
    NavBarComponent,
    HomeComponent,
    UserComponent,
    RegistrationComponent,
    LoginComponent,
    UserProfileComponent,
    ChangePasswordComponent,
    ResetPasswordComponent,
    TwoFactorAuthenticationComponent,
    ReportCreatorComponent,
    PendingTicketsComponent
  ],
  imports: [
    TabViewModule,
    PickListModule,
    BrowserModule,
    AppRoutingModule,
    MenubarModule,
    CalendarModule,
    FormsModule,
    BrowserAnimationsModule,
    ButtonModule,
    HttpClientModule,
    TableModule,
    MessagesModule,
    DropdownModule,
    MultiSelectModule,
    ToggleButtonModule,
    ReactiveFormsModule,
    PasswordModule,
    ToastModule,
    CheckboxModule,
    DynamicDialogModule
  ],
  providers: [UserService,
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
