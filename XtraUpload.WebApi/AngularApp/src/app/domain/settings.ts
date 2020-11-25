import { SocialUser } from "angularx-social-login";

export interface IAppInitializerConfig {
  appInfo: IWebAppInfo;
  pagesHeader: IPageHeader[];
  version: string;
}
export interface IWebAppInfo {
  title: string;
  description: string;
  keywords: string;
  version: string;
}
export interface IChangePassword {
  oldPassword: string;
  newPassword: string;
}

export interface IAdminOverView {
  driveSize: number;
  freeSpace: number;
  totalFiles: number;
  totalUsers: number;
  filesCount: IItemCount[];
  usersCount: IItemCount[];
  fileTypesCount: IFileTypeCount[];
}

export interface IItemCount {
  date: Date;
  itemCount: number;
}
export interface IFileTypeCount {
  fileType: number; // enum
  itemCount: number;
}
export interface IDateRange {
  start: Date;
  end: Date;
}

export interface IHealthCheck {
  status: string;
  duration: number;
  checks: ICheckResource[];
}
export interface ICheckResource {
  component: string;
  status: string;
  description: string;
}
export interface IEmailSettings {
  port: number;
  server: string;
  adminEmail: string;
  supportEmail: string;
  password: string;
  username: string;
  senderName: string;
}
export interface IPageHeader {
  id: string;
  name: string;
  url: string;
  visibleInFooter: boolean;
  createdAt: Date;
  updatedAt: Date;
}
export interface ILanguage {
  name: string;
  culture: string;
}
export interface IExtendedSocialUser extends SocialUser {
  language: string;
}
export interface IPage extends IPageHeader {
  content: string;
}
