
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ILanguage } from 'app/domain';
import { tap } from 'rxjs/operators';
import { UserStorageService } from './user.storage.service';
import { loadTranslations } from '@angular/localize';
import { getTranslations, getBrowserLang } from '@locl/core';

@Injectable()
export class LanguageService {
    constructor(
        private http: HttpClient,
        private userStorage: UserStorageService) { }

    async init(): Promise<void> {
        let langPath = '/assets/i18n/en.json';
        // check if user already set his language
        const profile = this.userStorage.getProfile();
        if (profile?.language) {
            langPath = '/assets/i18n/' + profile.language.culture + '.json';
        }
        // Set browser lang if it's supported   
        else {
            const browserLocl = getBrowserLang();
            var langs = await this.getLanguages();
            const supportedLang = langs.filter(s => s.culture === browserLocl)[0];
            if (langs && supportedLang) {
                langPath = '/assets/i18n/' + browserLocl + '.json';
                this.userStorage.setUserLang(supportedLang);
            }
            // Language is not supported by XtraUpload, set the default lang
            else {
                const defaultLang = langs.filter(s => s.default)[0];
                langPath = '/assets/i18n/' + defaultLang.culture + '.json';
                this.userStorage.setUserLang(defaultLang);
            }
        }
        
        const data = await getTranslations(langPath);
        loadTranslations(data.translations);
    }
    async getLanguages(): Promise<ILanguage[]> {
        var appLangs = this.userStorage.getAppLanguages();
        if (!appLangs) {
            return this.http.get<ILanguage[]>('setting/languages')
            .pipe(
                tap(l => {
                    this.userStorage.saveAppLanguages(l);
                })
            ).toPromise();
        }
        else return appLangs;
    }
    async updateLanguage(culture: string) {
        return this.http.patch<ILanguage>('setting/language', { culture: culture }).toPromise();
    }
}