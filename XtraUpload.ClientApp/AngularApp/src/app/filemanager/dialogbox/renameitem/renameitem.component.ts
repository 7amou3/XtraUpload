import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IItemInfo, IRenameItemModel } from 'app/domain';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { ComponentBase } from 'app/shared';

@Component({
  selector: 'app-renameitem',
  templateUrl: './renameitem.component.html'
})
export class RenameItemComponent extends ComponentBase implements OnInit {
  renameFormGroup: FormGroup;
  newName = new FormControl('', [Validators.required, Validators.minLength(4)]);
  constructor(
    public dialogRef: MatDialogRef<RenameItemComponent>,
    private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public item: IItemInfo) {
    super();
  }

  ngOnInit(): void {
    this.renameFormGroup = this.fb.group({
      newName: this.newName,
    });
  }

  onSubmit(formParams: IRenameItemModel) {
    if (formParams.newName === this.item.name) {
      this.newName.setErrors({ 'isSame': true });
      return;
    }
    formParams.fileId = this.item.id;
    this.dialogRef.close(formParams);
  }
}

