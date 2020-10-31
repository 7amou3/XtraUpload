import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PageEvent } from '@angular/material/paginator';
import { Observable, Subject } from 'rxjs';
import { IAdminOverView, IDateRange, IItemCount, IFileTypeCount,
    IHealthCheck, IPaging, IFileExtension, IFilteredUser, ISearchFile, IEditExtension, IFileInfo,
    IUserRole, IUserRoleClaims, IClaims, IFileInfoExtended, IEditProfile, IProfileClaim, IEmailSettings, IPage, IStorageServer, IUploadOptions, IHardwareOptions, IAddStorageServer, IUpdateStorageServer } from 'app/domain';

@Injectable()
export class AdminService {
    private isbusy$ = new Subject<boolean>();
    serviceBusy$ = this.isbusy$.asObservable();

    constructor(private http: HttpClient) { }
    notifyBusy(val: boolean): void {
        return this.isbusy$.next(val);
    }

    Overview(dateRange: IDateRange): Observable<IAdminOverView> {
        const params = new HttpParams()
            .set('start', dateRange.start.toISOString())
            .set('end', dateRange.end.toISOString());

        return this.http.get<IAdminOverView>('admin/overview/', { params: params });
    }

    uploadStats(dateRange: IDateRange): Observable<IItemCount[]> {
        const params = new HttpParams()
            .set('start', dateRange.start.toISOString())
            .set('end', dateRange.end.toISOString());

        return this.http.get<IItemCount[]>('admin/uploadstats/', { params: params });
    }
    userStats(dateRange: IDateRange): Observable<IItemCount[]> {
        const params = new HttpParams()
            .set('start', dateRange.start.toISOString())
            .set('end', dateRange.end.toISOString());

        return this.http.get<IItemCount[]>('admin/userstats/', { params: params });
    }

    filetypeStat(dateRange: IDateRange): Observable<IFileTypeCount[]> {
        const params = new HttpParams()
            .set('start', dateRange.start.toISOString())
            .set('end', dateRange.end.toISOString());

        return this.http.get<IFileTypeCount[]>('admin/filetypesstats/', { params: params });
    }

    healthCheck(url: string): Observable<IHealthCheck> {
        return this.http.get<IHealthCheck>(url);
    }

    getFiles(pageEvent: PageEvent, search: ISearchFile): Observable<IPaging<IFileInfoExtended>> {
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
        return this.http.get<IPaging<IFileInfoExtended>>('admin/files', { params: params });
    }

    getUsers (pageEvent: PageEvent, search: ISearchFile): Observable<IPaging<IProfileClaim>> {
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
        return this.http.get<IPaging<IProfileClaim>>('admin/users', { params: params });
    }

    getFileExtensions(): Observable<IFileExtension[]> {
        return this.http.get<IFileExtension[]>('admin/fileextensions');
    }

    searchUser(name: string): Observable<IFilteredUser> {
        if (name === undefined || name === '') {
            return new Subject<IFilteredUser>();
        }
        const params = new HttpParams().set('name', name);
        return this.http.get<IFilteredUser>('admin/searchusers', { params: params });
    }
    addExtension(name: string): Observable<IFileExtension> {
        return this.http.post<IFileExtension>('admin/extension', {name: name});
    }
    updateExtension(editedExt: IEditExtension): Observable<IFileExtension> {
        return this.http.patch<IFileExtension>('admin/extension', editedExt);
    }
    deleteExtension(item: IFileExtension): Observable<IFileExtension> {
        return this.http.delete<IFileExtension>('admin/extension/' + item.id);
    }
    deleteFiles(files: IFileInfo[]): Observable<IFileInfo[]> {
        return this.http.request<IFileInfo[]>('delete', 'admin/files/', { body: files.map(s => s.id) });
    }
    getUsersGroups(): Observable<IUserRoleClaims[]> {
        return this.http.get<IUserRoleClaims[]>('admin/groups/');
    }
    addGroup(groupParams: IClaims) {
        return this.http.post<IUserRoleClaims>('admin/groups/', {role: {name: groupParams.groupName }, claims: groupParams});
    }
    updateGroup(role: IUserRole, groupParams: IClaims) {
        return this.http.patch<IUserRoleClaims[]>('admin/groups/', {role: role, claims: groupParams});
    }
    deleteGroup(roleId: number) {
        return this.http.delete('admin/groups/' + roleId);
    }
    deleteUsers(usersId: string[]) {
        return this.http.request('delete', 'admin/users/', { body: usersId });
    }
    updateUser(user: IEditProfile): Observable<IProfileClaim> {
        return this.http.patch<IProfileClaim>('admin/user/', user);
    }
    getSettings() {
        return this.http.get('admin/appsettings/');
    }
    updateJwtOpts(jwtParams) {
        return this.http.patch('admin/jwtOptions/', jwtParams);
    }
    updateUploadOpts(uploadParams) {
        return this.http.patch('admin/uploadOptions/', uploadParams);
    }
    updateEmailOpts(emailParams: IEmailSettings) {
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
        } );
    }
    updateHardwareOpts(hardwareParams) {
        return this.http.patch('admin/hardwareOptions/', hardwareParams);
    }
    updateAppInfo(appInfoParams) {
        return this.http.patch('admin/appinfo/', appInfoParams);
    }
    updateSocialAuthSettings(socialAuthParams) {
        const params = {
            facebookAuth: {appId: socialAuthParams.facebookAppId},
            GoogleAuth: {clientId: socialAuthParams.googleClientId},
        };
        return this.http.patch('admin/socialAuthSettings/', params);
    }
    getPages(): Observable<IPage[]> {
        return this.http.get<IPage[]>('admin/pages');
    }
    addPage(addPage: IPage) {
        return this.http.post<IPage>('admin/page', addPage);
    }
    updatePage(editedPage: IPage) {
        return this.http.patch<IPage>('admin/page', editedPage);
    }
    deletePage(deletePage: IPage) {
        return this.http.delete('admin/page/' + deletePage.id);
    }
    getPage(url: string): Observable<IPage> {
        return this.http.get<IPage>('setting/page/' + url);
    }
    getStorageServers(): Observable<IStorageServer[]> {
        return this.http.get<IStorageServer[]>('admin/storageservers');
    }
    checkstorageconnectivity(address: string) {
        const params = new HttpParams()
            .set('address', address);
        return this.http.get('admin/checkstorageconnectivity', {params: params});
    }
    getUploadConfigrConfig(address: string) {
        const params = new HttpParams()
            .set('address', address);
        return this.http.get('admin/uploadconfig', {params: params});
    }
    getHardwareConfig(address: string) {
        const params = new HttpParams()
            .set('address', address);
        return this.http.get('admin/hardwareconfig', {params: params});
    }

    addStorageServer(addserver: IAddStorageServer): Observable<IStorageServer> {
        return this.http.post<IStorageServer>('admin/storageserver', addserver);
    }
    updateStorageServer(updateServer: IUpdateStorageServer): Observable<IStorageServer>{
        return this.http.patch<IStorageServer>('admin/storageserver', updateServer);
    }
    deleteServer(server: IStorageServer): Observable<IStorageServer> {
        return this.http.delete<IStorageServer>('admin/storageserver/'+server.id);
    }
}
