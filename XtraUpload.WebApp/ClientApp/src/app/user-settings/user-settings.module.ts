import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule, MessageModule } from 'app/shared';
import { PipeModule } from 'app/shared/pipe-modules';
import { UserSettingsRoutes } from './user-setting.routing';
import { FileManagerService, SettingsService, UserStorageService, AuthGuardService } from 'app/services';
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
        SharedModule,
        MessageModule,
        PipeModule,
        FlexLayoutModule,
        FormsModule,
        ReactiveFormsModule,
        ImageCropperModule,
        NgxDropzoneModule
    ],
    providers: [
        SettingsService,
        AuthGuardService,
        FileManagerService,
        UserStorageService,
    ]
})
export class UserSettingsModule { }
