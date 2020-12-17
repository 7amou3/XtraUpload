import { IAvatarData, IFileInfo, IHardwareOptions } from '.';
import * as tus from 'tus-js-client'
export class UploadStatus {
    instance: tus.Upload;
    status: 'Unknown' | 'ToDo' | 'InProgress' | 'Success' | 'Error';
    filename: string;
    fileId: string;
    message: object;
    uploadData: IAvatarData | IFileInfo;
    size: number;
  }
  export interface IUploadOptions {
    chunkSize: number;
    expiration: number;
    uploadPath: string;
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
  export interface IAddStorageServer {
    storageInfo: IStorageServer;
    uploadOpts: IUploadOptions;
    hardwareOpts: IHardwareOptions;
  }
  export interface IUpdateStorageServer extends IAddStorageServer {
    id: string;
  }