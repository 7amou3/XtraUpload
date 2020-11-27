import { Component, OnInit } from '@angular/core';
import { ComponentBase } from '../../shared';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { AuthService, SeoService } from 'app/services';

@Component({
  selector: 'app-forgotpwd',
  templateUrl: './forgotpwd.component.html'
})
export class ForgotpwdComponent extends ComponentBase implements OnInit {
  private readonly pageTitle =  $localize`Forgot Password`;
  forgotPassFormGroup: FormGroup;
  email = new FormControl('', [Validators.required, Validators.email]);
  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private seoService: SeoService) {
    super();
    seoService.setPageTitle(this.pageTitle);
  }

  ngOnInit(): void {
    this.forgotPassFormGroup = this.fb.group({
      email: this.email
    });
  }
  getEmailErrorMessage() {
    return this.email.hasError('required') ?  $localize`You must enter a value` :
        this.email.hasError('email') ?  $localize`Not a valid email` : '';
  }
  async onSubmit(lostPwdParams) {
    this.isBusy = true;
    await this.authService.resetPassword(lostPwdParams)
      .then(() => {
          this.resetForm(this.forgotPassFormGroup);
          this.message$.next({successMessage:  $localize`An email has been sent. Please check your inbox`});
        })
        .catch(error => this.message$.next({errorMessage: error?.error?.errorContent?.message}))
        .finally(() => this.isBusy = false);
  }
}
