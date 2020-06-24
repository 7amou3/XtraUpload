import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { takeUntil, finalize } from 'rxjs/operators';
import { ComponentBase } from 'app/shared';
import { AuthService } from 'app/services';
import { FormControl, Validators, FormBuilder, FormGroup } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { RecoverPassword } from 'app/domain';

@Component({
  selector: 'app-recoverpwd',
  templateUrl: './recoverpwd.component.html',
  styleUrls: ['./recoverpwd.component.css']
})
export class RecoverpwdComponent extends ComponentBase implements OnInit {
  private readonly pageTitle = 'Recover Password';
  private recoverPassword = new RecoverPassword();
  recoverPassFormGroup: FormGroup;
  password = new FormControl('', [Validators.required, Validators.minLength(6)]);
  tokenValid = false;
  hidePassword = true;
  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private fb: FormBuilder,
    private titleService: Title
  ) {
    super();
    titleService.setTitle(this.pageTitle);
   }

  ngOnInit(): void {
    const recoveryId = this.route.snapshot.params['recoveryId'];
    this.recoverPassFormGroup = this.fb.group({
      password: this.password
    });
    this.isBusy = true;
    this.authService.recoverPassInfo(recoveryId)
    .pipe(takeUntil(this.onDestroy), finalize(() => this.isBusy = false))
    .subscribe(
      () => {
        this.recoverPassword.recoveryKey = recoveryId;
        this.tokenValid = true;
      },
      (error) => {
        this.message$.next({errorMessage: error?.error?.errorContent?.message});
      }
    );
  }
  onSubmit(recoverPassParams) {
    this.isBusy = true;
    this.recoverPassword.newPassword = recoverPassParams.password;
    this.authService.updatePassword(this.recoverPassword)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
    .subscribe(
      () => {
        this.resetForm(this.recoverPassFormGroup);
        this.message$.next({successMessage: 'Your password has been updated successfully, please login.'});
      },
      error => {
        this.message$.next({errorMessage: error?.error?.errorContent?.message});
      }
    );
  }
}
