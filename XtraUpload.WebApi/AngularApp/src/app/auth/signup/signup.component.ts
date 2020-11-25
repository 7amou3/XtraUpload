import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ComponentBase } from 'app/shared';
import { takeUntil, finalize } from 'rxjs/operators';
import { ISignupParams } from 'app/domain';
import { AuthService, SeoService, UserStorageService } from 'app/services';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent extends ComponentBase implements OnInit {
  private readonly pageTitle = $localize`Signup`;
  signupFormGroup: FormGroup;
  userName = new FormControl('', [Validators.required, Validators.minLength(4)]);
  email = new FormControl('', [Validators.required, Validators.email]);
  password = new FormControl('', [Validators.required, Validators.minLength(6)]);
  termsOfService = new FormControl(false, [Validators.requiredTrue]);
  hidePassword = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private userStorage: UserStorageService,
    private seoService: SeoService) {
    super();
    seoService.setPageTitle(this.pageTitle);
  }
  ngOnInit(): void {
    this.signupFormGroup = this.fb.group({
      email: this.email,
      userName: this.userName,
      password: this.password,
      termsOfService : this.termsOfService
    });
  }
  getEmailErrorMessage() {
    return this.email.hasError('required') ? $localize`You must enter a value` :
        this.email.hasError('email') ? $localize`Not a valid email` : '';
  }
  onSubmit(signupParams: ISignupParams) {
    this.isBusy = true;
    signupParams.language = this.userStorage.getUserLang();
    this.authService.requestSignup(signupParams)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
    .subscribe(
      () => {
        this.message$.next({successMessage: $localize`Account successfully created, please log in.`});
        this.resetForm(this.signupFormGroup);
      },
      error => {
        this.message$.next({errorMessage: error?.error?.errorContent?.message});
        throw error;
      }
    );
  }
}
