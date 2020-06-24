import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { SignupComponent } from './signup/signup.component';
import { ForgotpwdComponent } from './forgotpwd/forgotpwd.component';
import { ConfirmemailComponent } from './confirmemail/confirmemail.component';
import { RecoverpwdComponent } from './recoverpwd/recoverpwd.component';
import { AuthUnGuardService } from '../services';

export const AuthRoutes: Routes = [
  { path: 'login', component: LoginComponent, canActivate: [AuthUnGuardService] },
  { path: 'signup', component: SignupComponent, canActivate: [AuthUnGuardService] },
  { path: 'forgotpassword', component: ForgotpwdComponent, canActivate: [AuthUnGuardService] },
  { path: 'confirmemail/:emailtoken', component: ConfirmemailComponent},
  { path: 'recoverpassword/:recoveryId', component: RecoverpwdComponent}
];
