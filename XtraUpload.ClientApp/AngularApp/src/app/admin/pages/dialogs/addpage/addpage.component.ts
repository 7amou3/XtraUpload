import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AdminService } from 'app/services';
import { IPage } from 'app/domain';
import { ComponentBase } from 'app/shared';
import { takeUntil, finalize } from 'rxjs/operators';

@Component({
  selector: 'app-addpage',
  templateUrl: './addpage.component.html'
})
export class AddpageComponent extends ComponentBase implements OnInit {

  addFormGroup: FormGroup;
  name = new FormControl('', [Validators.required, Validators.minLength(3)]);
  content = new FormControl('', [Validators.required]);
  constructor(
    private dialogRef: MatDialogRef<AddpageComponent>,
    private fb: FormBuilder,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) private fullPageList: IPage[]
  ) {
    super();
  }

  ngOnInit(): void {
    this.addFormGroup = this.fb.group({
      name: this.name,
      content: this.content
    });
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
