import { Component, OnInit } from '@angular/core';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { NgxDropzoneChangeEvent } from 'ngx-dropzone';
import { ComponentBase } from 'app/shared';
import { FileManagerService, HeaderService } from 'app/services';
import { takeUntil, finalize } from 'rxjs/operators';
import { RejectedFile } from 'ngx-dropzone/lib/ngx-dropzone.service';

@Component({
  selector: 'app-avatar',
  templateUrl: './avatar.component.html',
  styleUrls: ['./avatar.component.css']
})
export class AvatarComponent extends ComponentBase implements OnInit {
  selectedImg: File;
  croppedImage: any = '';
  constructor(
    private fileMngService: FileManagerService,
    private headerService: HeaderService) {
    super();
  }

  ngOnInit(): void {
  }
  onSelect(event: NgxDropzoneChangeEvent): void {
    if (event.rejectedFiles.length > 0) {
      const rejected = event.rejectedFiles[0] as RejectedFile;
        if (rejected.reason === 'type') {
          this.message$.next({errorMessage: 'The selected file type is not allowed.'});
        } else {
          this.message$.next({errorMessage: 'You exceeded the file size limit.'});
        }
    }
    this.selectedImg = event.addedFiles[0];
  }
  imageCropped(event: ImageCroppedEvent) {
    this.croppedImage = event.base64;
  }
  imageLoaded() {
    // show cropper
  }
  cropperReady() {
    // cropper ready
  }
  loadImageFailed() {
    // show message
  }
  onDeleteImage() {
    this.selectedImg = null;
  }
  onUpdateAvatar() {
    this.isBusy = true;
    this.urltoFile(this.croppedImage, 'avatar.png', 'image/png')
      .then( avatar => {
        this.fileMngService.startUpload(avatar, null, 1024 * 1024 * 2, 'avatarupload')
          .pipe(takeUntil(this.onDestroy),
            finalize(() => this.isBusy = false))
          .subscribe(result => {
            if (result.status === 'Success') {
              this.isBusy = false;
              this.selectedImg = null;
              this.message$.next({successMessage: 'Your avatar has been updated successfully.'});
              this.headerService.notifyAvatarChanged();
            }
          });
      });

  }
  urltoFile(url, filename, mimeType) {
    return (fetch(url)
      .then(function (res) { return res.arrayBuffer(); })
      .then(function (buf) { return new File([buf], filename, { type: mimeType }); })
    );
  }
}
