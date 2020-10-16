import { Routes } from '@angular/router';
import { AuthAdminGuardService } from '../services';
import { DashboardComponent } from './dashboard/dashboard.component';
import { OverviewComponent } from './overview/overview.component';
import { UserListComponent } from './users/users.component';
import { GroupsComponent } from './users/groups/groups.component';
import { FilesComponent } from './files/files.component';
import { ExtensionsComponent } from './files/extensions/extensions.component';
import { SettingsComponent } from './settings/settings.component';
import { PagesComponent } from './pages/pages.component';
import { ServersComponent } from './servers/servers.component';

export const AdminRoutes: Routes = [
  {
    path: 'administration',
    component: DashboardComponent,
    canActivate: [AuthAdminGuardService],
    children: [
      { path: '', redirectTo: 'overview' },
      { path: 'overview', component: OverviewComponent },
      { path: 'users', component: UserListComponent },
      { path: 'groups', component: GroupsComponent },
      { path: 'files', component: FilesComponent },
      { path: 'extensions', component: ExtensionsComponent },
      { path: 'settings', component: SettingsComponent },
      { path: 'pages', component: PagesComponent },
      { path: 'servers', component: ServersComponent }
    ]
  }
];
