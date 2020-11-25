import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IAppInitializerConfig, IChangePassword, ILanguage, IWebAppInfo } from 'app/domain';
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

  appInitializerConfig(): Observable<IAppInitializerConfig> {
    return this.http.get<IAppInitializerConfig>('setting/appinitializerconfig')
      .pipe(
        map(result => {
          this.userStorage.saveAppSettings(result);
          this.seoService.setMetaPage(result.appInfo);
          return result;
        })
      );

  }
  changePassword(changePassword: IChangePassword) {
    return this.http.patch('setting/password', changePassword);
  }
  updateTheme(theme: 'dark' | 'light') {
    let themeId = 0;
    if (theme === 'light') {
      themeId = 1;
    }
    return this.http.patch('setting/theme', { theme: themeId });
  }
  getLanguages(): Observable<ILanguage[]> {
    return this.http.get<ILanguage[]>('setting/languages');
  }
  updateLanguage(culture: string) {
    return this.http.patch<ILanguage>('setting/language', {culture: culture});
  }
}
