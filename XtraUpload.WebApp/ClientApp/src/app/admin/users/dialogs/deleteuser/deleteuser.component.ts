import { Component, OnInit, Inject } from '@angular/core';
import { ComponentBase } from 'app/shared';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AdminService } from 'app/services';
import { finalize, takeUntil } from 'rxjs/operators';
import { IProfile } from 'app/domain';

@Component({
  selector: 'app-deleteuser',
  templateUrl: './deleteuser.component.html'
})
export class DeleteuserComponent extends ComponentBase implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<DeleteuserComponent>,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) public users: IProfile[]
  ) {
    super();
   }

  ngOnInit(): void {
  }
  onDelete() {
    this.isBusy = true;
    this.adminService.deleteUsers(this.users.map(s => s.id))
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
    .subscribe(
      () => {
        this.dialogRef.close(this.users);
      }
    );
  }

}
