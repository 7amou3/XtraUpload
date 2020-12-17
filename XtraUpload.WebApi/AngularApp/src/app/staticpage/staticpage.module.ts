import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { StaticPageRoutes } from './staticpage.routing';
import { FlexLayoutModule } from '@angular/flex-layout';
import { SharedModule } from 'app/shared/shared.module';
import { StaticPageService } from 'app/services';
import { PageComponent } from './page/page.component';
import { MarkdownModule } from 'ngx-markdown';

@NgModule({
  declarations: [
    PageComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild(StaticPageRoutes),
    MarkdownModule.forRoot(),
    FlexLayoutModule
  ],
  providers: [
    StaticPageService,
  ]
})
export class StaticPageModule { }
