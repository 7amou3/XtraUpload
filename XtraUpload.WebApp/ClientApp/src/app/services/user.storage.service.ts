import { Injectable } from '@angular/core';
import { IProfile } from '../domain';
const PROFILE_DATA = 'XtraUpload';
/**
 *  Store usefull user data to localstorage
 * */
@Injectable()
export class UserStorageService {
  constructor() { }

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
    console.log(profile.theme)
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
