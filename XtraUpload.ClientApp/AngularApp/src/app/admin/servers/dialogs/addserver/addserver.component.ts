import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IStorageServer, serverState } from 'app/domain';
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
  checkingConnectivity = false;
  addressReachable = false;
  /** must be https, allowed ip or host name */
  address = new FormControl('https://', [Validators.required, /*Validators.pattern(/^https:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&\=]*)/)*/]);
  optionControl = new FormControl({value: '', disabled: true}, Validators.required);
  uploadPath = new FormControl({value: '', disabled: true}, [Validators.required]);
  chunkSize = new FormControl({value: '', disabled: true}, [Validators.required, Validators.min(1)]);
  expiration = new FormControl({value: '', disabled: true}, [Validators.required, Validators.min(1)]);
  
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
    this.address.valueChanges
    .pipe(takeUntil(this.onDestroy))
    .subscribe(() => {
      this.addressReachable = false;
      this.serverInfoFormGroup.setErrors({ 'checkAddress' : true});
      this.switchControls(false);
    });
  }
  onSubmit(formParams: IStorageServer) {
    console.log(this.optionControl.value )
    if (this.serversList.filter(s => s.address === formParams.address).length > 0) {
      this.address.setErrors({ 'itemExists': true });
      return;
    }
  }
  
  onConnectivityCheck() {
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
          // Retrieve server configuration
          this.adminService.GetStorageServerConfig(this.address.value)
          .pipe(takeUntil(this.onDestroy))
          .subscribe((result: any) => {
            if (result.state == 0) {
             this.switchControls(true);
             this.uploadPath.setValue(result.uploadOpts.uploadPath);
             this.chunkSize.setValue(result.uploadOpts.chunkSize);
             this.expiration.setValue(result.uploadOpts.expiration);
              this.uploadOpts.next(result.uploadOpts);
              console.log(result.uploadOpts)
            }
            else {
              this.message$.next({errorMessage: result?.errorContent?.message});
            }
          });
         }
      });
  }
  switchControls(enabled: boolean) {
    if (enabled) {
      this.uploadPath.enable();
      this.chunkSize.enable();
      this.expiration.enable();
      this.optionControl.enable();
    }
    else {
      this.uploadPath.disable();
      this.chunkSize.disable();
      this.expiration.disable();
      this.optionControl.disable();
    }
  }
}
interface IServerOption {
  name: string;
  hint: string;
  state: serverState;
}