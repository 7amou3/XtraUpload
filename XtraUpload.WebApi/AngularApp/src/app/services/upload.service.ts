import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import * as tus from 'tus-js-client';
import { UserStorageService } from './user.storage.service';
import { UploadStatus, IFileInfo,  IUploadSettings } from '../domain';
import { HttpResponse } from 'tus-js-client';

@Injectable()
export class UploadService {
    /** Emited when a file has been successfully uploaded */
    fileUploaded$ = new Subject<IFileInfo>();
    constructor(private storageService: UserStorageService) { }
    startUpload(file: File, uploadSettings: IUploadSettings, urlPath: string, folderId: string): Observable<UploadStatus> {
        const uploadStatus = new UploadStatus();
        uploadStatus.filename = file.name;
        uploadStatus.size = file.size;
        
        const upStatus$ = new Subject<UploadStatus>();
        const onTusError = (error) => {
            uploadStatus.status = 'Error';
            uploadStatus.message = error;
            upStatus$.next(uploadStatus);
        };
        const onAfterResponse = (req, res: HttpResponse) => {
            if (res.getStatus() == 204) {
                uploadStatus.uploadData = JSON.parse(res.getHeader("upload-data"));
            }
        };
        const onTusSuccess = () => {
            uploadStatus.status = 'Success';
            uploadStatus.fileId = uploadStatus.instance.url.split('/').pop();
            upStatus$.next(uploadStatus);
            if (urlPath === 'fileupload') {
                const file = uploadStatus.uploadData as IFileInfo;
                file.createdAt = new Date();
                file.lastModified = new Date();
                this.fileUploaded$.next(uploadStatus.uploadData as IFileInfo);
            }
        };
        const onTusProgress = (bytesUploaded: number, bytesTotal: number): void => {
            const progress = (bytesUploaded / bytesTotal * 100).toFixed(1);
            uploadStatus.status = 'InProgress';
            // uploadStatus.fileId = upload.url.split('/').pop();
            uploadStatus.message = progress as Object;
            upStatus$.next(uploadStatus);
        };
        // add '/' at the end of the url if not exist
        const address = uploadSettings.storageServer.address.replace(/\/?$/, '/');
        // Defaut urlPath for tus is [fileupload or avatarupload] as configured in the server
        const uri = address + urlPath;

        uploadStatus.instance = new tus.Upload(file,
            {
                endpoint: uri,
                chunkSize: uploadSettings.chunkSize === 0 ? 25 * 1024 * 1024 : uploadSettings.chunkSize,
                onError: onTusError,
                onProgress: onTusProgress,
                onSuccess: onTusSuccess,
                onAfterResponse: onAfterResponse,
                retryDelays: [0, 3000, 5000, 10000, 20000],
                metadata: {
                    name: file.name,
                    contentType: file.type || 'application/octet-stream',
                    folderId: folderId,
                    serverId: uploadSettings.storageServer.id
                },
                headers: {
                    'authorization': 'Bearer ' + this.storageService.jwt
                }
            });
        // Start the upload
        uploadStatus.instance.start();
        return upStatus$;
    }
}