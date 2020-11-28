import { Component, Inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AdminService } from 'app/services';
import { IPage } from 'app/domain';
import { PageCommon } from '../page.common';

@Component({
  selector: 'app-addpage',
  templateUrl: './addpage.component.html'
})
export class AddpageComponent extends PageCommon {

  constructor(
    private dialogRef: MatDialogRef<AddpageComponent>,
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) private fullPageList: IPage[]
  ) {
    super();
  }

  Init(): void {
    this.pageFormGroup = this.fb.group({
      name: this.name,
      content: this.content,
      visibleInFooter: this.visibleInFooter
    });
    this.visibleInFooter.setValue(true)
  }
  async onSubmit(formParams: IPage) {
    if (this.fullPageList.filter(s => s.name === formParams.name).length > 0) {
      this.name.setErrors({ 'itemExists': true });
      return;
    }
    this.isBusy = true;
    await this.adminService.addPage(formParams)
      .then(newPage => this.dialogRef.close(newPage))
      .catch(error => this.handleError(error, this.snackBar))
      .finally(() => this.isBusy = false);
  }
}
