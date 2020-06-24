import { Component, OnInit } from '@angular/core';
import { ComponentBase } from '../../shared';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { AuthService } from 'app/services';
import { takeUntil, finalize } from 'rxjs/operators';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-forgotpwd',
  templateUrl: './forgotpwd.component.html',
  styleUrls: ['./forgotpwd.component.css']
})
export class ForgotpwdComponent extends ComponentBase implements OnInit {
  private readonly pageTitle = 'Forgot Password';
  forgotPassFormGroup: FormGroup;
  email = new FormControl('', [Validators.required, Validators.email]);
  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private titleService: Title) {
    super();
    titleService.setTitle(this.pageTitle);
  }

  ngOnInit(): void {
    this.forgotPassFormGroup = this.fb.group({
      email: this.email
    });
  }
  getEmailErrorMessage() {
    return this.email.hasError('required') ? 'You must enter a value' :
        this.email.hasError('email') ? 'Not a valid email' : '';
  }
  onSubmit(lostPwdParams) {
    this.isBusy = true;
    this.authService.resetPassword(lostPwdParams)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
      .subscribe(
        () => {
          this.resetForm(this.forgotPassFormGroup);
          this.message$.next({successMessage: 'An email has been sent. Please check your inbox'});
        },
        (error) => {
          this.message$.next({errorMessage: error?.error?.errorContent?.message});
        }
      );
  }
}
