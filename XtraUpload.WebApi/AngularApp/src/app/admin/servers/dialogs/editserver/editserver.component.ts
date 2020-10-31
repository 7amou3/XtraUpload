import { Component, Inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IHardwareOptions, IStorageServer, IUploadOptions } from 'app/domain';
import { AdminService } from 'app/services';
import { finalize, takeUntil } from 'rxjs/operators';
import { ServerDialogBase } from '../server.dialog.base';

@Component({
  selector: 'app-editserver',
  templateUrl: '../server.dialog.component.html',
  styleUrls: ['../server.dialog.component.css']
})
export class EditserverComponent extends ServerDialogBase {

  constructor(
    private dialogRef: MatDialogRef<EditserverComponent>,
    fb: FormBuilder,
    adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) private data: {selectedServer: IStorageServer,  serversList: IStorageServer[]}
  ) {
    super(adminService, fb);
  }

  Init(): void {
    this.dialogTitle = 'Edit Storage Server';
    this.address.setValue(this.data.selectedServer.address);
    const opt = this.serverOptions.filter(s => s.state === this.data.selectedServer.state)[0];
    this.optionControl.setValue(opt);

  }
  onSubmit() {
    if (this.data.serversList.filter(s => s.address === this.address.value && s.id !== this.data.selectedServer.id).length > 0) {
      this.address.setErrors({ 'itemExists': true });
      return;
    }
    this.isBusy = true;
    const server = {
      id: this.data.selectedServer.id,
      storageInfo: this.serverInfoFormGroup.value as IStorageServer,
      uploadOpts: this.uploadOptsFormGroup.value as IUploadOptions,
      hardwareOpts: this.hdOptsFormGroup.value as IHardwareOptions
    };
    // state is an instance of IServerOption
    server.storageInfo.state = (server.storageInfo.state as any).state
    this.adminService.updateStorageServer(server)
      .pipe(takeUntil(this.onDestroy), finalize(() => this.isBusy = false))
      .subscribe((server) => {
        this.dialogRef.close(server);
      }, (error) => this.handleError(error))
  }
}
