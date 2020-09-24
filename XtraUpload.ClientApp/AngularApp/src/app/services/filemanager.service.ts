import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, Subject, ReplaySubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import * as tus from 'tus-js-client';
import { UserStorageService } from './user.storage.service';
import {
  IFolderModel, IItemInfo, IFolderInfo, UploadStatus,
  IFileInfo, MovedItemsModel, ICreateFolderModel, IRenameItemModel, IDownload, IUploadSettings,
  IAccountOverview, IBulkDelete } from '../domain';
import { isFile } from 'app/filemanager/dashboard/helpers';

@Injectable()
export class FileManagerService {
  /** Emited when a subfolder is created */
  subFolderCreated$ = new Subject<IFolderInfo>();
  /** Emited when a subfolder is deleted */
  subFolderDeleted$ = new Subject<IFolderInfo>();
  /**Emited when a file is deleted */
  fileDeleted$ = new Subject<IFileInfo>();
  /** Emited when a file has been renamed */
  fileRenamed$ = new Subject<IFileInfo>();
  /** Emited when a folder has been renamed */
  folderRenamed$ = new Subject<IFolderInfo>();
  /** Emited when a file has been successfully uploaded */
  fileUploaded$ = new Subject<UploadStatus>();
  /** Emited when a file availability changed */
  fileAvailabilityChanged$ = new Subject<IFileInfo>();
  /** Emited when a folder availability changed */
  folderAvailabilityChanged$ = new Subject<IFolderInfo>();
  /** Emited when the folder tree updated */
  folderTreeChanged$ = new Subject<IFolderInfo[]>();
  storageQuotaReached$ = new ReplaySubject<boolean>();
  itemsMoved$ = new Subject<string[]>();
  constructor(
    private http: HttpClient,
    private storageService: UserStorageService,
    @Inject('BASE_URL') private baseUrl: string) { }

  /** Get all folders */
  getAllFolders(): Observable<IFolderInfo[]> {
    return this.http.get<IFolderInfo[]>('folder/folders');
  }
  /** Get child folders relative to the given folder */
  getFolderTreeById(folderid: string): Observable<IFolderInfo[]> {
    return this.http.get<IFolderInfo[]>('folder/folders/' + folderid);
  }
  /** Get files and folders within a folder */
  getFolderContent(folderid?: string): Observable<IItemInfo[]> {

    let url = 'folder/';
    if (folderid) {
      url = url.concat(folderid);
    }
    // the response is an array of files and an array of folders
    return this.http.get<IFolderModel>(url)
      .pipe(
        map(data => [...data.folders ?? [], ...data.files ?? []])
      );
  }
  getPublicFolderContent(folderid: string, subfolderId?: string): Observable<IItemInfo[]> {
    let params = new HttpParams()
            .set('mainFolderId', folderid);
    if (subfolderId) {
      params = params.set('childFolderId', subfolderId);
    }
    return this.http.get<IFolderModel>('folder/publicfolder/', {params: params})
      .pipe(
        map(data => [...data.folders ?? [], ...data.files ?? []])
      );
  }
  /** Moves items (selected folders and files) to the destination folder */
  requestMoveItems(items: IItemInfo[], destFolderId: string): Observable<boolean> {
    const files = [];
    const folders = [];
    items.forEach(item => {
      if (isFile(item)) {
        files.push(item);
      }
      else {
        folders.push(item);
      }
    });
    return this.http.put('file/moveitems', new MovedItemsModel(files, folders, destFolderId))
      .pipe(
        map((result: any) => {
          if (result) {
            this.folderTreeChanged$.next(result.folders);
            this.itemsMoved$.next(result.movedItemsIds);
            return true;
          }
        })
      );
  }
  notifyStorageQuota(reached: boolean) {
    this.storageQuotaReached$.next(reached);
  }
  /** Create sub folder */
  createSubFolder(subFolder: ICreateFolderModel): Observable<IFolderInfo> {
    return this.http.post<IFolderInfo>('folder', subFolder)
      .pipe(
        tap(newFolder => {
          this.subFolderCreated$.next(newFolder);
        })
      );
  }

  /** Rename a file */
  renameFile(file: IRenameItemModel): Observable<IFileInfo> {
    return this.http.patch<IFileInfo>('file/rename', file)
    .pipe(tap(data => {
      this.fileRenamed$.next(data);
    }));
  }
  /** Rename a folder */
  renameFolder (folder: IRenameItemModel): Observable<IFolderInfo> {
    return this.http.patch<IFolderInfo>('folder/rename', folder)
    .pipe(tap(data => {
      this.folderRenamed$.next(data);
    }));
  }
  getFileInfo(tusFileId: string): Observable<IFileInfo> {
    return this.http.get<IFileInfo>('file/' + tusFileId);
  }
  getFile(fileId: string): Observable<IFileInfo> {
    return this.http.get<IFileInfo>('file/requestdownload/' + fileId);
  }

  deleteItems(items: IItemInfo[]): Observable<IBulkDelete> {
    const files = [];
    const folders = [];
    items.forEach(item => {
      if (isFile(item)) {
        files.push(item);
      }
      else {
        folders.push(item);
      }
    });
    return this.http.request<IBulkDelete>('delete', 'file/deleteitems', { body: {selectedFiles: files, selectedFolders: folders} })
    .pipe(
      tap(items => {
        items.files.forEach(file => {
          this.fileDeleted$.next(file);
        });
        items.folders.forEach(folder => {
          this.subFolderDeleted$.next(folder.folders[folder.folders.length - 1]);
        });
      })
    );
  }

  startUpload(file: File, uploadServer: string, urlPath: string, folderId: string, chunkSize: number): Observable<UploadStatus> {
    let upload: tus.Upload;
    const event = new Subject<UploadStatus>();
    const uploadStatus = new UploadStatus();
    uploadStatus.filename = file.name;
    uploadStatus.size = file.size;

    const onTusError = (error) => {
      uploadStatus.status = 'Error';
      uploadStatus.message = error;
      event.next(uploadStatus);
    };
    const onTusSuccess = () => {
      uploadStatus.status = 'Success';
      uploadStatus.fileId = upload.url.split('/').pop();
      event.next(uploadStatus);
      this.fileUploaded$.next(uploadStatus);
    };
    const onTusProgress = (bytesUploaded: number, bytesTotal: number): void => {
      const progress = (bytesUploaded / bytesTotal * 100).toFixed(1);
      uploadStatus.status = 'InProgress';
      // uploadStatus.fileId = upload.url.split('/').pop();
      uploadStatus.message = progress as Object;
      event.next(uploadStatus);
    };
    // Defaut urlPath for tus is [fileupload or avatarupload] as configured in the server
    const uri = uploadServer + urlPath;
    upload = new tus.Upload(file,
    {
      endpoint: uri,
      chunkSize: chunkSize,
      onError: onTusError,
      onProgress: onTusProgress,
      onSuccess: onTusSuccess,
      retryDelays: [0, 3000, 5000, 10000, 20000],
      metadata: {
        name: file.name,
        contentType: file.type || 'application/octet-stream',
        folderId: folderId
      },
      headers: {
        'authorization': 'Bearer ' + this.storageService.getToken()
      }
    });
    // Start the upload
    upload.start();

    return event;
  }

  updateFileAvailability(file: IItemOnlineAvailability) {
    return this.http.patch<IFileInfo>('file/fileavailability', {fileid: file.itemId, available: file.available})
    .pipe(
      tap(data => this.fileAvailabilityChanged$.next(data))
    );
  }
  updateFolderAvailability(folder: IItemOnlineAvailability) {
    return this.http.patch<IFolderInfo>('folder/folderavailability', {folderid: folder.itemId, available: folder.available})
    .pipe(
      tap(data => this.folderAvailabilityChanged$.next(data))
    );
  }

  generateDownloadLink(fileid: string): Observable<IDownload> {
    return this.http.get<IDownload>('file/templink/' + fileid);
  }

  startDownload(url: string): Observable<Blob> {
    return this.http.get(url, {
      responseType: 'blob',
      reportProgress: true,
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
        Authorization: `Bearer ${this.storageService.getToken()}`
      }
    });
  }

  getUploadSetting(): Observable<IUploadSettings> {
    return this.http.get<IUploadSettings>('setting/uploadsetting');
  }

  getAccountOverview(): Observable<IAccountOverview> {
    return this.http.get<IAccountOverview>('setting/accountoverview');
  }
}

interface IItemOnlineAvailability {
  itemId: string;
  available: boolean;
}
