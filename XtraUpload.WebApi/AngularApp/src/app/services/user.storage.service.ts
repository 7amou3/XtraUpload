import { Injectable } from '@angular/core';
import { ILanguage, IPageHeader, IProfile, IWebAppInfo } from 'app/models';
const PROFILE = 'xu-Profile';
const APP_INFO = 'xu-AppInfo';
const APP_LANG = 'xu-Lang';
const STATICPAGE_LINKS = 'xu-PageLinks';

@Injectable({ providedIn: 'root' })
export class UserStorageService {
  private _profile: IProfile;
  private _applanguages: ILanguage[];
  private _appinfo: IWebAppInfo;
  private _pagelinks: IPageHeader[];
  constructor() {
    this.init();
   }
  
  get profile(): IProfile {
    return this._profile;
  }

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
    this._profile.language = language;
    // save to local storages
    this.profile = this._profile;
  }
  get applanguages(): ILanguage[] {
    return this._applanguages;
  }
  set applanguages(languages: ILanguage[]) {
    if (!languages) return;
    this._applanguages = languages;
    window.localStorage.setItem(APP_LANG, JSON.stringify(languages));
  }
  get appinfo(): IWebAppInfo {
    return this._appinfo;
  }
  set appinfo(appInfo: IWebAppInfo) {
    if (!appInfo) return;
    this._appinfo = appInfo;
    window.localStorage.setItem(APP_INFO, JSON.stringify(appInfo));
  }
  
  get pagelinks(): IPageHeader[] {
    return this._pagelinks;
  }
  set pagelinks(pagelinks: IPageHeader[]) {
    if (!pagelinks) return;
    this._pagelinks = pagelinks;
    window.localStorage.setItem(STATICPAGE_LINKS, JSON.stringify(pagelinks));
  }
  updateTheme(theme: 'dark' | 'light') {
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
  private init() {
    const initfield = (itemname: string) => {
      const item = localStorage.getItem(itemname);
      return JSON.parse(item);
    }
    this._appinfo = initfield(APP_INFO);
    this._profile = initfield(PROFILE) ?? {};
    this._applanguages = initfield(APP_LANG);
    this._pagelinks = initfield(STATICPAGE_LINKS);
  }
}
