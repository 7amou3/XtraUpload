import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatTreeModule } from '@angular/material/tree';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MessageModule, FooterModule } from '../shared';
import { PipeModule } from '../shared/pipe-modules';
import { DownloadRoutes } from './download.routing';
import { FileManagerService, UploadService } from 'app/services';
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
    MessageModule,
    FooterModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatCheckboxModule,
    MatCardModule,
    MatIconModule,
    MatDividerModule,
    MatProgressBarModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSidenavModule,
    MatDialogModule,
    MatTreeModule,
    MatTableModule,
    MatTooltipModule,
    PipeModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    FileManagerService,
    UploadService,
    FileMngContextMenuService,
    SnavContextMenuService
  ]
})
export class DownloadModule { }
