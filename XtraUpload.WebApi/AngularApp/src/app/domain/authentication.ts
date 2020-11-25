export interface ILoginParams {
  userName: string;
  password: string;
  rememberMe: boolean;
}

export interface ISignupParams extends ILoginParams {
  email: string;
  termsOfservice: boolean;
  language: string;
}

export interface IProfile {
  id: string;
  userName: string;
  email: string;
  emailConfirmed: boolean;
  accountSuspended: boolean;
  createdAt: Date;
  lastModified: Date;
  avatar: string;
  jwtToken: JwtToken;
  role: string;
  itemsDisplay: 'list' | 'grid';
  theme: 'dark' | 'light';
  language: string;
}
export interface IProfileClaim extends IProfile {
  roleName: string;
}
export interface IEditProfile {
  id: string;
  userName: string;
  email: string;
  newPassword: string;
  emailConfirmed: boolean;
  suspendAccount: boolean;
  roleId: number;
}
export interface JwtToken {
  token: string;
  Expires_in: number;
}
export interface IGenericMessage {
  errorMessage?: string;
  successMessage?: string;
}
export class RecoverPassword {
  newPassword: string;
  recoveryKey: string;
}
