import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared';
import { takeUntil, merge, map, finalize } from 'rxjs/operators';
import { IClaims, IUserRoleClaims } from 'app/domain';

@Component({
  selector: 'app-addgroup',
  templateUrl: './addgroup.component.html'
})
export class AddgroupComponent extends ComponentBase implements OnInit {
  addFormGroup: FormGroup;
  groupName = new FormControl('', [Validators.required, Validators.minLength(3)]);
  adminAreaAccess = new FormControl(false);
  fileManagerAccess = new FormControl(true);
  downloadSpeed = new FormControl(0, [Validators.required]);
  storageSpace = new FormControl(0, [Validators.required]);
  maxFileSize = new FormControl(0, [Validators.required]);
  fileExpiration = new FormControl(0, [Validators.required]);
  concurrentUpload = new FormControl(0, [Validators.required]);
  waitTime = new FormControl(0, [Validators.required]);
  downloadTTW = new FormControl(0, [Validators.required]);
  constructor(
    private dialogRef: MatDialogRef<AddgroupComponent>,
    private fb: FormBuilder,
    private adminService: AdminService,
    @Inject(MAT_DIALOG_DATA) private fullGroupList: IUserRoleClaims[]
  ) {
    super();
   }

  ngOnInit(): void {
    this.addFormGroup = this.fb.group({
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
    // Check if the group name is unique
    this.groupName.valueChanges
    .pipe(takeUntil(this.onDestroy))
    .subscribe(gName => {
      // isUnique
      if (this.fullGroupList.find(s => s.role.name === gName)) {
        this.groupName.setErrors({'isDuplicated': true});
      }
      else if (this.groupName.hasError('isDuplicated')) {
        this.groupName.setErrors(null);
      }
    });
  }
  onSubmit(groupParams: IClaims) {
    this.isBusy = true;
    this.adminService.addGroup(groupParams)
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.isBusy = false))
      .subscribe(
        (result) => {
          this.dialogRef.close(result);
        }
      );
  }

}
