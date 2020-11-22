import { Component, Output, EventEmitter, Input, OnInit } from '@angular/core';
import { AuthService, UserStorageService, SettingsService, HeaderService } from 'app/services';
import { ComponentBase, ILoggedin } from 'app/shared';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.scss']
})
export class HeaderComponent extends ComponentBase implements OnInit {
  @Input() currentTheme: 'dark' | 'light';
  @Output() selectedTheme = new EventEmitter<'dark' | 'light'>();
  avatar: string;
  loggedIn: ILoggedin;
  constructor(
    private authService: AuthService,
    private settingService: SettingsService,
    private storageService: UserStorageService,
    private headerService: HeaderService) {
      super();
    }
   ngOnInit() {
      const profile = this.storageService.getProfile();
      if (!profile?.avatar) {
          this.avatar = 'assets/images/users/profile-icon.png';
      } else {
        this.avatar = profile.avatar;
      }
      // Listen to avatar change request
      this.headerService.subscribeAvatarChange()
     .pipe(takeUntil(this.onDestroy))
     .subscribe( avatarData => {
       const user = this.storageService.getProfile();

       if (user) {
         this.avatar = null;
         this.avatar = avatarData.avatarUrl + '/' + Date.now();
         user.avatar = avatarData.avatarUrl;
         this.storageService.saveUser(user);
       }
     });
   }
  onLoggedIn(loggedIn: ILoggedin) {
    this.loggedIn = loggedIn;
  }
  onChangeThemeClick(theme: 'dark' | 'light') {
    this.selectedTheme.emit(theme);
    if (this.loggedIn.isLoggedIn) {
      this.settingService.updateTheme(theme)
      .pipe(takeUntil(this.onDestroy))
      .subscribe();
    }
  }
  onSignOut() {
    this.authService.signOut();
  }
}
