import { Component, OnInit, Inject } from '@angular/core';
import { ComponentBase } from 'app/shared';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AdminService } from 'app/services';
import { IProfile } from 'app/domain';

@Component({
  selector: 'app-deleteuser',
  templateUrl: './deleteuser.component.html'
})
export class DeleteuserComponent extends ComponentBase implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<DeleteuserComponent>,
    private adminService: AdminService,
    private snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public users: IProfile[]
  ) {
    super();
  }

  ngOnInit(): void {
  }
  async onDelete() {
    this.isBusy = true;
    this.adminService.deleteUsers(this.users.map(s => s.id))
      .then(() => this.dialogRef.close(this.users))
      .catch((error) => this.handleError(error, this.snackBar))
      .finally(() => this.isBusy = false);
  }
}
