import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { loadTranslations } from '@angular/localize';
import { environment } from './environments/environment';
import { getTranslations, getBrowserCultureLang, ParsedTranslationBundle } from '@locl/core';
import { IProfile } from 'app/domain';
export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}
export function getAPIUrl() {
  return getBaseUrl() + 'api/';
}
const providers = [
  { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
  { provide: 'API_URL', useFactory: getAPIUrl , deps: []}
];

if (environment.production) {
  enableProdMode();
}

let langPath = '/assets/i18n/en.json';
const userItem = localStorage.getItem('xu-Profile');
if (userItem) {
  var user: IProfile = JSON.parse(userItem)
  if (user.language) {
    langPath = '/assets/i18n/' + user.language + '.json';
  }
}

getTranslations(langPath).then(
  (data: ParsedTranslationBundle) => {
    loadTranslations(data.translations);
    import('./app/app.module').then(module => {
      platformBrowserDynamic(providers).bootstrapModule(module.AppModule)
        .catch(err => console.error(err));
    });
  }
);
