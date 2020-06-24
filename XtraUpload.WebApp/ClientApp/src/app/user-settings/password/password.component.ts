import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, FormGroupDirective } from '@angular/forms';
import { ComponentBase } from 'app/shared';
import { IChangePassword } from 'app/domain';
import { SettingsService } from 'app/services';
import { takeUntil, finalize } from 'rxjs/operators';

@Component({
  selector: 'app-password',
  templateUrl: './password.component.html',
  styleUrls: ['./password.component.css']
})
export class ChangePasswordComponent extends ComponentBase implements OnInit {
  changePassFormGroup: FormGroup;
  oldPassword = new FormControl('', [Validators.required, Validators.minLength(6)]);
  newPassword = new FormControl('', [Validators.required, Validators.minLength(6)]);
  hideOldPass = true;
  hideNewPass = true;
  constructor(
    private fb: FormBuilder,
    private settingsService: SettingsService) {
    super();
  }
  ngOnInit(): void {
    this.changePassFormGroup = this.fb.group({
      oldPassword: this.oldPassword,
      newPassword: this.newPassword
    });
  }
  onSubmit(changePassParams: IChangePassword) {
    if (changePassParams.newPassword === changePassParams.oldPassword) {
      this.message$.next({errorMessage: 'The new password must differ from current password.'});
      return;
    }
    this.isBusy = true;
    this.settingsService.changePassword(changePassParams)
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.isBusy = false))
      .subscribe(
        () => {
          this.resetForm(this.changePassFormGroup);
          this.message$.next({successMessage: 'your password has been successfully changed.'});
        },
        (error) => {
          this.message$.next({errorMessage: error?.error?.errorContent?.message});
          throw error;
        });
  }
}
