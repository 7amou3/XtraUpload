import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ProgressNotificationService } from 'app/services';

@Injectable()
export class HttpProgressHandler implements HttpInterceptor {

    constructor(@Inject(PLATFORM_ID) private platformId: Object, private progressService: ProgressNotificationService) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

      if (!isPlatformBrowser(this.platformId)) {
        return next.handle(req);
      }
      if (!req.reportProgress) {
        return next.handle(req);
      }

      return next.handle(req).pipe(
        tap((resp) => {
          switch (resp.type) {
            case HttpEventType.Sent:
              this.progressService.setProgress({ status: 'Started', currentProgress: 0 });
              break;

            case HttpEventType.DownloadProgress:
              const percentDone = Math.round(100 * resp.loaded / resp.total);
              this.progressService.setProgress({ status: 'InProgress', currentProgress: percentDone });
              break;

            case HttpEventType.Response:
              this.progressService.setProgress({ status: 'Completed', currentProgress: 100 });
                break;

          }
        },
         () => {
          this.progressService.setProgress({ status: 'Error', currentProgress: 100 });
         })
      );
    }

}
