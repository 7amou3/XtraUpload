import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthRoutes } from './auth.routing';
import { FlexLayoutModule } from '@angular/flex-layout';
import { SharedModule, MessageModule } from '../shared';
import { PipeModule } from '../shared/pipe-modules';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthService, UserStorageService, AuthUnGuardService } from '../services';
import { SocialmediaComponent } from './socialmedia/socialmedia.component';
import { SocialLoginModule, SocialAuthServiceConfig } from 'angularx-social-login';
import { LoginComponent } from './login/login.component';
import { SignupComponent } from './signup/signup.component';
import { ForgotpwdComponent } from './forgotpwd/forgotpwd.component';
import { RecoverpwdComponent } from './recoverpwd/recoverpwd.component';
import { ConfirmemailComponent } from './confirmemail/confirmemail.component';

export function socialLoginFactory(authService: AuthService): Promise<SocialAuthServiceConfig> {
  return authService.loadConfig().toPromise();
}

@NgModule({
  declarations: [
    LoginComponent,
    SignupComponent,
    SocialmediaComponent,
    ForgotpwdComponent,
    RecoverpwdComponent,
    ConfirmemailComponent,
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(AuthRoutes),
    FlexLayoutModule,
    SharedModule,
    MessageModule,
    PipeModule,
    FormsModule,
    ReactiveFormsModule,
    SocialLoginModule
  ],
  providers: [
    AuthService,
    AuthUnGuardService,
    UserStorageService,
    {
      provide: 'SocialAuthServiceConfig',
      useFactory: socialLoginFactory,
      deps: [AuthService]
    }
  ]
})
export class AuthModule { }
