import { Injectable } from '@angular/core';
import { IProfile, IWebSetting } from 'app/domain';
const PROFILE_DATA = 'XtraUpload';
const WEBPAGE_DATA = 'Webpage';
/**
 *  Store user data to localstorage
 * */
@Injectable()
export class UserStorageService {
  constructor() { }
  getPageSetting(): IWebSetting {
    const pageSetting = localStorage.getItem(WEBPAGE_DATA);
    if (!pageSetting) {
      return null;
    }
    return JSON.parse(pageSetting);
  }
  savePageSetting(websetting: IWebSetting) {
    window.localStorage.removeItem(WEBPAGE_DATA);
    window.localStorage.setItem(WEBPAGE_DATA, JSON.stringify(websetting));
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
    window.localStorage.removeItem(PROFILE_DATA);
    window.localStorage.setItem(PROFILE_DATA, JSON.stringify(profile));
    return profile;
  }

  getProfile(): IProfile {
    const user = localStorage.getItem(PROFILE_DATA);
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
    window.localStorage.removeItem(PROFILE_DATA);
    window.localStorage.clear();
  }
}
