import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { IFileExtension } from 'app/models';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared/components';

@Component({
  selector: 'app-delete',
  templateUrl: './delete.component.html'
})
export class DeleteComponent extends ComponentBase implements OnInit {

  constructor(
    private snackBar: MatSnackBar,
    private dialogRef: MatDialogRef<DeleteComponent>,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) public item: IFileExtension
  ) {
    super();
  }

  ngOnInit(): void {
  }
  async onDelete() {
    this.isBusy = true;
    await this.adminService.deleteExtension(this.item)
      .then(() => this.dialogRef.close(this.item))
      .catch((error) => this.handleError(error, this.snackBar))
      .finally(() => this.isBusy = false);
  }
}
