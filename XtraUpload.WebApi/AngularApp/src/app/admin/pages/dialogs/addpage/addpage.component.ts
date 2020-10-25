import { Component, Inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AdminService } from 'app/services';
import { IPage } from 'app/domain';
import { takeUntil, finalize } from 'rxjs/operators';
import { PageCommon } from '../page.common';

@Component({
  selector: 'app-addpage',
  templateUrl: './addpage.component.html'
})
export class AddpageComponent extends PageCommon {

  constructor(
    private dialogRef: MatDialogRef<AddpageComponent>,
    private fb: FormBuilder,
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
  onSubmit(formParams: IPage) {
    if (this.fullPageList.filter(s => s.name === formParams.name).length > 0) {
      this.name.setErrors({ 'itemExists': true });
      return;
    }
    this.isBusy = true;
    this.adminService.addPage(formParams)
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.isBusy = false))
      .subscribe(
        (newPage) => {
          this.dialogRef.close(newPage);
        }, (error) => this.handleError(error)
      );
  }
}
