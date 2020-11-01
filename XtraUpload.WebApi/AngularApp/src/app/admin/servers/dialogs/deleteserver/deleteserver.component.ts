import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IStorageServer } from 'app/domain';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared';
import { finalize, takeUntil } from 'rxjs/operators';

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
  onDelete() {
    this.isBusy = true;
    this.adminService.deleteServer(this.server)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
    .subscribe(
      () => {
        this.dialogRef.close(this.server);
      }, (error) => this.handleError(error)
    );
  }
}
