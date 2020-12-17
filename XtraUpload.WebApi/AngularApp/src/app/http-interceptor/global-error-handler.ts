import { ErrorHandler, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorNotificationService, UserStorageService } from 'app/services';

/** Intercept incomming http error responses and displays them to the user */
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {

  constructor(
    private errorNotifService: ErrorNotificationService,
    private router: Router,
    private userStorage: UserStorageService) { }
  handleError(error: Error | HttpErrorResponse) {

    let message: string;
    if (error instanceof HttpErrorResponse) {
      // Server response error
      message = this.errorNotifService.getServerErrorMessage(error);
      switch (error.status) {
        case 400:
          // Client Bad request, let the front handle this error
          return;
        case 401:
            // Expired token
            this.userStorage.clearLocalStorage();
            this.router.navigate(['/login']);
            break;
        case 403:
          // Forbidden
          break;
            case 404:
          message = $localize`The requested resource was not found:`+ ' ' + error.url;
          break;
        // TBC..
        default :
              break;
      }
      this.errorNotifService.showError(message);
    } else {
      // Client Error
      message = this.errorNotifService.getClientErrorMessage(error);
      this.errorNotifService.showError(message);
    }
    // todo: send error to server
    console.error(error);
  }
}
