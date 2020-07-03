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
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatExpansionModule } from '@angular/material/expansion';
import {CdkAccordionModule} from '@angular/cdk/accordion';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MessageModule } from 'app/shared';
import { AdminRoutes } from './admin.routing';
import { PipeModule } from 'app/shared/pipe-modules';
import { ChartsModule } from 'ng2-charts';
import { AdminService, UserStorageService, AuthAdminGuardService, AuthService } from 'app/services';
import { FileMngContextMenuService, SnavContextMenuService } from 'app/services/contextmenu';
import { DashboardComponent } from './dashboard/dashboard.component';
import { OverviewComponent } from './overview/overview.component';
import { FileuploadsComponent } from './overview/charts/linechart/fileuploads.component';
import { UsersComponent } from './overview/charts/linechart/users.component';
import { PiechartComponent } from './overview/charts/piechart/piechart.component';
import { HealthchekComponent } from './overview/healthchek/healthchek.component';
import { FilesComponent } from './files/files.component';
import { UserListComponent} from './users/users.component';
import { SettingsComponent } from './settings/settings.component';
import { SidebarComponent } from './dashboard/sidebar/sidebar.component';
import { ExtensionsComponent } from './files/extensions/extensions.component';
import { GroupsComponent } from './users/groups/groups.component';
import { EditComponent } from './files/extensions/dialogs/edit/edit.component';
import { DeleteComponent } from './files/extensions/dialogs/delete/delete.component';
import { AddComponent } from './files/extensions/dialogs/add/add.component';
import { DeleteFileComponent } from './files/delete-file/delete-file.component';
import { AddgroupComponent } from './users/groups/dialogs/addgroup/addgroup.component';
import { EditgroupComponent } from './users/groups/dialogs/editgroup/editgroup.component';
import { DeletegroupComponent } from './users/groups/dialogs/deletegroup/deletegroup.component';
import { EdituserComponent } from './users/dialogs/edituser/edituser.component';
import { DeleteuserComponent } from './users/dialogs/deleteuser/deleteuser.component';
import { PagesComponent } from './pages/pages.component';
import { EditpageComponent } from './pages/dialogs/editpage/editpage.component';
import { DeletepageComponent } from './pages/dialogs/deletepage/deletepage.component';
import {NgxWigModule} from 'ngx-wig';
import { AddpageComponent } from './pages/dialogs/addpage/addpage.component';

@NgModule({
  declarations: [
    DashboardComponent,
    OverviewComponent,
    FileuploadsComponent,
    UsersComponent,
    PiechartComponent,
    HealthchekComponent,
    FilesComponent,
    UserListComponent,
    SettingsComponent,
    SidebarComponent,
    ExtensionsComponent,
    GroupsComponent,
    EditComponent,
    DeleteComponent,
    AddComponent,
    DeleteFileComponent,
    AddgroupComponent,
    EditgroupComponent,
    DeletegroupComponent,
    EdituserComponent,
    DeleteuserComponent,
    PagesComponent,
    EditpageComponent,
    DeletepageComponent,
    AddpageComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(AdminRoutes),
    MessageModule,
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
    MatTooltipModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatExpansionModule,
    CdkAccordionModule,
    MatAutocompleteModule,
    MatCheckboxModule,
    ChartsModule,
    PipeModule,
    FlexLayoutModule,
    FormsModule,
    ReactiveFormsModule,
    NgxWigModule
  ],
  providers: [
    AuthService,
    AuthAdminGuardService,
    AdminService,
    UserStorageService,
    SnavContextMenuService,
    FileMngContextMenuService
  ]
})
export class AdminModule { }
