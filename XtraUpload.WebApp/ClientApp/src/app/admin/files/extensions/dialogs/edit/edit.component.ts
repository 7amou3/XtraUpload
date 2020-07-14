import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IFileExtension, IEditExtension } from 'app/domain';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { ComponentBase } from 'app/shared';
import { AdminService } from 'app/services';
import { takeUntil, finalize } from 'rxjs/operators';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html'
})
export class EditComponent extends ComponentBase implements OnInit {
  editFormGroup: FormGroup;
  newExt = new FormControl('', [Validators.required, Validators.pattern('^[.][a-zA-Z0-9]*$'), Validators.minLength(3)]);
  constructor(
    private dialogRef: MatDialogRef<EditComponent>,
    private fb: FormBuilder,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) public item: { selectedExt: IFileExtension, fullExtList: IFileExtension[] }) {
    super();
  }

  ngOnInit(): void {
    this.editFormGroup = this.fb.group({
      newExt: this.newExt,
    });
  }
  onSubmit(formParams: IEditExtension) {
    if (formParams.newExt === this.item.selectedExt.name) {
      this.newExt.setErrors({ 'isSame': true });
      return;
    }
    if (this.item.fullExtList.filter(s => s.name === formParams.newExt).length > 0) {
      this.newExt.setErrors({ 'itemExists': true });
      return;
    }
    this.isBusy = true;
    formParams.id = this.item.selectedExt.id;
    this.adminService.updateExtension(formParams)
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.isBusy = false))
      .subscribe(
        () => {
          this.dialogRef.close(formParams);
        }, (error) => this.handleError(error)
      );
  }

}
