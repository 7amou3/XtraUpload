import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IChangePassword } from '../domain';
import { Subject } from 'rxjs';

@Injectable()
export class SettingsService {
  constructor(private http: HttpClient) { }
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
