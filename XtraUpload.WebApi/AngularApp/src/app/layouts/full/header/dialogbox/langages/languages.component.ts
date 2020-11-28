import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ILanguage } from 'app/domain';
import { LanguageService, UserStorageService } from 'app/services';
import { ComponentBase, ILoggedin } from 'app/shared';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-langages',
  templateUrl: './languages.component.html',
  styleUrls: ['./languages.component.scss']
})
export class LanguagesComponent extends ComponentBase implements OnInit {
  languages$ = new Subject<ILanguage[]>();
  selectedLangs : ILanguage[] = [];
  constructor(
    private languageService: LanguageService,
    private userStorage: UserStorageService,
    private dialogRef: MatDialogRef<LanguagesComponent>,
    @Inject(MAT_DIALOG_DATA) private loggedIn: ILoggedin
    ) {
      super();
     }

  async ngOnInit() {
    await this.languageService.getLanguages()
    .then((languages) => {
      this.languages$.next(languages);
      this.selectedLangs[0] = this.userStorage.getUserLang();
    })
  }
  async onLangClick() {
    this.isBusy = true;
    this.userStorage.setUserLang(this.selectedLangs[0]);
    if (!this.loggedIn?.isLoggedIn) {
      // Reload the app to apply the selected lang
      window.location.href = '/'
    }
    // User is loggedin, update user lang in db
    else {
      const result = await this.languageService.updateLanguage(this.selectedLangs[0].culture)
      if (result) {
        // Reload the app since to apply the selected lang
        window.location.href = '/'
      }
    }
  }
}
