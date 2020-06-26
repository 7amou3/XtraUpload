export interface IWebSetting {
  title: string;
  description: string;
  expire: Date;
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
export interface IPage {
  id: string;
  name: string;
  url: string;
  content: string;
  createdAt: Date;
  updatedAt: Date;
}
