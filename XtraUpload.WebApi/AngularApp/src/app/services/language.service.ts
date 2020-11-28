
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ILanguage } from 'app/domain';
import { tap } from 'rxjs/operators';
import { UserStorageService } from './user.storage.service';
import { loadTranslations } from '@angular/localize';
import { getTranslations, getBrowserLang } from '@locl/core';
import { registerLocaleData } from '@angular/common';

@Injectable({ providedIn: 'root' })
export class LanguageService {
    private _locale: string;
    constructor(
        private http: HttpClient,
        private userStorage: UserStorageService) { }

    get locale(): string {
        return this._locale || 'en-US';
    }
    set locale(culture: string) {
        this._locale = culture;
    }
    async init(): Promise<void> {
        // check if user already set his language
        const profile = this.userStorage.getProfile();
        if (profile?.language?.culture) {
            this._locale = profile.language.culture;
        }
        // Set browser lang if it's supported   
        else {
            const browserCulture = getBrowserLang();
            var langs = await this.getLanguages();
            const supportedLang = langs.filter(s => s.culture === browserCulture)[0];
            if (langs && supportedLang) {
                this._locale = browserCulture;
                this.userStorage.setUserLang(supportedLang);
            }
            // Language is not supported by XtraUpload, set the default lang
            else {
                const defaultLang = langs.filter(s => s.default)[0];
                this._locale = defaultLang.culture;
                this.userStorage.setUserLang(defaultLang);
            }
        }
        await this.localeInitializer(this._locale)
        await getTranslations('/assets/i18n/' + this._locale + '.json')
        .then(data => loadTranslations(data.translations));
    }
    private async localeInitializer(culture: string): Promise<any> {
        // DO NOT DELETE webpack magic comments
        // Add the comments bellow if you want to bundle all XtraUpload's supported languages in one file
        /* webpackMode: "lazy-once" */
        /* webpackChunkName: "i18n-base" */
        const module = await import(
            /* webpackInclude: /(de|en|es|fr|it|nl|no|pl|pt-BR|pt|fi|sv|ko|ru|zh|ja|ar)\.js/ */
            `@angular/common/locales/${culture}.js`);
        return registerLocaleData(module.default);
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