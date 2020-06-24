import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { StaticPageRoutes } from './staticpage.routing';
import { FlexLayoutModule } from '@angular/flex-layout';
import { SharedModule } from '../shared';
import { StaticPageService } from '../services';
import { NgxWigModule } from 'ngx-wig';
import { PageComponent } from './page/page.component';

@NgModule({
  declarations: [
    PageComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(StaticPageRoutes),
    FlexLayoutModule,
    SharedModule,
    NgxWigModule
  ],
  providers: [
    StaticPageService,
  ]
})
export class StaticPageModule { }
