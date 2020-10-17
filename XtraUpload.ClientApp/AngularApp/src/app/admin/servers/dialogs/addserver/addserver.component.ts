import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IStorageServer, serverState } from 'app/domain';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared';
import { finalize, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-addserver',
  templateUrl: './addserver.component.html'
})

export class AddserverComponent extends ComponentBase implements OnInit {
  addFormGroup: FormGroup;
  /** must be https, allowed ip or host name */
  address = new FormControl('', [Validators.required, /*Validators.pattern(/^https:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&\=]*)/)*/]);
  optionControl = new FormControl('', Validators.required);
  checkingConnectivity = false;
  addressReachable = false;
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
    this.addFormGroup = this.fb.group({
      address: this.address,
      state: this.optionControl,
    });
    this.address.valueChanges
    .pipe(takeUntil(this.onDestroy))
    .subscribe(() => {
      this.addressReachable = false;
      this.addFormGroup.setErrors({checkAddress: true});
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
          this.message$.next({ successMessage: 'The storage server is up and running' });
         }
      });
  }
}
interface IServerOption {
  name: string;
  hint: string;
  state: serverState;
}