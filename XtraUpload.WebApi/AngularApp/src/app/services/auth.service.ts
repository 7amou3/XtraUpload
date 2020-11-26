import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap, map } from 'rxjs/operators';
import { GoogleLoginProvider, FacebookLoginProvider, SocialAuthServiceConfig } from 'angularx-social-login';
import { IProfile, ILoginParams, ISignupParams, RecoverPassword, IExtendedSocialUser } from 'app/domain';
import { UserStorageService } from './user.storage.service';
import { Observable } from 'rxjs';

@Injectable()
export class AuthService {
  constructor(private http: HttpClient, private userStorage: UserStorageService) { }
  async requestLogin(loginParams: ILoginParams): Promise<IProfile> {
    return this.http.post<IProfile>('user/login', loginParams)
      .pipe(
        tap(profile => {
          const p = this.userStorage.getProfile();
          if (p) {
            if (p.theme) profile.theme = p.theme;
            if (p.language) profile.language = p.language;
          }
          this.userStorage.saveUser(profile);
        })
      )
      .toPromise();
  }
  async socialmediaAuth(user: IExtendedSocialUser) {
    return this.http.post<IProfile>('user/socialauth', user)
      .pipe(
        tap(profile => {
          const p = this.userStorage.getProfile();
          if (p) {
            if (p.theme) profile.theme = p.theme;
            if (p.language) profile.language = p.language;
          }
          this.userStorage.saveUser(profile);
        })
      );
  }
  loadConfig(): Promise<SocialAuthServiceConfig> {
    return this.http.get('setting/socialauthconfig')
      .pipe(
        map((r: any) => {
          return {
            autoLogin: false,
            providers: [
              {
                id: GoogleLoginProvider.PROVIDER_ID,
                provider: new GoogleLoginProvider(
                  r.googleAuth.clientId
                ),
              },
              {
                id: FacebookLoginProvider.PROVIDER_ID,
                provider: new FacebookLoginProvider(
                  r.facebookAuth.appId
                ),
              }
            ]
          };
        })
      )
      .toPromise();
  }
  requestSignup(signupParams: ISignupParams) {
    return this.http.post('user/register', signupParams);
  }
  resetPassword(lostPwdParams) {
    return this.http.post('user/lostpassword', lostPwdParams);
  }
  recoverPassInfo(recoveryId: string) {
    return this.http.get('user/pwdrecoveryinfo/' + recoveryId);
  }
  updatePassword(recoverPassword: RecoverPassword) {
    return this.http.put('user/recoverPassword/', recoverPassword);
  }
  requestConfirmEmail() {
    return this.http.get('setting/confirmemail/');
  }
  confirmEmail(emailToken: string) {
    return this.http.put('setting/confirmemail/', { emailToken: emailToken });
  }
  isUserAuthicated(): boolean {
    const user = this.userStorage.getProfile();
    if (user && user.jwtToken) {
      return true;
    }
    return false;
  }
  isAdminAuthenticated(): boolean {
    const admin = this.userStorage.getProfile();
    if (admin && admin.jwtToken && admin.role === 'Admin') {
      return true;
    }
    return false;
  }
  signOut(): void {
    this.userStorage.clearLocalStorage();
    window.location.href = '/';
  }
}
