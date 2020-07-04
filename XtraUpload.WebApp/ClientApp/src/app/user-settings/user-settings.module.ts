import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatTreeModule } from '@angular/material/tree';
import { MatListModule } from '@angular/material/list';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MessageModule } from 'app/shared';
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
        MessageModule,
        PipeModule,
        FlexLayoutModule,
        MatFormFieldModule,
        MatSelectModule,
        MatInputModule,
        MatCardModule,
        MatIconModule,
        MatDividerModule,
        MatProgressBarModule,
        MatButtonModule,
        MatProgressSpinnerModule,
        MatSidenavModule,
        MatTreeModule,
        MatListModule,
        MatTooltipModule,
        FormsModule,
        ReactiveFormsModule,
        ImageCropperModule,
        NgxDropzoneModule
    ],
    providers: [
        SettingsService,
        AuthGuardService,
        FileManagerService,
        UserStorageService
    ]
})
export class UserSettingsModule { }
