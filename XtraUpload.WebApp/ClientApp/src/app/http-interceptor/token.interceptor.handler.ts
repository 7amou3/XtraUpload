import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { UserStorageService } from '../services/user.storage.service';
import { Observable } from 'rxjs/Observable';
@Injectable()

/** Intercept ongoing http request and attach a jwt token as a bearer */
export class TokenInterceptor implements HttpInterceptor {
  constructor(public storageService: UserStorageService) {}
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${this.storageService.getToken()}`
      }
    });
    return next.handle(request);
  }
}
