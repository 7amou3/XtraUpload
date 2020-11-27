import { Component, OnInit } from '@angular/core';
import { ComponentBase } from 'app/shared';
import { FileManagerService, AuthService } from 'app/services';
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

  async ngOnInit() {
    this.isBusy = true;
    await this.fileMngService.getAccountOverview()
      .then(data => this.accountOverview = data)
      .catch((error) => this.message$.next({ errorMessage: error?.error?.errorContent?.message }))
      .finally(() => this.isBusy = false);
  }
  async verifyEmail() {
    this.sendingEmail = true;
    await this.authService.requestConfirmEmail()
      .then(() => {
        this.message$.next({ successMessage: 'An email has been sent to your inbox, please check your email.' });
      })
      .catch((error) => this.message$.next({ errorMessage: error?.error?.errorContent?.message }))
      .finally(() => this.sendingEmail = false);
  }
}
