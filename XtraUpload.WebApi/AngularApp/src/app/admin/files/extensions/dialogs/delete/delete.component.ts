import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IFileExtension } from 'app/domain';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared';

@Component({
  selector: 'app-delete',
  templateUrl: './delete.component.html'
})
export class DeleteComponent extends ComponentBase implements OnInit {

  constructor(
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
      .catch((error) => this.handleError(error))
      .finally(() => this.isBusy = false);
  }
}
