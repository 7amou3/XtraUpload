import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AdminService } from 'app/services';
import { IFileInfo } from 'app/domain';
import { ComponentBase } from 'app/shared';
import { takeUntil, finalize } from 'rxjs/operators';

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
  onDelete() {
    this.isBusy = true;
    this.adminService.deleteFiles(this.files)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
    .subscribe(
      (result) => {
        this.dialogRef.close(result);
      }
    );
  }

}
