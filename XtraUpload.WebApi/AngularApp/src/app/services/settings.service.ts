import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IAppInitializerConfig, IChangePassword } from 'app/models';
import { map } from 'rxjs/operators';
import { UserStorageService } from './user.storage.service';
import { SeoService } from './seo.service';

@Injectable()
export class SettingsService {
  constructor(
    private http: HttpClient,
    private userStorage: UserStorageService,
    private seoService: SeoService) { }

  async appInitializerConfig(): Promise<IAppInitializerConfig> {
    return this.http.get<IAppInitializerConfig>('setting/appinitializerconfig')
      .pipe(
        map(result => {
          result.appInfo.version = result.version;
          this.userStorage.pagelinks = result.pagesHeader;
          this.userStorage.appinfo = result.appInfo;
          this.seoService.setMetaPage(result.appInfo);
          return result;
        })
      )
      .toPromise();
  }
  async changePassword(changePassword: IChangePassword) {
    return this.http.patch('setting/password', changePassword).toPromise();
  }
  async updateTheme(theme: 'dark' | 'light') {
    let themeId = 0;
    if (theme === 'light') {
      themeId = 1;
    }
    return this.http.patch('setting/theme', { theme: themeId }).toPromise();
  }
  
}
