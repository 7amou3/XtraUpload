import { Component, Inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { IHardwareOptions, IStorageServer, IUploadOptions } from 'app/models';
import { AdminService } from 'app/services';
import { ServerDialogBase } from '../server.dialog.base';

@Component({
  selector: 'app-editserver',
  templateUrl: '../server.dialog.component.html',
  styleUrls: ['../server.dialog.component.css']
})
export class EditserverComponent extends ServerDialogBase {

  constructor(
    private dialogRef: MatDialogRef<EditserverComponent>,
    private snackBar: MatSnackBar,
    fb: FormBuilder,
    adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) private data: { selectedServer: IStorageServer, serversList: IStorageServer[] }
  ) {
    super(adminService, fb);
  }

  Init(): void {
    this.dialogTitle = $localize`Edit Storage Server`;
    this.address.setValue(this.data.selectedServer.address);
    const opt = this.serverOptions.filter(s => s.state === this.data.selectedServer.state)[0];
    this.optionControl.setValue(opt);

  }
  async onSubmit() {
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
    await this.adminService.updateStorageServer(server)
      .then((server) => this.dialogRef.close(server))
      .catch(error => this.handleError(error, this.snackBar))
      .finally(() => this.isBusy = false)
  }
}
