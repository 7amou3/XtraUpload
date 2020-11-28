import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared';
import { IPage } from 'app/domain';

@Component({
  selector: 'app-deletepage',
  templateUrl: './deletepage.component.html'
})
export class DeletepageComponent extends ComponentBase {

  constructor(
    private dialogRef: MatDialogRef<DeletepageComponent>,
    private snackBar: MatSnackBar,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) public page: IPage
  ) {
    super();
  }

  async onDelete() {
    this.isBusy = true;
    this.adminService.deletePage(this.page)
      .then(() => this.dialogRef.close(this.page))
      .catch(error => this.handleError(error, this.snackBar))
      .finally(() => this.isBusy = false);
  }

}
