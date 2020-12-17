import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PageEvent } from '@angular/material/paginator';
import { Subject } from 'rxjs';
import {
    IAdminOverView, IDateRange, IItemCount, IFileTypeCount,
    IHealthCheck, IPaging, IFileExtension, IFilteredUser, ISearchFile, IEditExtension, IFileInfo,
    IUserRole, IUserRoleClaims, IClaims, IFileInfoExtended, IEditProfile, IProfileClaim, IEmailSettings, IPage, IStorageServer, IUploadOptions, IHardwareOptions, IAddStorageServer, IUpdateStorageServer
} from 'app/models';

@Injectable()
export class AdminService {
    private isbusy$ = new Subject<boolean>();
    serviceBusy$ = this.isbusy$.asObservable();

    constructor(private http: HttpClient) { }
    notifyBusy(val: boolean): void {
        return this.isbusy$.next(val);
    }

    async Overview(dateRange: IDateRange): Promise<IAdminOverView> {
        const params = new HttpParams()
            .set('start', dateRange.start.toISOString())
            .set('end', dateRange.end.toISOString());

        return this.http.get<IAdminOverView>('admin/overview/', { params: params }).toPromise();
    }

    async uploadStats(dateRange: IDateRange): Promise<IItemCount[]> {
        const params = new HttpParams()
            .set('start', dateRange.start.toISOString())
            .set('end', dateRange.end.toISOString());

        return this.http.get<IItemCount[]>('admin/uploadstats/', { params: params }).toPromise();
    }
    async userStats(dateRange: IDateRange): Promise<IItemCount[]> {
        const params = new HttpParams()
            .set('start', dateRange.start.toISOString())
            .set('end', dateRange.end.toISOString());

        return this.http.get<IItemCount[]>('admin/userstats/', { params: params }).toPromise();
    }

    async filetypeStat(dateRange: IDateRange): Promise<IFileTypeCount[]> {
        const params = new HttpParams()
            .set('start', dateRange.start.toISOString())
            .set('end', dateRange.end.toISOString());

        return this.http.get<IFileTypeCount[]>('admin/filetypesstats/', { params: params }).toPromise();
    }

    async healthCheck(url: string): Promise<IHealthCheck> {
        return this.http.get<IHealthCheck>(url).toPromise();
    }

    async getFiles(pageEvent: PageEvent, search: ISearchFile): Promise<IPaging<IFileInfoExtended>> {
        let params = new HttpParams()
            .set('pageIndex', pageEvent?.pageIndex?.toString() ?? '0')
            .set('pageSize', pageEvent?.pageSize?.toString() ?? '50')
            .set('length', pageEvent?.length?.toString() ?? '0')
            .set('previousPageIndex', pageEvent?.previousPageIndex?.toString() ?? '0');
        if (search.start !== null) {
            params = params.set('start', search.start.toISOString());
        }
        if (search.end !== null) {
            params = params.set('end', search.end.toISOString());
        }
        if (search.fileExtension != null && search.fileExtension !== '') {
            params = params.set('fileExtension', search.fileExtension);
        }
        if (search.user !== null && search.user.id != null) {
            params = params.set('userId', search.user.id);
        }
        return this.http.get<IPaging<IFileInfoExtended>>('admin/files', { params: params }).toPromise();
    }

    async getUsers(pageEvent: PageEvent, search: ISearchFile): Promise<IPaging<IProfileClaim>> {
        let params = new HttpParams()
            .set('pageIndex', pageEvent?.pageIndex?.toString() ?? '0')
            .set('pageSize', pageEvent?.pageSize?.toString() ?? '50')
            .set('length', pageEvent?.length?.toString() ?? '0')
            .set('previousPageIndex', pageEvent?.previousPageIndex?.toString() ?? '0');
        if (search.start !== null) {
            params = params.set('start', search.start.toISOString());
        }
        if (search.end !== null) {
            params = params.set('end', search.end.toISOString());
        }
        if (search.user !== null && search.user.id != null) {
            params = params.set('userId', search.user.id);
        }
        return this.http.get<IPaging<IProfileClaim>>('admin/users', { params: params }).toPromise();
    }

    async getFileExtensions(): Promise<IFileExtension[]> {
        return this.http.get<IFileExtension[]>('admin/fileextensions').toPromise();
    }

    async searchUser(name: string): Promise<IFilteredUser> {
        if (name === undefined || name === '') {
            return Promise.resolve({} as IFilteredUser);
        }
        const params = new HttpParams().set('name', name);
        return this.http.get<IFilteredUser>('admin/searchusers', { params: params }).toPromise();
    }
    async addExtension(name: string): Promise<IFileExtension> {
        return this.http.post<IFileExtension>('admin/extension', { name: name }).toPromise();
    }
    async updateExtension(editedExt: IEditExtension): Promise<IFileExtension> {
        return this.http.patch<IFileExtension>('admin/extension', editedExt).toPromise();
    }
    async deleteExtension(item: IFileExtension): Promise<IFileExtension> {
        return this.http.delete<IFileExtension>('admin/extension/' + item.id).toPromise();
    }
    async deleteFiles(files: IFileInfo[]): Promise<IFileInfo[]> {
        return this.http.request<IFileInfo[]>('delete', 'admin/files/', { body: files.map(s => s.id) }).toPromise();
    }
    async getUsersGroups(): Promise<IUserRoleClaims[]> {
        return this.http.get<IUserRoleClaims[]>('admin/groups/').toPromise();
    }
    async addGroup(groupParams: IClaims) {
        return this.http.post<IUserRoleClaims>('admin/groups/', { role: { name: groupParams.groupName }, claims: groupParams }).toPromise();
    }
    async updateGroup(role: IUserRole, groupParams: IClaims) {
        return this.http.patch<IUserRoleClaims[]>('admin/groups/', { role: role, claims: groupParams }).toPromise();
    }
    async deleteGroup(roleId: number) {
        return this.http.delete('admin/groups/' + roleId).toPromise();
    }
    async deleteUsers(usersId: string[]) {
        return this.http.request('delete', 'admin/users/', { body: usersId }).toPromise();
    }
    async updateUser(user: IEditProfile): Promise<IProfileClaim> {
        return this.http.patch<IProfileClaim>('admin/user/', user).toPromise();
    }
    async getSettings() {
        return this.http.get('admin/appsettings/').toPromise();
    }
    async updateJwtOpts(jwtParams) {
        return this.http.patch('admin/jwtOptions/', jwtParams).toPromise();
    }

    async updateEmailOpts(emailParams: IEmailSettings) {
        return this.http.patch('admin/emailOptions/', {
            smtp: {
                server: emailParams.server,
                port: emailParams.port,
                username: emailParams.username,
                password: emailParams.password
            },
            sender: {
                name: emailParams.senderName,
                admin: emailParams.adminEmail,
                support: emailParams.supportEmail
            }
        })
        .toPromise();
    }
    async updateHardwareOpts(hardwareParams) {
        return this.http.patch('admin/hardwareOptions/', hardwareParams).toPromise();
    }
    async updateAppInfo(appInfoParams) {
        return this.http.patch('admin/appinfo/', appInfoParams).toPromise();
    }
    async updateSocialAuthSettings(socialAuthParams) {
        const params = {
            facebookAuth: { appId: socialAuthParams.facebookAppId },
            GoogleAuth: { clientId: socialAuthParams.googleClientId },
        };
        return this.http.patch('admin/socialAuthSettings/', params).toPromise();
    }
    async getPages(): Promise<IPage[]> {
        return this.http.get<IPage[]>('admin/pages').toPromise();
    }
    async addPage(addPage: IPage): Promise<IPage> {
        return this.http.post<IPage>('admin/page', addPage).toPromise();
    }
    async updatePage(editedPage: IPage): Promise<IPage> {
        return this.http.patch<IPage>('admin/page', editedPage).toPromise();
    }
    async deletePage(deletePage: IPage) {
        return this.http.delete('admin/page/' + deletePage.id).toPromise();
    }
    async getPage(url: string): Promise<IPage> {
        return this.http.get<IPage>('setting/page/' + url).toPromise();
    }
    async getStorageServers(): Promise<IStorageServer[]> {
        return this.http.get<IStorageServer[]>('admin/storageservers').toPromise();
    }
    async checkstorageconnectivity(address: string) {
        const params = new HttpParams()
            .set('address', address);
        return this.http.get('admin/checkstorageconnectivity', { params: params }).toPromise();
    }
    async getUploadConfigrConfig(address: string) {
        const params = new HttpParams()
            .set('address', address);
        return this.http.get('admin/uploadconfig', { params: params }).toPromise();
    }
    async getHardwareConfig(address: string) {
        const params = new HttpParams()
            .set('address', address);
        return this.http.get('admin/hardwareconfig', { params: params }).toPromise();
    }

    async addStorageServer(addserver: IAddStorageServer): Promise<IStorageServer> {
        return this.http.post<IStorageServer>('admin/storageserver', addserver).toPromise();
    }
    async updateStorageServer(updateServer: IUpdateStorageServer): Promise<IStorageServer> {
        return this.http.patch<IStorageServer>('admin/storageserver', updateServer).toPromise();
    }
    async deleteServer(server: IStorageServer): Promise<IStorageServer> {
        return this.http.delete<IStorageServer>('admin/storageserver/' + server.id).toPromise();
    }
}
