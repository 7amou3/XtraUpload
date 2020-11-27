import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared';
import { IUserRoleClaims, IEditProfile, IProfileClaim } from 'app/domain';

@Component({
  selector: 'app-edituser',
  templateUrl: './edituser.component.html'
})
export class EdituserComponent extends ComponentBase implements OnInit {
  editFormGroup: FormGroup;
  userName = new FormControl('', [Validators.required, Validators.minLength(3)]);
  email = new FormControl('', [Validators.required, Validators.email]);
  emailConfirmed = new FormControl();
  suspendAccount = new FormControl();
  newPassword = new FormControl('', [Validators.minLength(6)]);
  selectedGroup = new FormControl();
  hidePassword = true;
  constructor(
    private dialogRef: MatDialogRef<EdituserComponent>,
    private fb: FormBuilder,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) public item: { user: IProfileClaim, groups: IUserRoleClaims[] }
  ) {
    super();
  }
  ngOnInit(): void {
    this.editFormGroup = this.fb.group({
      id: this.item.user.id,
      userName: this.userName,
      email: this.email,
      newPassword: this.newPassword,
      emailConfirmed: this.emailConfirmed,
      suspendAccount: this.suspendAccount,
      selectedGroup: this.selectedGroup
    });
    this.userName.setValue(this.item.user.userName);
    this.email.setValue(this.item.user.email);
    this.emailConfirmed.setValue(this.item.user.emailConfirmed);
    this.suspendAccount.setValue(this.item.user.accountSuspended);
    this.selectedGroup.setValue(this.item.groups.find(s => s.role.name === this.item.user.roleName));
  }
  async onSubmit(user: IEditProfile) {
    user.roleId = this.selectedGroup.value.role.id;
    await this.adminService.updateUser(user)
      .then(() => this.dialogRef.close(user))
      .catch((error) => this.handleError(error));
  }
}
