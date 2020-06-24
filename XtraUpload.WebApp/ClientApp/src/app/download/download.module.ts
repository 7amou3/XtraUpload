import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SharedModule, MessageModule } from '../shared';
import { PipeModule } from '../shared/pipe-modules';
import { DownloadRoutes } from './download.routing';
import { FileManagerService } from 'app/services';
import { FileComponent } from './file/file.component';
import { FolderComponent } from './folder/folder.component';
import { SidebarComponent } from './folder/sidebar/sidebar.component';
import { TableviewComponent } from './folder/tableview/tableview.component';
import { FileMngContextMenuService, SnavContextMenuService } from 'app/services/contextmenu';

@NgModule({
  declarations: [
    FileComponent,
    FolderComponent,
    SidebarComponent,
    TableviewComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(DownloadRoutes),
    FlexLayoutModule,
    SharedModule,
    MessageModule,
    PipeModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    FileManagerService,
    FileMngContextMenuService,
    SnavContextMenuService
  ]
})
export class DownloadModule { }
