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
  export interface IPaging<T> {
    items: T[];
    totalItems: number;
    PageIndex: number;
  }