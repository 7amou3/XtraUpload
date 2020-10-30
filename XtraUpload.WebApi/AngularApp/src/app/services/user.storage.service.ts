import { Injectable } from '@angular/core';
import { IAppInitializerConfig, IPageHeader, IProfile, IWebAppInfo } from 'app/domain';
const PROFILE = 'xu-Profile';
const APP_INFO = 'xu-AppInfo';
const STATICPAGE_LINKS = 'xu-PageLinks';
/**
 *  Store user data to localstorage
 * */
@Injectable()
export class UserStorageService {
  constructor() { }
  getAppInfo(): IWebAppInfo {
    const pageSetting = localStorage.getItem(APP_INFO);
    if (!pageSetting) {
      return null;
    }
    return JSON.parse(pageSetting);
  }
  getPageLinks(): IPageHeader[] {
    const links = localStorage.getItem(STATICPAGE_LINKS);
    if (!links) {
      return null;
    }
    return JSON.parse(links);
  }
  saveAppSettings(pageSettings: IAppInitializerConfig) {
    if (!pageSettings) return;
    pageSettings.appInfo.version = pageSettings.version;
    window.localStorage.removeItem(APP_INFO);
    window.localStorage.removeItem(STATICPAGE_LINKS);
    window.localStorage.setItem(APP_INFO, JSON.stringify(pageSettings.appInfo));
    window.localStorage.setItem(STATICPAGE_LINKS, JSON.stringify(pageSettings.pagesHeader));
  }
  saveUser(profile: IProfile): IProfile {
    if (!profile) {
      return;
    }
    if ((profile.theme as unknown) === 0 || profile.theme === 'dark') {
      profile.theme = 'dark';
    }
    else {
      profile.theme = 'light';
    }
    window.localStorage.removeItem(PROFILE);
    window.localStorage.setItem(PROFILE, JSON.stringify(profile));
    return profile;
  }

  getProfile(): IProfile {
    const user = localStorage.getItem(PROFILE);
    if (!user) {
      return null;
    }
    return JSON.parse(user);
  }

  getToken(): string {
    const user = this.getProfile();
    if (!user) {
      return null;
    }
    return user.jwtToken?.token;
  }

  clearLocalStorage() {
    window.localStorage.removeItem(PROFILE);
    window.localStorage.clear();
  }
}
