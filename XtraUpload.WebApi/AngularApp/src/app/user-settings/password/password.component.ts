import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { ComponentBase } from 'app/shared';
import { IChangePassword } from 'app/domain';
import { SettingsService } from 'app/services';
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
  async onSubmit(changePassParams: IChangePassword) {
    if (changePassParams.newPassword === changePassParams.oldPassword) {
      this.message$.next({errorMessage: $localize`The new password must differ from current password.`});
      return;
    }
    this.isBusy = true;
    await this.settingsService.changePassword(changePassParams)
    .then(() => {
      this.isBusy = false;
      this.resetForm(this.changePassFormGroup);
      this.message$.next({successMessage: $localize`Your password has been successfully changed.`});
    })
    .catch(error => {
      this.message$.next({errorMessage: error?.error?.errorContent?.message});
      throw error;
    });
  }
}
