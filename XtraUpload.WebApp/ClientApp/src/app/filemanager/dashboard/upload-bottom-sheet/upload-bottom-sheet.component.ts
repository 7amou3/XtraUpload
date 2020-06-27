import { Component, OnInit } from '@angular/core';
import { MatBottomSheetRef } from '@angular/material';
import { FileManagerService } from 'app/services';
import { ActivatedRoute } from '@angular/router';
import { takeUntil, finalize } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { UploadStatus, IUploadSettings } from 'app/domain';
import { ComponentBase } from 'app/shared';
import { NgxDropzoneChangeEvent } from 'ngx-dropzone';
import { RejectedFile } from 'ngx-dropzone/lib/ngx-dropzone.service';

@Component({
  selector: 'app-upload-bottom-sheet',
  templateUrl: './upload-bottom-sheet.component.html',
  styleUrls: ['./upload-bottom-sheet.component.css']
})
export class UploadBottomSheetComponent extends ComponentBase implements OnInit {
  storageLimitReached: boolean;
  files: FileUpload[] = [];
  uploadSetting: IUploadSettings;
  constructor(
    private fileMngService: FileManagerService,
    private route: ActivatedRoute,
    private bottomSheetRef: MatBottomSheetRef<UploadBottomSheetComponent>) {
    super();
  }
  ngOnInit() {
    this.bottomSheetRef.disableClose = true;
    this.isBusy = true;
    this.fileMngService.getUploadSetting()
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
    .subscribe(setting => {
      this.uploadSetting = setting;
    },
    (error) => {
      throw error;
    });
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
          throw Error ('The selected file type is not allowed.');
        } else {
          throw Error ('You exceeded the file size limit.');
        }
    }
    // Accepted files
    event.addedFiles.forEach((upload: File) => {
      if (this.files.find(s => s.name === upload.name)) {
        // file already exist
        throw Error(`File ${upload.name} already exist`);
      }
      const upstatus = new UploadStatus();
      upstatus.filename = upload.name;
      upstatus.status = 'ToDo';
      upstatus.message = 0 as Object;
      upstatus.size = upload.size;
      const fileToUpload = {file: upload, uploadStatus$: new Subject<UploadStatus>(), name: upload.name, size: upload.size };
      this.files.push(fileToUpload);
      fileToUpload.uploadStatus$.next(upstatus);
    });
    const numFilesToUpload = this.files.filter(s => s.file).length;
    if (numFilesToUpload > this.uploadSetting.concurrentUpload) {
      const amount = numFilesToUpload - this.uploadSetting.concurrentUpload;
      for (let i = 0; i < amount; i++) {
        this.files.splice(this.files.length - 1, 1);
      }
      throw Error(`Max conccurent upload exceeded, you can upload up to ${this.uploadSetting.concurrentUpload} files at a time.`);
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
    this.files.forEach(upload => {
      if (upload.file) {
        this.fileMngService.startUpload(upload.file, currentFolderId, this.uploadSetting.chunkSize)
        .pipe(takeUntil(this.onDestroy))
        .subscribe(data => {
          const file = this.files.find(s => s.name === data.filename);
          if (!file) {
            return;
          }
          file.uploadStatus$.next(data);
          if (data.status === 'Success') {
            // delete the uploaded file from the collection so the user cannot re-upload it
            file.file = null;
          }
        });
      }
    });
  }
}
export class FileUpload {
  file: File;
  name: string;
  size: number;
  uploadStatus$: Subject<UploadStatus>;
}
