
import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler, APP_INITIALIZER } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { LocationStrategy, PathLocationStrategy } from '@angular/common';
import { AppRoutes } from './app.routing';
import { FlexLayoutModule } from '@angular/flex-layout';
import { Angulartics2Module } from 'angulartics2';
import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { ContainerComponent, HeaderComponent, LanguagesComponent } from './layout';

import {
  UrlForwarderHandler,
  HttpProgressHandler,
  TokenInterceptor,
  GlobalErrorHandler,
  ProgressComponent,
  HeaderCultureProvider
} from './http-interceptor';
import { UserStorageService, AuthService, SettingsService, HeaderService, SidenavService, CustomIconService, LanguageService } from 'app/services';

export function languageFactory(lang: LanguageService) {
  return () => lang.init();
}
export function webSettingFactory(settings: SettingsService) {
  return () => settings.appInitializerConfig();
}
export function loadIcons(iconService: CustomIconService) {
  return () => iconService.init();
}
@NgModule({
  declarations: [
    AppComponent,
    ProgressComponent,
    ContainerComponent,
    HeaderComponent,
    LanguagesComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    FlexLayoutModule,
    HttpClientModule,
    RouterModule.forRoot(AppRoutes, { scrollPositionRestoration: 'enabled', relativeLinkResolution: 'legacy' }),
    Angulartics2Module.forRoot(),
    SharedModule
  ],
  providers: [
    { provide: LocationStrategy, useClass: PathLocationStrategy },
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
    { provide: HTTP_INTERCEPTORS, useClass: UrlForwarderHandler, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: HttpProgressHandler, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true},
    { provide: HTTP_INTERCEPTORS, useClass: HeaderCultureProvider, multi: true},
    {
      provide: APP_INITIALIZER,
      useFactory: languageFactory,
      deps: [LanguageService],
      multi: true
    },
    {
      provide: APP_INITIALIZER,
      useFactory: webSettingFactory,
      deps: [SettingsService],
      multi: true
    },
    {
      provide: APP_INITIALIZER,
      useFactory: loadIcons,
      deps: [CustomIconService],
      multi: true
    },
    UserStorageService,
    AuthService,
    SettingsService,
    HeaderService,
    SidenavService,
    CustomIconService,
    LanguageService
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
