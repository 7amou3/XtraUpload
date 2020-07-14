import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AdminService } from 'app/services';
import { IPage } from 'app/domain';
import { ComponentBase } from 'app/shared';
import { takeUntil, finalize } from 'rxjs/operators';

@Component({
  selector: 'app-editpage',
  templateUrl: './editpage.component.html'
})
export class EditpageComponent extends ComponentBase  implements OnInit {
  editFormGroup: FormGroup;
  name = new FormControl('', [Validators.required, Validators.minLength(3)]);
  content = new FormControl('', [Validators.required]);
  constructor(
    private dialogRef: MatDialogRef<EditpageComponent>,
    private fb: FormBuilder,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) public item: { selectedPage: IPage, fullPageList: IPage[] }
  ) {
    super();
  }

  ngOnInit(): void {
    this.editFormGroup = this.fb.group({
      id:  this.item.selectedPage.id,
      name: this.name,
      content: this.content
    });
    this.name.setValue(this.item.selectedPage.name);
    this.content.setValue(this.item.selectedPage.content);
  }
  onSubmit(formParams: IPage) {
    if (this.item.fullPageList.filter(s => s.name === formParams.name && s.id !== formParams.id).length > 0) {
      this.name.setErrors({ 'itemExists': true });
      return;
    }
    this.isBusy = true;
    this.adminService.updatePage(formParams)
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.isBusy = false))
      .subscribe(
        (page) => {
          this.dialogRef.close(page);
        }, (error) => this.handleError(error)
      );
  }

}
