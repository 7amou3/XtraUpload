import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { StaticPageRoutes } from './staticpage.routing';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { StaticPageService } from '../services';
import { PageComponent } from './page/page.component';

@NgModule({
  declarations: [
    PageComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(StaticPageRoutes),
    FlexLayoutModule,
    MatIconModule,
    MatCardModule,
    MatDividerModule,
    MatProgressBarModule
  ],
  providers: [
    StaticPageService,
  ]
})
export class StaticPageModule { }
