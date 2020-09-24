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
import { MatTableModule } from '@angular/material/table';
import { MatDialogModule } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatBottomSheetModule } from '@angular/material/bottom-sheet';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { FileManagerRoutes } from './filemanager.routing';
import { PipeModule } from 'app/shared/pipe-modules';
import { FileManagerService, UserStorageService, AuthGuardService, AuthService } from 'app/services';
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
    MatTableModule,
    MatDialogModule,
    MatMenuModule,
    MatSlideToggleModule,
    MatBottomSheetModule,
    DragDropModule,
    MatTooltipModule,
    FormsModule,
    ReactiveFormsModule,
    NgxDropzoneModule
  ],
  providers: [
    AuthService,
    AuthGuardService,
    FileManagerService,
    UserStorageService,
    SnavContextMenuService,
    FileMngContextMenuService
  ]
})
export class FilemanagerModule { }
