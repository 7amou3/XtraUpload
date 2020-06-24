import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { IItemInfo, ISetPasswordItemModel } from 'app/domain';
import { ComponentBase } from 'app/shared';

@Component({
  selector: 'app-password',
  templateUrl: './password.component.html'
})
export class PasswordComponent extends ComponentBase implements OnInit {
  passwordFormGroup: FormGroup;
  password = new FormControl('', [Validators.required, Validators.minLength(4)]);
  hidePassword = true;
  constructor(
    public dialogRef: MatDialogRef<PasswordComponent>,
    private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public item: IItemInfo) {
    super();
  }

  ngOnInit(): void {
    this.passwordFormGroup = this.fb.group({
      password: this.password,
    });
  }

  onSubmit(formParams: ISetPasswordItemModel) {
    this.dialogRef.close(formParams);
  }
}
