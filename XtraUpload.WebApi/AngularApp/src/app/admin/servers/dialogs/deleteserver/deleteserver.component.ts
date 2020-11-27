import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IStorageServer } from 'app/domain';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared';

@Component({
  selector: 'app-deleteserver',
  templateUrl: './deleteserver.component.html'
})
export class DeleteserverComponent extends ComponentBase implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<DeleteserverComponent>,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) public server: IStorageServer
  ) {
    super();
  }

  ngOnInit(): void {
  }
  async onDelete() {
    this.isBusy = true;
    await this.adminService.deleteServer(this.server)
      .then(() => this.dialogRef.close(this.server))
      .catch(error => this.handleError(error))
      .finally(() => this.isBusy = false);
  }
}
