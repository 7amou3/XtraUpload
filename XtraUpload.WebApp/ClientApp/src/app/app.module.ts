
import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler, APP_INITIALIZER } from '@angular/core';
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
import { IsLoggedInDirective } from './shared/loggedin.directive';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { AppComponent } from './app.component';
import { FullComponent, HeaderComponent, FooterComponent, PageNotFoundComponent } from './layouts';

import {
  UrlForwarderHandler,
  HttpProgressHandler,
  TokenInterceptor,
  GlobalErrorHandler,
  ProgressComponent
} from './http-interceptor';
import { UserStorageService, AuthService, SettingsService, HeaderService, SidenavService } from 'app/services';
import { SpinnerComponent } from './shared';
import { PipeModule } from './shared/pipe-modules';
export function webSettingFactory(settings: SettingsService) {
  return () => settings.webappconfig().toPromise();
}
@NgModule({
  declarations: [
    AppComponent,
    SpinnerComponent,
    HeaderComponent,
    FullComponent,
    FooterComponent,
    ProgressComponent,
    PageNotFoundComponent,
    IsLoggedInDirective
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
    MatButtonModule,
    MatSnackBarModule,
    HttpClientModule,
    RouterModule.forRoot(AppRoutes, {scrollPositionRestoration: 'enabled'}),
    PipeModule
  ],
  providers: [
    { provide: LocationStrategy, useClass: PathLocationStrategy },
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
    { provide: HTTP_INTERCEPTORS, useClass: UrlForwarderHandler, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: HttpProgressHandler, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true},
    {
      provide: APP_INITIALIZER,
      useFactory: webSettingFactory,
      deps: [SettingsService],
      multi: true
    },
    UserStorageService,
    AuthService,
    SettingsService,
    HeaderService,
    SidenavService
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
