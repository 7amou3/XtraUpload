
import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { loadTranslations } from '@angular/localize';
import { getTranslations, getBrowserLang } from '@locl/core';
import { DOCUMENT, registerLocaleData } from '@angular/common';
import { ILanguage } from 'app/models';
import { tap } from 'rxjs/operators';
import { UserStorageService } from './user.storage.service';
@Injectable()
export class LanguageService {
    private culture: string;
    constructor(
        private http: HttpClient,
        private userStorage: UserStorageService,
        @Inject(DOCUMENT) private document: Document) { }

    async init(): Promise<void> {
        // check if user already set his language
        const userlang = this.userStorage.userlanguage;
        if (userlang?.culture) {
            this.culture = userlang.culture;
        }
        // Set browser lang if it's supported   
        else {
            var langs = await this.getLanguages();
            const supportedLang = langs.filter(s => s.culture === getBrowserLang())[0];
            if (langs && supportedLang) {
                this.culture = supportedLang.culture;
                this.userStorage.userlanguage = supportedLang;
            }
            // Language is not supported by XtraUpload, set the default lang
            else {
                const defaultLang = langs.filter(s => s.default)[0];
                this.culture = defaultLang.culture;
                this.userStorage.userlanguage = defaultLang;
            }
        }
        this.document.documentElement.lang = this.culture; 
        await this.localeInitializer(this.culture)
        await getTranslations('/assets/i18n/' + this.culture + '.json')
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
        var appLangs = this.userStorage.applanguages;
        if (!appLangs) {
            return this.http.get<ILanguage[]>('setting/languages')
                .pipe(
                    tap(langs => {
                        this.userStorage.applanguages = langs;
                    })
                ).toPromise();
        }
        else return appLangs;
    }
    async updateLanguage(culture: string) {
        return this.http.patch<ILanguage>('setting/language', { culture: culture }).toPromise();
    }

}