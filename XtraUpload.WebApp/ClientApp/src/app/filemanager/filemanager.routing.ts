import { Routes } from '@angular/router';
import { AuthGuardService } from '../services';
import { DashboardComponent } from './dashboard/dashboard.component';
export const FileManagerRoutes: Routes = [
  {
    path: 'filemanager',
    component: DashboardComponent,
    canActivate: [AuthGuardService],
    children: [
      {
        path: '/:folder',
        component: DashboardComponent
      }
    ]
  }
];
