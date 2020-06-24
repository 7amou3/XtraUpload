import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { AdminService } from 'app/services';
import { IFileExtension, IEditExtension } from 'app/domain';
import { ComponentBase } from 'app/shared';
import { takeUntil, finalize } from 'rxjs/operators';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html'
})
export class AddComponent extends ComponentBase implements OnInit {
  addFormGroup: FormGroup;
  newExt = new FormControl('', [Validators.required, Validators.pattern('^[.][a-zA-Z0-9]*$'), Validators.maxLength(8)]);
  constructor(
    private dialogRef: MatDialogRef<AddComponent>,
    private fb: FormBuilder,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) private item: IFileExtension[]
  ) {
    super();
  }

  ngOnInit(): void {
    this.addFormGroup = this.fb.group({
      newExt: this.newExt,
    });
  }

  onSubmit(formParams: IEditExtension) {
    if (this.item.filter(s => s.name === formParams.newExt).length > 0) {
      this.newExt.setErrors({ 'itemExists': true });
      return;
    }
    this.isBusy = true;
    this.adminService.addExtension(formParams.newExt)
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.isBusy = false))
      .subscribe(
        (extension) => {
          this.dialogRef.close(extension);
        }
      );
  }
}
