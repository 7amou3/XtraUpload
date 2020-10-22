import { Routes } from '@angular/router';
import { FullComponent } from './layouts/full/full.component';
import { PageNotFoundComponent } from './layouts/page-not-found/page-not-found.component';

export const AppRoutes: Routes = [
  {
    path: '',
    component: FullComponent,
    children: [
      {
        path: '',
        loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule)
      },
      {
        path: '',
        loadChildren: () => import('./filemanager/filemanager.module').then(m => m.FilemanagerModule)
      },
      {
        path: '',
        loadChildren: () => import('./download/download.module').then(m => m.DownloadModule)
      },
      {
        path: '',
        loadChildren: () => import('./user-settings/user-settings.module').then(m => m.UserSettingsModule)
      },
      {
        path: '',
        loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule)
      },
      {
        path: '',
        loadChildren: () => import('./staticpage/staticpage.module').then(m => m.StaticPageModule)
      },
      {
        path: '**',
        component: PageNotFoundComponent
      }
    ]
  }
];
