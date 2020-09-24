import { Routes } from '@angular/router';
import { AuthGuardService } from '../services';
import { SettingsComponent, OverviewComponent, UserinfoComponent, ChangePasswordComponent,
          AvatarComponent } from './';
export const UserSettingsRoutes: Routes = [
  {
    path: 'settings',
    component: SettingsComponent,
    canActivate: [AuthGuardService],
    children: [
      {
        path: '',
        redirectTo: 'overview'
      },
      {
        path: 'overview',
        component: OverviewComponent
      },
      {
        path: 'info',
        component: UserinfoComponent
      },
      {
        path: 'password',
        component: ChangePasswordComponent
      },
      {
        path: 'avatar',
        component: AvatarComponent
      }
    ]
  }
];
