import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IFileExtension } from 'app/domain';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared';
import { takeUntil, finalize } from 'rxjs/operators';

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
  onDelete() {
    this.isBusy = true;
    this.adminService.deleteExtension(this.item)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
    .subscribe(
      () => {
        this.dialogRef.close(this.item);
      }
    );
  }
}
