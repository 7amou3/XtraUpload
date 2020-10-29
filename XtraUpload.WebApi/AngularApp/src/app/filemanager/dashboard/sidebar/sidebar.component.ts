import {Component, Output, EventEmitter, OnInit, Input} from '@angular/core';
import { IFolderInfo, IUploadSettings } from 'app/domain';
import { ComponentBase } from 'app/shared';
import { FileManagerService, UserStorageService, AuthService } from 'app/services';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class AppSidebarComponent extends ComponentBase implements OnInit {
  @Input() uploadSetting$: Subject<IUploadSettings>;
  @Output() folderPathChanged$ = new EventEmitter<IFolderInfo[]>();
  uploadSetting: IUploadSettings;
  avatar: string;
  username: string;
  constructor(
    private fileMngService: FileManagerService,
    private storageService: UserStorageService,
    private authService: AuthService) {
    super();
  }
  ngOnInit() {
    const profile = this.storageService.getProfile();
    if (profile) {
      this.avatar = profile.avatar;
      this.username = profile.userName;
      if (!profile.avatar) {
        this.avatar = 'assets/images/users/profile-icon.png';
    }
    }

    // subscribe to successful upload to update progressbar size status
    this.fileMngService.fileUploaded$
    .pipe(takeUntil(this.onDestroy))
    .subscribe(
      file => {
        this.uploadSetting.usedSpace += file.size;
        this.checkUploadLimit();
      }
    );
    // subscribe to delete event to update progressbar size status
    this.fileMngService.fileDeleted$
    .pipe(takeUntil(this.onDestroy))
    .subscribe(
      file => {
        this.uploadSetting.usedSpace -= file.size;
        this.checkUploadLimit();
      }
    );

    this.uploadSetting$
    .pipe(takeUntil(this.onDestroy))
    .subscribe(
      setting => {
        this.uploadSetting = setting;
        this.checkUploadLimit();
      }
    );
  }
  checkUploadLimit() {
    this.fileMngService.notifyStorageQuota(
    this.uploadSetting.usedSpace > this.uploadSetting.storageSpace * 1024 * 1024);
  }
  onfolderPathChanged(childfolderPath: IFolderInfo[]) {
    this.folderPathChanged$.emit(childfolderPath);
  }

  onSignOut() {
    this.authService.signOut();
  }
}
