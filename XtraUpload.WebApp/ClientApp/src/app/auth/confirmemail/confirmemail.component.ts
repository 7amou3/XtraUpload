import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ComponentBase } from 'app/shared';
import { AuthService, SeoService } from 'app/services';
import { takeUntil, finalize } from 'rxjs/operators';

@Component({
  selector: 'app-confirmemail',
  templateUrl: './confirmemail.component.html',
  styleUrls: ['./confirmemail.component.css']
})
export class ConfirmemailComponent extends ComponentBase implements OnInit {
  private readonly pageTitle = 'Email Confirmation';
  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private seoService: SeoService
  ) {
    super();
    seoService.setPageTitle(this.pageTitle);
  }

  ngOnInit(): void {
    this.isBusy = true;
    const emailtoken = this.route.snapshot.params['emailtoken'];
    this.authService.confirmEmail(emailtoken)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
    .subscribe(
      () => {
        this.message$.next({successMessage: 'Your Email has been confirmed successfully.'});
      },
      (error) => {
        this.message$.next({errorMessage: error?.error?.errorContent?.message});
        throw error;
      }
    );
  }

}
