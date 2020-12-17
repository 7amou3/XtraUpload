import { Routes } from '@angular/router';
import { ContainerComponent } from './layout/container/container.component';
import { PageNotFoundComponent } from './shared/components/page-not-found/page-not-found.component';

export const AppRoutes: Routes = [
  {
    path: '',
    component: ContainerComponent,
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
