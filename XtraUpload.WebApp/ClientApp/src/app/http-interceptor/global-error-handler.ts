import { ErrorHandler, Injectable, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorNotificationService, UserStorageService } from '../services';


/** Intercept incomming http error responses and displays them to the user */
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {

  constructor(private injector: Injector) { }
  handleError(error: Error | HttpErrorResponse) {
    const notifier = this.injector.get(ErrorNotificationService);

    let message: string;
    if (error instanceof HttpErrorResponse) {
      // Server response error
      message = notifier.getServerErrorMessage(error);
      switch (error.status) {
        case 400:
          // Client Bad request, let the front handle this error
          return;
        case 401:
            // Expired token
            const router = this.injector.get(Router);
            const userStorage = this.injector.get(UserStorageService);
            userStorage.clearLocalStorage();
            router.navigate(['/login']);
            break;
        case 403:
          // Forbidden
          break;
            case 404:
          message = 'The requested resource was not found: ' + error.url;
          break;
        // TBC..
        default :
              break;
      }
      notifier.showError(message);
    } else {
      // Client Error
      message = notifier.getClientErrorMessage(error);
      notifier.showError(message);
    }
    // todo: send error to server
    console.error(error);
  }
}
