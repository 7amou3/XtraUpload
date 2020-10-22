import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { FooterComponent } from './footer.component';
/**The footer modul was created because we don't want to show the footer in evry page of the app, specially in filemanager area */
@NgModule({
  declarations: [
    FooterComponent,
  ],
  imports: [
    CommonModule,
    FlexLayoutModule,
    RouterModule,
    MatIconModule,
    MatButtonModule
  ],
  exports: [
    FooterComponent
  ]
})
export class FooterModule { }
