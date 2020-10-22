import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IChangePassword, IWebSetting } from 'app/domain';
import { map } from 'rxjs/operators';
import { UserStorageService } from './user.storage.service';
import { of, Observable } from 'rxjs';
import { SeoService } from './seo.service';

@Injectable()
export class SettingsService {
  constructor(
    private http: HttpClient,
    private userStorage: UserStorageService,
    private seoService: SeoService) { }

  webappconfig(): Observable<IWebSetting> {
    const pagesetting = this.userStorage.getPageSetting();
    // make request to server only if cache expires (cache is valid for 5 min)
    if (!pagesetting || new Date().getTime() > new Date(pagesetting?.expire).getTime()) {
      return this.http.get<IWebSetting>('setting/webappconfig')
      .pipe(
        map(result => {
          result.expire = new Date();
          result.expire.setMinutes(result.expire.getMinutes() + 5);
          this.userStorage.savePageSetting(result);
          this.seoService.setMetaPage(result);
          return result;
        })
      );
    } else { return of(pagesetting); }
  }
  changePassword(changePassword: IChangePassword) {
    return this.http.patch('setting/password', changePassword);
  }
  updateTheme(theme: 'dark' | 'light') {
    let themeId = 0;
    if (theme === 'light') {
      themeId = 1;
    }
    return this.http.patch('setting/theme', {theme : themeId});
  }
}
