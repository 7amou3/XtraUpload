import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, Subject, ReplaySubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { UserStorageService } from './user.storage.service';
import {
  IFolderModel, IItemInfo, IFolderInfo,
  IFileInfo, MovedItemsModel, ICreateFolderModel, IRenameItemModel, IDownload, IUploadSettings,
  IAccountOverview, IBulkDelete
} from '../models';
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
  /** Emited when a file availability changed */
  fileAvailabilityChanged$ = new Subject<IFileInfo>();
  /** Emited when a folder availability changed */
  folderAvailabilityChanged$ = new Subject<IFolderInfo>();
  /** Emited when the folder tree updated */
  folderTreeChanged$ = new Subject<IFolderInfo[]>();
  storageQuotaReached$ = new ReplaySubject<boolean>();
  itemsMoved$ = new Subject<string[]>();
  private isbusy$ = new Subject<boolean>();
  serviceBusy$ = this.isbusy$.asObservable();

  constructor(
    private http: HttpClient,
    private storageService: UserStorageService) { }

  /** Notify whether the service is busy */
  notifyBusy(val: boolean): void {
    return this.isbusy$.next(val);
  }

  /** Get all folders */
  getAllFolders(): Observable<IFolderInfo[]> {
    return this.http.get<IFolderInfo[]>('folder/folders');
  }
  /** Get child folders relative to the given folder */
  async getFolderTreeById(folderid: string): Promise<IFolderInfo[]> {
    return this.http.get<IFolderInfo[]>('folder/folders/' + folderid).toPromise();
  }
  /** Get files and folders within a folder */
  async getFolderContent(folderid?: string): Promise<IItemInfo[]> {
    let url = 'folder/';
    if (folderid) {
      url = url.concat(folderid);
    }
    // the response is an array of files and an array of folders
    return this.http.get<IFolderModel>(url)
      .pipe(map(data => [...data.folders ?? [], ...data.files ?? []]))
      .toPromise();
  }
  async getPublicFolderContent(folderid: string, subfolderId?: string): Promise<IItemInfo[]> {
    let params = new HttpParams()
      .set('mainFolderId', folderid);
    if (subfolderId) {
      params = params.set('childFolderId', subfolderId);
    }
    return this.http.get<IFolderModel>('folder/publicfolder/', { params: params })
      .pipe(map(data => [...data.folders ?? [], ...data.files ?? []]))
      .toPromise();
  }
  /** Moves items (selected folders and files) to the destination folder */
  async requestMoveItems(items: IItemInfo[], destFolderId: string): Promise<boolean> {
    const files = [], folders = [];
    items.forEach(item => {
      if (isFile(item)) {
        files.push(item);
      } else {
        folders.push(item);
      }
    });
    return this.http.put('file/moveitems', new MovedItemsModel(files, folders, destFolderId))
      .pipe(
        tap((result: any) => {
          if (result) {
            this.folderTreeChanged$.next(result.folders);
            this.itemsMoved$.next(result.movedItemsIds);
            return true;
          }
        })
      )
      .toPromise();
  }
  notifyStorageQuota(reached: boolean) {
    this.storageQuotaReached$.next(reached);
  }
  /** Create sub folder */
  async createSubFolder(subFolder: ICreateFolderModel): Promise<IFolderInfo> {
    return this.http.post<IFolderInfo>('folder', subFolder)
      .pipe(
        tap(newFolder => {
          this.subFolderCreated$.next(newFolder);
        }))
      .toPromise();
  }

  /** Rename a file */
  async renameFile(file: IRenameItemModel): Promise<IFileInfo> {
    return this.http.patch<IFileInfo>('file/rename', file)
      .pipe(tap(data => {
        this.fileRenamed$.next(data);
      }))
      .toPromise();
  }
  /** Rename a folder */
  async renameFolder(folder: IRenameItemModel): Promise<IFolderInfo> {
    return this.http.patch<IFolderInfo>('folder/rename', folder)
      .pipe(tap(data => {
        this.folderRenamed$.next(data);
      }))
      .toPromise();
  }

  async getFile(fileId: string): Promise<IFileInfo> {
    return this.http.get<IFileInfo>('file/requestdownload/' + fileId).toPromise();
  }

  async deleteItems(items: IItemInfo[]): Promise<IBulkDelete> {
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
    return this.http.request<IBulkDelete>('delete', 'file/deleteitems', { body: { selectedFiles: files, selectedFolders: folders } })
      .pipe(
        tap(items => {
          items.files.forEach(file => {
            this.fileDeleted$.next(file);
          });
          items.folders.forEach(folder => {
            this.subFolderDeleted$.next(folder.folders[folder.folders.length - 1]);
          });
        })
      )
      .toPromise();
  }
  async updateFileAvailability(file: IItemOnlineAvailability) {
    return this.http.patch<IFileInfo>('file/fileavailability', { fileid: file.itemId, available: file.available })
      .pipe(tap(data => this.fileAvailabilityChanged$.next(data)))
      .toPromise();
  }
  async updateFolderAvailability(folder: IItemOnlineAvailability) {
    return this.http.patch<IFolderInfo>('folder/folderavailability', { folderid: folder.itemId, available: folder.available })
      .pipe(tap(data => this.folderAvailabilityChanged$.next(data)))
      .toPromise();
  }

  async generateDownloadLink(fileid: string): Promise<IDownload> {
    return this.http.get<IDownload>('file/templink/' + fileid).toPromise();
  }

  async startDownload(url: string): Promise<Blob> {
    return this.http.get(url, {
      responseType: 'blob',
      reportProgress: true,
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
        Authorization: `Bearer ${this.storageService.jwt}`
      }
    })
      .toPromise();
  }

  async getUploadSetting(): Promise<IUploadSettings> {
    return this.http.get<IUploadSettings>('setting/uploadsetting').toPromise();
  }

  async getAccountOverview(): Promise<IAccountOverview> {
    return this.http.get<IAccountOverview>('setting/accountoverview').toPromise();
  }
}

interface IItemOnlineAvailability {
  itemId: string;
  available: boolean;
}
