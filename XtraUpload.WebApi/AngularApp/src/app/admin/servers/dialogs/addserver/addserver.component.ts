import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IHardwareOptions, IStorageServer, IUploadOptions } from 'app/domain';
import { AdminService } from 'app/services';
import { finalize, takeUntil } from 'rxjs/operators';
import { ServerDialogBase } from '../server.dialog.base';

@Component({
  selector: 'app-addserver',
  templateUrl: '../server.dialog.component.html',
  styleUrls: ['../server.dialog.component.css'],
})

export class AddserverComponent extends ServerDialogBase implements OnInit {
  constructor(
    private dialogRef: MatDialogRef<AddserverComponent>,
    fb: FormBuilder,
    adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) private serversList: IStorageServer[]
  ) {
    super(adminService, fb);
  }

  Init(): void {
    this.dialogTitle = 'Add Storage Server';
  }
  onSubmit() {
    if (this.serversList.filter(s => s.address === this.address.value).length > 0) {
      this.address.setErrors({ 'itemExists': true });
      return;
    }
    this.isBusy = true;
    const addStorageServer = {
      storageInfo: this.serverInfoFormGroup.value as IStorageServer,
      uploadOpts: this.uploadOptsFormGroup.value as IUploadOptions,
      hardwareOpts: this.hdOptsFormGroup.value as IHardwareOptions
    };
    // state is an instance of IServerOption
    addStorageServer.storageInfo.state = (addStorageServer.storageInfo.state as any).state
    this.adminService.addStorageServer(addStorageServer)
      .pipe(takeUntil(this.onDestroy), finalize(() => this.isBusy = false))
      .subscribe((server) => {
        this.dialogRef.close(server);
      }, (error) => this.handleError(error))
  }
}
