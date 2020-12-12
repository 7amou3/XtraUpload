import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ComponentBase } from 'app/shared';
import { AuthService, SeoService } from 'app/services';

@Component({
  selector: 'app-confirmemail',
  templateUrl: './confirmemail.component.html'
})
export class ConfirmemailComponent extends ComponentBase implements OnInit {
  private readonly pageTitle = $localize`Email Confirmation`;
  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private seoService: SeoService
  ) {
    super();
    seoService.setPageTitle(this.pageTitle);
  }

  async ngOnInit() {
    this.isBusy = true;
    const emailtoken = this.route.snapshot.params['emailtoken'];
    await this.authService.confirmEmail(emailtoken)
    .then(() => {
      this.message$.next({successMessage: $localize`Your Email has been confirmed successfully.`})
    })
    .catch(error => this.message$.next({errorMessage: error?.error}))
    .finally(() => this.isBusy = false);
  }

}
