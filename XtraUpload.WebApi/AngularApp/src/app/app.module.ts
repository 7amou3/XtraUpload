
import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler, APP_INITIALIZER, LOCALE_ID } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { LocationStrategy, PathLocationStrategy } from '@angular/common';
import { AppRoutes } from './app.routing';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatDialogModule } from '@angular/material/dialog';
import { IsLoggedInDirective } from './shared/loggedin.directive';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { Angulartics2Module } from 'angulartics2';
import { AppComponent } from './app.component';
import { FooterModule } from './shared';
import { FullComponent, HeaderComponent, LanguagesComponent, PageNotFoundComponent } from './layouts';

import {
  UrlForwarderHandler,
  HttpProgressHandler,
  TokenInterceptor,
  GlobalErrorHandler,
  ProgressComponent,
  HeaderCultureProvider
} from './http-interceptor';
import { UserStorageService, AuthService, SettingsService, HeaderService, SidenavService, CustomIconService, LanguageService } from 'app/services';
import { SpinnerComponent } from './shared';
import { PipeModule } from './shared/pipe-modules';
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
    SpinnerComponent,
    HeaderComponent,
    FullComponent,
    ProgressComponent,
    PageNotFoundComponent,
    IsLoggedInDirective,
    LanguagesComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    FlexLayoutModule,
    MatToolbarModule,
    MatCardModule,
    MatDividerModule,
    MatMenuModule,
    MatIconModule,
    MatListModule,
    MatDialogModule,
    MatButtonModule,
    MatSnackBarModule,
    HttpClientModule,
    RouterModule.forRoot(AppRoutes, { scrollPositionRestoration: 'enabled', relativeLinkResolution: 'legacy' }),
    Angulartics2Module.forRoot(),
    PipeModule,
    FooterModule
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
