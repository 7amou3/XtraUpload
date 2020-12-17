import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
import { SharedModule } from 'app/shared/shared.module';
import { UserSettingsRoutes } from './user-setting.routing';
import { FileManagerService, SettingsService, UserStorageService, AuthGuardService, UploadService } from 'app/services';
import {SettingsComponent, OverviewComponent, UserinfoComponent, ChangePasswordComponent,
    AvatarComponent } from './';
import { ImageCropperModule } from 'ngx-image-cropper';
import { NgxDropzoneModule } from 'ngx-dropzone';

@NgModule({
    declarations: [
        SettingsComponent,
        OverviewComponent,
        AvatarComponent,
        ChangePasswordComponent,
        UserinfoComponent,
    ],
    imports: [
        CommonModule,
        RouterModule.forChild(UserSettingsRoutes),
        FlexLayoutModule,
        ImageCropperModule,
        NgxDropzoneModule,
        SharedModule
    ],
    providers: [
        SettingsService,
        AuthGuardService,
        UploadService,
        FileManagerService,
        UserStorageService
    ]
})
export class UserSettingsModule { }
