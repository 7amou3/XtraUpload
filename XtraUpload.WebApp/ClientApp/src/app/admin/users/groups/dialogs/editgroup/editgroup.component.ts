import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { takeUntil, merge, map, finalize } from 'rxjs/operators';
import { ComponentBase } from 'app/shared';
import { AdminService } from 'app/services';
import { IUserRoleClaims, IClaims } from 'app/domain';

@Component({
  selector: 'app-editgroup',
  templateUrl: './editgroup.component.html'
})
export class EditgroupComponent extends ComponentBase implements OnInit {
  editFormGroup: FormGroup;
  groupName = new FormControl('', [Validators.required, Validators.minLength(3)]);
  adminAreaAccess = new FormControl(true);
  fileManagerAccess = new FormControl(true);
  downloadSpeed = new FormControl(0, [Validators.required]);
  storageSpace = new FormControl(0, [Validators.required]);
  maxFileSize = new FormControl(0, [Validators.required]);
  fileExpiration = new FormControl(0, [Validators.required]);
  concurrentUpload = new FormControl(0, [Validators.required]);
  waitTime = new FormControl(0, [Validators.required]);
  downloadTTW = new FormControl(0, [Validators.required]);
  constructor(
    private dialogRef: MatDialogRef<EditgroupComponent>,
    private fb: FormBuilder,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) public item: { selectedGroup: IUserRoleClaims, fullGroupList: IUserRoleClaims[] }
  ) {
    super();
   }

  ngOnInit(): void {
    this.editFormGroup = this.fb.group({
      groupName: this.groupName,
      adminAreaAccess: this.adminAreaAccess,
      fileManagerAccess: this.fileManagerAccess,
      downloadSpeed: this.downloadSpeed,
      storageSpace: this.storageSpace,
      maxFileSize: this.maxFileSize,
      fileExpiration: this.fileExpiration,
      concurrentUpload: this.concurrentUpload,
      waitTime: this.waitTime,
      downloadTTW: this.downloadTTW,
    });
    // Set default values
    this.groupName.setValue(this.item.selectedGroup.role.name);
    this.downloadSpeed.setValue(+this.getClaim('DownloadSpeed'));
    this.storageSpace.setValue(+this.getClaim('StorageSpace'));
    this.maxFileSize.setValue(+this.getClaim('MaxFileSize'));
    this.fileExpiration.setValue(+this.getClaim('FileExpiration'));
    this.concurrentUpload.setValue(+this.getClaim('ConcurrentUpload'));
    this.waitTime.setValue(+this.getClaim('WaitTime'));
    this.downloadTTW.setValue(+this.getClaim('DownloadTTW'));
    this.adminAreaAccess.setValue(this.getClaim('AdminAreaAccess') && true);
    this.fileManagerAccess.setValue(this.getClaim('FileManagerAccess') && true);
    // Listen for changes
    this.fileManagerAccess.valueChanges
    .pipe(takeUntil(this.onDestroy),
          merge(this.adminAreaAccess.valueChanges),
          map(() => this.adminAreaAccess.value || this.fileManagerAccess.value))
    .subscribe(state => {
      if (state) {
        this.storageSpace.enable({onlySelf: state});
        this.maxFileSize.enable({onlySelf: state});
        this.fileExpiration.enable({onlySelf: state});
        this.concurrentUpload.enable({onlySelf: state});
      }
      else {
        this.storageSpace.disable({onlySelf: state});
        this.maxFileSize.disable({onlySelf: state});
        this.fileExpiration.disable({onlySelf: state});
        this.concurrentUpload.disable({onlySelf: state});
      }
    });
    // Check if at least there is an active admin group
    this.adminAreaAccess.valueChanges
    .pipe(takeUntil(this.onDestroy))
    .subscribe(state => {
      const adminGroup = this.item.fullGroupList.find(s => s.role.name !== this.item.selectedGroup.role.name 
        && s.claims.find(x => x.claimType === 'AdminAreaAccess'));
      if (!adminGroup && !state) {
        this.adminAreaAccess.setErrors({'missingAdmin': true});
      }
      else this.adminAreaAccess.setErrors(null);
    });
    // Check if the group name is unique
    this.groupName.valueChanges
    .pipe(takeUntil(this.onDestroy))
    .subscribe(gName => {
      // isUnique
      const groups = this.item.fullGroupList.filter(s => s.role.id !== this.item.selectedGroup.role.id);
      if (groups.find(s => s.role.name === gName)) {
        this.groupName.setErrors({'isDuplicated': true});
      }
      else if (this.groupName.hasError('isDuplicated')) {
        this.groupName.setErrors(null);
      }
    });
  }
  onSubmit(groupParams: IClaims) {
    this.isBusy = true;
    this.item.selectedGroup.role.name = groupParams.groupName;
    this.adminService.updateGroup(this.item.selectedGroup.role, groupParams)
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.isBusy = false))
      .subscribe(
        (result) => {
          this.dialogRef.close(result);
        }, (error) => this.handleError(error)
      );
  }
  getClaim(claim: string) {
    return this.item.selectedGroup.claims.find(s => s.claimType === claim)?.claimValue;
  }

}
