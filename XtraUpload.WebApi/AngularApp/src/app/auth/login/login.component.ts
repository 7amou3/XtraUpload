import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService, SeoService } from 'app/services';
import { ILoginParams, IGenericMessage } from 'app/domain';
import { ComponentBase } from 'app/shared';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent extends ComponentBase implements OnInit {
  private readonly pageTitle = $localize`Login to your account`;
  loginFormGroup: FormGroup;
  email = new FormControl('', [Validators.required, Validators.email]);
  password = new FormControl('', [Validators.required, Validators.minLength(6)]);
  hidePassword = true;
  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private seoService: SeoService) {
    super();
    seoService.setPageTitle(this.pageTitle);
  }
  ngOnInit(): void {
    this.loginFormGroup = this.fb.group({
      email: this.email,
      password: this.password,
      rememberMe: false
    });
  }
  getEmailErrorMessage() {
    return this.email.hasError('required') ? $localize`You must enter a value` :
      this.email.hasError('email') ? $localize`Not a valid email` : '';
  }
  async onSubmit(loginParams: ILoginParams) {
    this.isBusy = true;
    await this.authService.requestLogin(loginParams)
      .then((data) => {
        // Reload the entire app
        window.location.href = data.role === 'Admin' ? '/administration' : '/filemanager';
      })
      .catch((error) => this.message$.next({ errorMessage: error?.error?.errorContent?.message }));
      //.finally(() => this.isBusy = false);
  }
  onSMMessage(message: IGenericMessage) {
    this.message$.next(message);
  }
}
