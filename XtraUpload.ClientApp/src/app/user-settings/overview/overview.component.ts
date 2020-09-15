import { Component, OnInit } from '@angular/core';
import { ComponentBase } from 'app/shared';
import { FileManagerService, AuthService } from 'app/services';
import { takeUntil, finalize } from 'rxjs/operators';
import { IAccountOverview } from 'app/domain';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.css']
})
export class OverviewComponent extends ComponentBase implements OnInit {
  accountOverview: IAccountOverview;
  sendingEmail = false;
  constructor(
    private fileMngService: FileManagerService,
    private authService: AuthService
  ) {
    super();
  }

  ngOnInit(): void {
    this.isBusy = true;
    this.fileMngService.getAccountOverview()
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.isBusy = false))
      .subscribe(
        data => {
          this.accountOverview = data;
        },
        (error) => {
          throw Error(error.message);
        }
      );

  }
  verifyEmail() {
    this.sendingEmail = true;
    this.authService.requestConfirmEmail()
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.sendingEmail = false))
      .subscribe(
        () => {
          this.message$.next({successMessage: 'An email has been sent to your inbox, please check your email.' });
        },
        (error) => {
          this.message$.next({errorMessage: error?.error?.errorContent?.message});
          throw error;
        }
      );
  }

}
