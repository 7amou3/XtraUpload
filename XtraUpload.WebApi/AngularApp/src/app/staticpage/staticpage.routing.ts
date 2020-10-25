import { Routes } from '@angular/router';
import { PageComponent } from './page/page.component';

export const StaticPageRoutes: Routes = [
  { path: 'page/:url', component: PageComponent },
];
