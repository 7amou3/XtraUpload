import { IProfile } from '.';

/** Represent a file or a folder */
export interface IItemInfo {
  id: string;
  name: string;
  thumbnail: string;
  createdAt: Date;
  lastModified: Date;
  hasPassword: boolean;
  status: boolean;
}

export interface IFileInfo extends IItemInfo {
  folderId: string;
  extension: string;
  mimeType: string;
  downloadCount: number;
  link: string;
  size: number;
  waitTime: number;
  userLoggedIn: boolean;
  storageServer: IStorageServer;
}

export interface IFileInfoExtended extends IFileInfo {
  userName: string;
}
export interface IPaging<T> {
  items: T[];
  totalItems: number;
  PageIndex: number;
}
export interface IFileExtension {
  id: number;
  name: string;
}
export interface IUserRoleClaims {
  role: IUserRole;
  claims: IRoleClaim[];
}
export interface IUserRole {
  id: number;
  name: string;
  isDefault: boolean;
}
export interface IRoleClaim {
  id: number;
  roleId: string;
  claimType: string;
  claimValue: number;
}
export class IClaims {
  groupName: string;
  adminAreaAccess: boolean;
  fileManagerAccess: boolean;
  concurrentUpload: number;
  downloadSpeed: number;
  downloadTTW: number;
  fileExpiration: number;
  maxFileSize: number;
  storageSpace: number;
  waitTime: number;
}

export interface IFolderInfo extends IItemInfo {
  parentid: string;
}

export interface IFolderModel {
  files: IFileInfo[];
  folders: IFolderInfo[];
}

export class MovedItemsModel {
  constructor(selectedFiles: IFileInfo[], selectedFolders: IFolderInfo[], destFolderId: string) {
    this.destFolderId = destFolderId;
    this.selectedFiles = selectedFiles;
    this.selectedFolders = selectedFolders;
  }
  destFolderId: string;
  selectedFiles: IFileInfo[];
  selectedFolders: IFolderInfo[];
}

export interface IRenameItemModel {
  fileId: string;
  newName: string;
}
export interface ICreateFolderModel {
  folderName: string;
  parentFolder: IFolderInfo;
}

export interface ISetPasswordItemModel {
  password: string;
}
export interface IFolderNode {
  id: string;
  name: string;
  children?: IFolderNode[];
}

export interface IItemsMenu {
  description: string;
  action: itemAction;
  icon: string;
  class?: string;
}

/** Flat node with expandable and level information */
export interface IFlatNode {
  expandable: boolean;
  name: string;
  level: number;
  id: string;
}

export class UploadStatus {
  status: 'Unknown' | 'ToDo' | 'InProgress' | 'Success' | 'Error';
  filename: string;
  fileId: string;
  message: object;
  uploadData: IAvatarData | IFileInfo;
  size: number;
}
export interface IDownload {
  downloadurl: string;
}
export interface IAvatarData {
  avatarUrl: string;
}
export interface IAccountOverview {
  uploadSetting: IUploadSettings;
  downloadSetting: IDownloadSettings;
  filesStats: IFilesStats;
  user: IProfile;
}
export interface IDownloadSettings {
  // Time to wait before download start
  timeToWait: number;
  // Time to wait before requesting a new download
  downloadTTW: number;
  fileExpiration: number;
  downloadSpeed: number;
}
export interface IFilesStats {
  totalFiles: number;
  totalDownloads: number;
}
export interface IUploadSettings {
  concurrentUpload: number;
  storageSpace: number;
  usedSpace: number;
  maxFileSize: number;
  chunkSize: number;
  fileExtensions: string;
  storageServer: IStorageServer;
}
export interface IStorageServer {
  id: string;
  address: string;
  state: serverState;
}
export enum serverState {
  Unknow = 0,
  Active,
  Passive,
  Disabled
}
export interface IFilteredUser {
  users: IProfile[];
}
export interface ISearchFile {
  start: Date;
  end: Date;
  user: ISearchUser;
  fileExtension: string;
}
export interface ISearchUser {
  id: string;
  userName: string;
}
export interface IEditExtension {
  id: number;
  newExt: string;
}
export interface IBulkDelete {
  files: IFileInfo [];
  folders: [{files: IFileInfo[], folders: IFolderInfo[]}];
}
export interface IUploadOptions {
  chunkSize: number;
  expiration: number;
  uploadPath: string;
}
export interface IHardwareOptions {
  memoryThreshold: number;
  storageThreshold: number;
}
export interface IAddStorageServer {
  storageInfo: IStorageServer;
  uploadOpts: IUploadOptions;
  hardwareOpts: IHardwareOptions;
}
export interface IUpdateStorageServer extends IAddStorageServer {
  id: string;
}
export enum itemAction {
  info = 0,
  rename,
  create,
  download,
  setPassword,
  unsetPassword,
  delete,
  openFolder,
  move
}
