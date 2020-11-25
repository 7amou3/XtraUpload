import { Injectable } from '@angular/core';
import { getBrowserCultureLang } from '@locl/core';
import { IAppInitializerConfig, IPageHeader, IProfile, IWebAppInfo } from 'app/domain';
const PROFILE = 'xu-Profile';
const APP_INFO = 'xu-AppInfo';
const APP_LANG = 'xu-Lang';
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
  getLang(): string {
    const profile = this.getProfile();
    return profile?.language ?? getBrowserCultureLang();
  }
  updateLang(culture: string) {
    let profile = this.getProfile();
    // If the user is not logedin we create a Profile placeholder
    if (!profile) {
      profile = {} as IProfile;
    }
    profile.language = culture;
    this.saveUser(profile);
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
