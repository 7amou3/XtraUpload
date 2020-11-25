import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ILanguage } from 'app/domain';
import { SettingsService, UserStorageService } from 'app/services';
import { ComponentBase, ILoggedin } from 'app/shared';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-langages',
  templateUrl: './languages.component.html',
  styleUrls: ['./languages.component.scss']
})
export class LanguagesComponent extends ComponentBase implements OnInit {
  languages$ = new Subject<ILanguage[]>();
  selectedCultures : string[] = [];
  constructor(
    private settingService: SettingsService,
    private userStorage: UserStorageService,
    private dialogRef: MatDialogRef<LanguagesComponent>,
    @Inject(MAT_DIALOG_DATA) private loggedIn: ILoggedin
    ) {
      super();
     }

  ngOnInit(): void {
    this.settingService.getLanguages()
    .pipe(takeUntil(this.onDestroy))
    .subscribe(languages => {
      this.languages$.next(languages);
      this.selectedCultures[0] = this.userStorage.getLang();
    });
  }
  onLangSelected() {
    this.userStorage.updateLang(this.selectedCultures[0]);
    if (!this.loggedIn?.isLoggedIn) {
      // Reload the app to apply the selected lang
      window.location.href = '/'
    }
    // User is loggedin, update user lang in db
    else {
      this.settingService.updateLanguage(this.selectedCultures[0])
      .pipe(takeUntil(this.onDestroy))
      .subscribe(result => {
        if (result) {
          // Reload the app since to apply the selected lang
          window.location.href = '/'
        }
      });
    }
  }
}
