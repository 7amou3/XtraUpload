import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { AdminService } from 'app/services';
import { IFileExtension, IEditExtension } from 'app/models';
import { ComponentBase } from 'app/shared/components';
import { MatSnackBar } from '@angular/material/snack-bar';

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
    private snackBar: MatSnackBar,
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

  async onSubmit(formParams: IEditExtension) {
    if (this.item.filter(s => s.name === formParams.newExt).length > 0) {
      this.newExt.setErrors({ 'itemExists': true });
      return;
    }
    this.isBusy = true;
    await this.adminService.addExtension(formParams.newExt)
      .then((extension) => this.dialogRef.close(extension))
      .catch((error) => this.handleError(error, this.snackBar))
      .finally(() => this.isBusy = false)
  }
}
