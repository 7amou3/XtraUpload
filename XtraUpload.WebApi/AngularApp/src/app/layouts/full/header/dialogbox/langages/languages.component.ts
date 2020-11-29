import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ILanguage } from 'app/domain';
import { LanguageService, UserStorageService } from 'app/services';
import { ComponentBase, ILoggedin } from 'app/shared';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-langages',
  templateUrl: './languages.component.html',
  styleUrls: ['./languages.component.scss']
})
export class LanguagesComponent extends ComponentBase implements OnInit {
  languages$ = new BehaviorSubject<ILanguage[]>(null);
  selectedCultures : string[] = [];
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
      this.selectedCultures = [this.userStorage.userlanguage.culture];
    })
  }
  async onLangClick() {
    this.isBusy = true;
    const lang = this.languages$.getValue().find(s => s.culture === this.selectedCultures[0]);
    if (!lang) {
      throw Error('Invalid language!')
    }
    // Save to lacoal storage
    this.userStorage.userlanguage = lang;
    if (!this.loggedIn?.isLoggedIn) {
      // Reload the app to apply the selected lang
      window.location.href = '/'
    }
    // User is loggedin, update user lang in db
    else {
      const result = await this.languageService.updateLanguage(this.selectedCultures[0])
      if (result) {
        // Reload the app since to apply the selected lang
        window.location.href = '/'
      }
    }
  }
}
