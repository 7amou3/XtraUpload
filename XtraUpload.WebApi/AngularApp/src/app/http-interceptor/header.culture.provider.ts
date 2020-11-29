import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LanguageService } from 'app/services';
@Injectable()
/** Intercept ongoing http request and set the Accept-Language header field */
export class HeaderCultureProvider implements HttpInterceptor {
    constructor(public langService: LanguageService) {}
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        request = request.clone({
          setHeaders: {
            'Accept-Language': this.langService.locale
          }
        });
        return next.handle(request);
      }
}