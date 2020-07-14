import { Component, OnInit, Inject } from '@angular/core';
import { ComponentBase } from 'app/shared';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AdminService } from 'app/services';
import { finalize, takeUntil } from 'rxjs/operators';
import { IUserRoleClaims } from 'app/domain';

@Component({
  selector: 'app-deletegroup',
  templateUrl: './deletegroup.component.html'
})
export class DeletegroupComponent extends ComponentBase implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<DeletegroupComponent>,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) public item: { selectedGroup: IUserRoleClaims, fullGroupList: IUserRoleClaims[] }
  ) {
    super();
   }

  ngOnInit(): void {
  }
  onDelete() {
    console.log(this.item);
    this.isBusy = true;
    this.adminService.deleteGroup(this.item.selectedGroup.role.id)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
    .subscribe(
      () => {
        this.dialogRef.close(this.item);
      }, (error) => this.handleError(error)
    );
  }

}
