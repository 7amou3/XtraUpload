import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { IItemInfo, ICreateFolderModel, IFolderInfo } from '../../../domain';
import { ComponentBase } from 'app/shared';

@Component({
  selector: 'app-createfolder',
  templateUrl: './createfolder.component.html'
})
export class CreatefolderComponent extends ComponentBase implements OnInit {
  createFormGroup: FormGroup;
  folderName = new FormControl('', [Validators.required, Validators.minLength(4)]);
  constructor(
    public dialogRef: MatDialogRef<CreatefolderComponent>,
    private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public item: IFolderInfo) {
      super();
   }

  ngOnInit(): void {
    this.createFormGroup = this.fb.group({
      folderName: this.folderName,
    });
  }
  onSubmit(formParams: ICreateFolderModel) {
    formParams.parentFolder = this.item;
    this.dialogRef.close(formParams);
  }
}
