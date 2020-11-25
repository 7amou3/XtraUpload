import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService, UserStorageService } from 'app/services';
import { ComponentBase } from 'app/shared';
import { takeUntil, finalize } from 'rxjs/operators';
import { SocialUser } from 'angularx-social-login';
import { SocialAuthService, FacebookLoginProvider, GoogleLoginProvider } from 'angularx-social-login';
import { IExtendedSocialUser, IGenericMessage } from 'app/domain';

@Component({
  selector: 'app-socialmedia',
  templateUrl: './socialmedia.component.html',
  styleUrls: ['./socialmedia.component.css']
})
export class SocialmediaComponent extends ComponentBase implements OnInit {
  @Output() loginMessage = new EventEmitter<IGenericMessage>();
  constructor(
    private localAuth: AuthService,
    private socialAuth: SocialAuthService,
    private userStorage: UserStorageService
    ) {
    super();
  }

  signInWithGoogle(): void {
    this.socialAuth
    .signIn(GoogleLoginProvider.PROVIDER_ID)
    .then(user => this.process(user));
  }

  ngOnInit(): void {
  }
  signInWithFB(): void {
    this.socialAuth
    .signIn(FacebookLoginProvider.PROVIDER_ID)
    .then(user => this.process(user));
  }

  process(user: SocialUser) {
    this.isBusy = true;
    let exUser: IExtendedSocialUser = Object.create(user);
    exUser.language = this.userStorage.getLang();
    this.localAuth.socialmediaAuth(exUser)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
    .subscribe(
      () => {
        // Reload the entire app
        window.location.href = '/filemanager';
      },
      error => {
        this.loginMessage.emit({errorMessage: error?.error?.errorContent?.message});
        throw error;
      }
    );
  }

}
