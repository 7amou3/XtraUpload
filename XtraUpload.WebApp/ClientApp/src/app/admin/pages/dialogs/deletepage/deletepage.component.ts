import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared';
import { takeUntil, finalize } from 'rxjs/operators';
import { IPage } from 'app/domain';

@Component({
  selector: 'app-deletepage',
  templateUrl: './deletepage.component.html'
})
export class DeletepageComponent extends ComponentBase implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<DeletepageComponent>,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) public page: IPage
  ) {
    super();
   }

  ngOnInit(): void {
  }
  onDelete() {
    this.isBusy = true;
    this.adminService.deletePage(this.page)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
    .subscribe(
      () => {
        this.dialogRef.close(this.page);
      }
    );
  }

}
