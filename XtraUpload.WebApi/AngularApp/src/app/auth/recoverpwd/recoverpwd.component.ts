import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { takeUntil, finalize } from 'rxjs/operators';
import { ComponentBase } from 'app/shared';
import { AuthService, SeoService } from 'app/services';
import { FormControl, Validators, FormBuilder, FormGroup } from '@angular/forms';
import { RecoverPassword } from 'app/domain';

@Component({
  selector: 'app-recoverpwd',
  templateUrl: './recoverpwd.component.html'
})
export class RecoverpwdComponent extends ComponentBase implements OnInit {
  private readonly pageTitle = $localize`Recover Password`;
  private recoverPassword = new RecoverPassword();
  recoverPassFormGroup: FormGroup;
  password = new FormControl('', [Validators.required, Validators.minLength(6)]);
  tokenValid = false;
  hidePassword = true;
  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private fb: FormBuilder,
    private seoService: SeoService
  ) {
    super();
    seoService.setPageTitle(this.pageTitle);
  }

  async ngOnInit() {
    const recoveryId = this.route.snapshot.params['recoveryId'];
    this.recoverPassFormGroup = this.fb.group({
      password: this.password
    });
    this.isBusy = true;
    await this.authService.recoverPassInfo(recoveryId)
      .then(() => {
        this.recoverPassword.recoveryKey = recoveryId;
        this.tokenValid = true;
      })
      .catch((error) => this.message$.next({ errorMessage: error?.error?.errorContent?.message }))
      .finally(() => this.isBusy = false);
  }
  async onSubmit(recoverPassParams) {
    this.isBusy = true;
    this.recoverPassword.newPassword = recoverPassParams.password;
    await this.authService.updatePassword(this.recoverPassword)
      .then(() => {
        this.resetForm(this.recoverPassFormGroup);
        this.message$.next({ successMessage: $localize`Your password has been updated successfully, please login.` });
      })
      .catch(error => this.message$.next({ errorMessage: error?.error?.errorContent?.message }))
      .finally(() => this.isBusy = false);
  }
}
