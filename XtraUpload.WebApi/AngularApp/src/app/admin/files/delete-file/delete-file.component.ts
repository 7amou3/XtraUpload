import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AdminService } from 'app/services';
import { IFileInfo } from 'app/domain';
import { ComponentBase } from 'app/shared';

@Component({
  selector: 'app-delete-file',
  templateUrl: './delete-file.component.html'
})
export class DeleteFileComponent extends ComponentBase implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<DeleteFileComponent>,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) public files: IFileInfo[]
  ) {
    super();
  }
  ngOnInit(): void {
  }
  async onDelete() {
    this.isBusy = true;
    await this.adminService.deleteFiles(this.files)
      .then((result) => this.dialogRef.close(result))
      .catch((error) => this.handleError(error))
      .finally(() => this.isBusy = false);
  }

}
