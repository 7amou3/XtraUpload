import { Injectable } from '@angular/core';
import { getBrowserCultureLang } from '@locl/core';
import { IAppInitializerConfig, ILanguage, IPageHeader, IProfile, IWebAppInfo } from 'app/domain';
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
  getUserLang(): ILanguage {
    const languages = this.getAppLanguages();
    const profile = this.getProfile();
    if (profile?.language) {
      const langName = !languages 
                                  ? $localize`Language`
                                  : languages.filter(s => s.culture === profile.language)[0].name;
      return { name: langName, culture: profile.language }
    } 
    else return { culture: getBrowserCultureLang() } as ILanguage;
  }
  getAppLanguages(): ILanguage[] {
    const langs = localStorage.getItem(APP_LANG);
    if (!langs) {
      return null;
    }
    return JSON.parse(langs);
  }
  saveAppLanguages(langs: ILanguage[]) {
    if (!langs) return;
    window.localStorage.setItem(APP_LANG, JSON.stringify(langs));
  }
  updateLang(culture: string) {
    let profile = this.getProfile();
    // If the user is not logged in, we create a Profile placeholder
    if (!profile) {
      profile = {} as IProfile;
    }
    profile.language = culture;
    this.saveUser(profile);
  }
  updateTheme(theme: 'dark' | 'light') {
    let profile = this.getProfile();
    // If the user is not logged in we create a Profile placeholder
    if (!profile) {
      profile = {} as IProfile;
    }
    profile.theme = theme;
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

  /**Clear local storage but keep user preferences (ex language, theme..) */
  clearLocalStorage() {
    const user = this.getProfile();
    // Delete all key value pairs
    window.localStorage.clear();
    // Save user preferences
    const profile = { language: user.language, theme: user.theme} as IProfile;
    this.saveUser(profile);
  }
}
