import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap, map } from 'rxjs/operators';
import { GoogleLoginProvider, FacebookLoginProvider, SocialAuthServiceConfig } from 'angularx-social-login';
import { IProfile, ILoginParams, ISignupParams, RecoverPassword, IExtendedSocialUser } from 'app/domain';
import { UserStorageService } from './user.storage.service';

@Injectable()
export class AuthService {
  constructor(private http: HttpClient, private userStorage: UserStorageService) { }
  async requestLogin(loginParams: ILoginParams): Promise<IProfile> {
    return this.http.post<IProfile>('user/login', loginParams)
      .pipe(
        tap(profile => {
          const p = this.userStorage.profile;
          if (p) {
            if (p.theme) profile.theme = p.theme;
            if (p.language) profile.language = p.language;
          }
          this.userStorage.profile = profile;
        })
      ).toPromise();
  }
  async socialmediaAuth(user: IExtendedSocialUser) {
    return this.http.post<IProfile>('user/socialauth', user)
      .pipe(
        tap(profile => {
          const p = this.userStorage.profile;
          if (p) {
            if (p.theme) profile.theme = p.theme;
            if (p.language) profile.language = p.language;
          }
          this.userStorage.profile = profile;
        })
      ).toPromise();
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
  async requestSignup(signupParams: ISignupParams) {
    return this.http.post('user/register', signupParams).toPromise();
  }
  async resetPassword(lostPwdParams) {
    return this.http.post('user/lostpassword', lostPwdParams).toPromise();
  }
  async recoverPassInfo(recoveryId: string) {
    return this.http.get('user/pwdrecoveryinfo/' + recoveryId).toPromise();
  }
  async updatePassword(recoverPassword: RecoverPassword) {
    return this.http.put('user/recoverPassword/', recoverPassword).toPromise();
  }
  async requestConfirmEmail() {
    return this.http.get('setting/confirmemail/').toPromise();
  }
  async confirmEmail(emailToken: string) {
    return this.http.put('setting/confirmemail/', { emailToken: emailToken }).toPromise();
  }
  isUserAuthicated(): boolean {
    const user = this.userStorage.profile;
    return user != null && user.jwtToken != null;
  }
  isAdminAuthenticated(): boolean {
    const admin = this.userStorage.profile;
    return admin != null && admin.jwtToken != null && admin.role === 'Admin'
  }
  signOut(): void {
    this.userStorage.clearLocalStorage();
    window.location.href = '/';
  }
}
