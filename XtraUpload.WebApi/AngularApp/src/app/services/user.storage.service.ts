import { Injectable } from '@angular/core';
import { ILanguage, IPageHeader, IProfile, IWebAppInfo } from 'app/domain';
const PROFILE = 'xu-Profile';
const APP_INFO = 'xu-AppInfo';
const APP_LANG = 'xu-Lang';
const STATICPAGE_LINKS = 'xu-PageLinks';
/**
 *  Store user data to localstorage
 * */
@Injectable({ providedIn: 'root' })
export class UserStorageService {
  private _profile: IProfile;
  private _applanguages: ILanguage[];
  private _appinfo: IWebAppInfo;
  private _pagelinks: IPageHeader[];
  constructor() { }

  /** Get the profile from memory or local storage */
  get profile(): IProfile {
    if (!this._profile) {
      const profile = localStorage.getItem(PROFILE);
      if (profile) {
        this._profile = JSON.parse(profile);
      }
    }
    return this._profile;
  }

  /** Save the profile to local storage */
  set profile(profile: IProfile) {
    if (!profile) return;
    window.localStorage.setItem(PROFILE, JSON.stringify(profile));
    this._profile = profile;
  }

  get jwt() {
    return this._profile?.jwtToken?.token;
  }
  
  get userlanguage(): ILanguage {
    return this._profile.language;
  }
  set userlanguage(language: ILanguage) {
    // If the user is not logged in, we create a Profile placeholder
    if (!this._profile) {
      this._profile = {} as IProfile;
    }

    this._profile.language = language;
    // save to local storages
    this.profile = this._profile;
  }
  get applanguages(): ILanguage[] {
    if (!this._applanguages) {
      const applangs = localStorage.getItem(APP_LANG);
      if (applangs) {
        this._applanguages = JSON.parse(applangs);
      }
    }
    return this._applanguages;
  }
  set applanguages(languages: ILanguage[]) {
    if (!languages) return;
    this._applanguages = languages;
    window.localStorage.setItem(APP_LANG, JSON.stringify(languages));
  }
  get appinfo(): IWebAppInfo {
    if (!this._appinfo) {
      const appinfo = localStorage.getItem(APP_INFO);
      if (appinfo) {
        this._appinfo = JSON.parse(appinfo);
      }
    }
    return this._appinfo;
  }
  set appinfo(appInfo: IWebAppInfo) {
    if (!appInfo) return;
    this._appinfo = appInfo;
    window.localStorage.setItem(APP_INFO, JSON.stringify(appInfo));
  }
  
  get pagelinks(): IPageHeader[] {
    if (!this._pagelinks) {
      const pagelinks = localStorage.getItem(STATICPAGE_LINKS);
      if (pagelinks) {
        this._pagelinks = JSON.parse(pagelinks);
      }
    }
    return this._pagelinks;
  }
  set pagelinks(pagelinks: IPageHeader[]) {
    if (!pagelinks) return;
    this._pagelinks = pagelinks;
    window.localStorage.setItem(STATICPAGE_LINKS, JSON.stringify(pagelinks));
  }
  updateTheme(theme: 'dark' | 'light') {
    // If the user is not logged in we create a Profile placeholder
    if (!this._profile) {
      this._profile = {} as IProfile;
    }
    this._profile.theme = theme;
    // Save to local storage
    this.profile = this._profile;
  }
  /**Clear local storage but keep user preferences (ex language, theme..) */
  clearLocalStorage() {
    // Delete all key value pairs
    window.localStorage.clear();
    // Save user preferences to local storage
    this.profile = { language: this._profile.language, theme: this._profile.theme } as IProfile;
  }
}
