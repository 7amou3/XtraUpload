import { IProfile, IStorageServer, IUploadSettings } from '.';

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

export interface IFileExtension {
  id: number;
  name: string;
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

export interface IFilteredUser {
  users: IProfile[];
}

export interface IEditExtension {
  id: number;
  newExt: string;
}
export interface IBulkDelete {
  files: IFileInfo [];
  folders: [{files: IFileInfo[], folders: IFolderInfo[]}];
}

export interface IHardwareOptions {
  memoryThreshold: number;
  storageThreshold: number;
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
