import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { FileManagerRoutes } from './filemanager.routing';
import { SharedModule } from 'app/shared/shared.module';
import { FileManagerService, UserStorageService, AuthGuardService, AuthService, UploadService } from 'app/services';
import { FileMngContextMenuService, SnavContextMenuService } from 'app/services/contextmenu';
import { DashboardComponent, TableviewComponent, GridviewComponent, IteminfoComponent,
         AppSidebarComponent, FoldersTreeComponent, UploadBottomSheetComponent} from './dashboard';
import { RenameItemComponent, PasswordComponent, DeleteItemComponent, CreatefolderComponent } from './dialogbox';
import { MoveItemComponent } from './dialogbox/moveitem/moveitem.component';

@NgModule({
  declarations: [
    DashboardComponent,
    TableviewComponent,
    GridviewComponent,
    IteminfoComponent,
    DeleteItemComponent,
    RenameItemComponent,
    PasswordComponent,
    AppSidebarComponent,
    FoldersTreeComponent,
    UploadBottomSheetComponent,
    CreatefolderComponent,
    MoveItemComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(FileManagerRoutes),
    SharedModule,
    FlexLayoutModule,
    DragDropModule,
    MatTooltipModule,
    ClipboardModule,
    NgxDropzoneModule
  ],
  providers: [
    AuthService,
    UploadService,
    AuthGuardService,
    FileManagerService,
    UserStorageService,
    SnavContextMenuService,
    FileMngContextMenuService
  ]
})
export class FilemanagerModule { }
