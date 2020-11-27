import { Component, Inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AdminService } from 'app/services';
import { IPage } from 'app/domain';
import { PageCommon } from '../page.common';

@Component({
  selector: 'app-editpage',
  templateUrl: './editpage.component.html'
})
export class EditpageComponent extends PageCommon {

  constructor(
    private dialogRef: MatDialogRef<EditpageComponent>,
    private adminService: AdminService,
    private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) private item: { selectedPage: IPage, fullPageList: IPage[] }
  ) {
    super();
  }

  async Init() {
    this.pageFormGroup = this.fb.group({
      id: this.item.selectedPage.id,
      name: this.name,
      content: this.content,
      visibleInFooter: this.visibleInFooter
    });
    this.isBusy = true;
    await this.adminService.getPage(this.item.selectedPage.url)
      .then(page => {
        this.name.setValue(page.name);
        this.content.setValue(page.content);
        this.content.setValue(page.content);
        this.visibleInFooter.setValue(page.visibleInFooter)
      })
      .finally(() => this.isBusy = false)
  }
  async onSubmit(formParams: IPage) {
    if (this.item.fullPageList.filter(s => s.name === formParams.name && s.id !== formParams.id).length > 0) {
      this.name.setErrors({ 'itemExists': true });
      return;
    }
    this.isBusy = true;
    this.adminService.updatePage(formParams)
      .then(page => this.dialogRef.close(page))
      .catch(error => this.handleError(error))
      .finally(() => this.isBusy = false);
  }

}
