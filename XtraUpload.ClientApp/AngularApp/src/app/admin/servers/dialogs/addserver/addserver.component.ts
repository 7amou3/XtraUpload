import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IHardwareOptions, IStorageServer, IUploadOptions, serverState } from 'app/domain';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared';
import { finalize, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-addserver',
  templateUrl: './addserver.component.html',
  styleUrls: ['./addserver.component.css'],
})

export class AddserverComponent extends ComponentBase implements OnInit {
  serverInfoFormGroup: FormGroup;
  uploadOptsFormGroup: FormGroup;
  hdOptsFormGroup: FormGroup;
  checkingConnectivity = false;
  addressReachable = false;
  /** must be https, allowed ip or host name */
  address = new FormControl('https://', [Validators.required, /*Validators.pattern(/^https:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&\=]*)/)*/]);
  optionControl = new FormControl({value: '', disabled: true}, Validators.required);
  uploadPath = new FormControl({value: '', disabled: true}, [Validators.required]);
  chunkSize = new FormControl({value: '', disabled: true}, [Validators.required, Validators.min(1)]);
  expiration = new FormControl({value: '', disabled: true}, [Validators.required, Validators.min(1)]);
  memoryThreshold = new FormControl({value: '', disabled: true}, [Validators.required, Validators.min(1)]);
  storageThreshold = new FormControl({value: '', disabled: true}, [Validators.required, Validators.min(1)]);
  serverOptions: IServerOption[] = [
    { name: 'Active', state: serverState.Active, hint: 'Uploads and downloads are enabled'},
    { name: 'Passive', state: serverState.Passive, hint: 'Downloads are available, uploads are disabled'},
    { name: 'Disabled',state: serverState.Disabled,  hint: 'Server is reachable, but no uploads/downloads are allowed'}
  ]
  constructor(
    private dialogRef: MatDialogRef<AddserverComponent>,
    private fb: FormBuilder,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) private serversList: IStorageServer[]
  ) {
    super();
  }

  ngOnInit(): void {
    this.serverInfoFormGroup = this.fb.group({
      address: this.address,
      state: this.optionControl,
    });
    this.uploadOptsFormGroup = this.fb.group({
      uploadPath: this.uploadPath,
      chunkSize: this.chunkSize,
      expiration: this.expiration
    });
    this.hdOptsFormGroup = this.fb.group({
      memoryThreshold: this.memoryThreshold,
      storageThreshold: this.storageThreshold
    });
    this.address.valueChanges
    .pipe(takeUntil(this.onDestroy))
    .subscribe(() => {
      this.addressReachable = false;
      this.serverInfoFormGroup.setErrors({ 'checkAddress' : true});
      this.switchControls(false);
    });
  }
  onSubmit() {
    /*if (this.serversList.filter(s => s.address === this.address.value).length > 0) {
      this.address.setErrors({ 'itemExists': true });
      return;
    }*/
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
      console.log(server);
    }, (error) => this.handleError(error))
  }
  
  onConnectivityCheck() {
    if (this.checkingConnectivity) return;
    this.checkingConnectivity = true;
    this.adminService.checkstorageconnectivity(this.address.value)
    .pipe(takeUntil(this.onDestroy), finalize(() => this.checkingConnectivity = false))
      .subscribe((result: any) => {
        this.addressReachable = result.state == 0;
        if (result.state != 0) {
          this.message$.next({errorMessage: result?.errorContent?.message});
        }
        else {
          this.address.setErrors(null);
          this.message$.next({ successMessage: 'The storage server is up and running' });
          // Retrieve storage server upload configuration
          this.adminService.getUploadConfigrConfig(this.address.value)
          .pipe(takeUntil(this.onDestroy))
          .subscribe((result: any) => {
            if (result.state == 0) {
             this.switchControls(true);
             this.uploadPath.setValue(result.uploadOpts.uploadPath);
             this.chunkSize.setValue(result.uploadOpts.chunkSize);
             this.expiration.setValue(result.uploadOpts.expiration);
             // Retrieve storage server hardware configuration
             this.adminService.getHardwareConfig(this.address.value)
             .pipe(takeUntil(this.onDestroy))
             .subscribe((result: any) => {
              if (result.state == 0) {
                this.memoryThreshold.setValue(result.hardwareOptions.memoryThreshold);
                this.storageThreshold.setValue(result.hardwareOptions.storageThreshold);
              }
              else {
                this.message$.next({errorMessage: result?.errorContent?.message});
              }
             })
            }
            else {
              this.message$.next({errorMessage: result?.errorContent?.message});
            }
          });
         }
      });
  }
  private switchControls(enabled: boolean) {
    if (enabled) {
      this.uploadPath.enable();
      this.chunkSize.enable();
      this.expiration.enable();
      this.optionControl.enable();
      this.memoryThreshold.enable();
      this.storageThreshold.enable();
    }
    else {
      this.uploadPath.disable();
      this.chunkSize.disable();
      this.expiration.disable();
      this.optionControl.disable();
      this.memoryThreshold.disable();
      this.storageThreshold.disable();
    }
  }
}
interface IServerOption {
  name: string;
  hint: string;
  state: serverState;
}