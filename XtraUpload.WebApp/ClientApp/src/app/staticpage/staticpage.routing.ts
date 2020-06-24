import { Routes } from '@angular/router';
import { PageComponent } from './page/page.component';

export const StaticPageRoutes: Routes = [
  { path: 'page/:name', component: PageComponent },
];
