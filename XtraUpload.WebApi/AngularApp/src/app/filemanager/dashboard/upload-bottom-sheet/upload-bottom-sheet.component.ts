import { Component, OnInit } from '@angular/core';
import { MatBottomSheetRef } from '@angular/material/bottom-sheet';
import { FileManagerService, UploadService } from 'app/services';
import { ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { BehaviorSubject, Subject } from 'rxjs';
import { UploadStatus, IUploadSettings, IFileInfo } from 'app/models';
import { ComponentBase } from 'app/shared/components';
import { NgxDropzoneChangeEvent } from 'ngx-dropzone';
import { RejectedFile } from 'ngx-dropzone/lib/ngx-dropzone.service';
import { forEachPromise } from '../helpers';

@Component({
  selector: 'app-upload-bottom-sheet',
  templateUrl: './upload-bottom-sheet.component.html',
  styleUrls: ['./upload-bottom-sheet.component.scss']
})
export class UploadBottomSheetComponent extends ComponentBase implements OnInit {
  storageLimitReached: boolean;
  files: FileUpload[] = [];
  uploadSetting$ = new BehaviorSubject<IUploadSettings>(null);
  constructor(
    private fileMngService: FileManagerService,
    private uploadService: UploadService,
    private route: ActivatedRoute,
    private bottomSheetRef: MatBottomSheetRef<UploadBottomSheetComponent>) {
    super();
  }
  async ngOnInit() {
    this.isBusy = true;
    await this.fileMngService.getUploadSetting()
      .then(setting => this.uploadSetting$.next(setting))
      .catch(() => this.uploadSetting$.next(null))
      .finally(() => this.isBusy = false);
    this.fileMngService.storageQuotaReached$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(storageLimitReached => {
        this.storageLimitReached = storageLimitReached;
      });
  }

  onCloseUploadSheet(event: MouseEvent): void {
    this.bottomSheetRef.dismiss();
    event.preventDefault();
  }

  onSelect(event: NgxDropzoneChangeEvent): void {
    if (event.rejectedFiles.length > 0) {
      const rejected = event.rejectedFiles[0] as RejectedFile;
      if (rejected.reason === 'type') {
        throw Error($localize`The selected file type is not allowed.`);
      } else {
        throw Error($localize`You exceeded the file size limit.`);
      }
    }
    // Accepted files
    event.addedFiles.forEach((upload: File) => {
      if (this.files.find(s => s.name === upload.name)) {
        // file already exist
        throw Error($localize`File ${upload.name} already exist`);
      }
      const upstatus = new UploadStatus();
      upstatus.filename = upload.name;
      upstatus.status = 'ToDo';
      upstatus.message = 0 as Object;
      upstatus.size = upload.size;
      const fileToUpload = { file: upload, uploadStatus$: new BehaviorSubject<UploadStatus>(null), name: upload.name, size: upload.size, downloadUrl: null };
      this.files.push(fileToUpload);
      fileToUpload.uploadStatus$.next(upstatus);
    });
    const numFilesToUpload = this.files.filter(s => s.file).length;
    if (numFilesToUpload > this.uploadSetting$.getValue().concurrentUpload) {
      const amount = numFilesToUpload - this.uploadSetting$.getValue().concurrentUpload;
      for (let i = 0; i < amount; i++) {
        this.files.splice(this.files.length - 1, 1);
      }
      throw Error($localize`Max conccurent upload exceeded, you can upload up to ${this.uploadSetting$.getValue().concurrentUpload} files at a time.`);
    }
  }

  onRemoveFile(file: FileUpload): void {
    const fileremove = this.files.find(s => s.file === file.file);
    if (fileremove) {
      this.files.splice(this.files.indexOf(fileremove), 1);
    }
  }
  onRequestUpload() {
    let currentFolderId = this.route.snapshot.queryParamMap.get('folder');
    if (!currentFolderId) {
      currentFolderId = 'root';
    }
    forEachPromise(this.files.filter(s => s.file != null), this.processUpload, new UploadContext(this.uploadService, this.files, this.uploadSetting$.getValue(), this.onDestroy, currentFolderId));
  }

  private async processUpload(upload: FileUpload, context: UploadContext) {
    return new Promise(async (resolve, reject) => {
      context.uploadService.startUpload(upload.file, context.uploadSettings, 'fileupload', context.currentFolderId)
        .pipe(takeUntil(context.onDestroy))
        .subscribe((data => {
          const file = context.files.find(s => s.name === data.filename);
          if (!file)
            reject();
          file.uploadStatus$.next(data);
          if (data.status === 'Success') {
            file.downloadUrl = 'file?id=' + (data.uploadData as IFileInfo).id;
            // delete the uploaded file from the collection so the user cannot re-upload it
            file.file = null;
            resolve();
          }
        }), error => reject());
    })
  }
}
export class FileUpload {
  file: File;
  name: string;
  size: number;
  uploadStatus$: BehaviorSubject<UploadStatus>;
  downloadUrl: string;
}
export class UploadContext {

  constructor(uploadService: UploadService,
    files: FileUpload[],
    uploadSettings: IUploadSettings,
    onDestroy: Subject<void>,
    currentFolderId: string) {
    this.uploadService = uploadService;
    this.files = files;
    this.uploadSettings = uploadSettings;
    this.onDestroy = onDestroy;
    this.currentFolderId = currentFolderId;
  }
  uploadService: UploadService;
  uploadSettings: IUploadSettings;
  files: FileUpload[];
  onDestroy: Subject<void>;
  currentFolderId: string;
}
