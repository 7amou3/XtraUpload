import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
import { SharedModule } from '../shared-modules';
import { MessageComponent } from './message.component';

@NgModule({
  declarations: [
    MessageComponent,
  ],
  imports: [
    CommonModule,
    FlexLayoutModule,
    SharedModule
  ],
  exports: [
    MessageComponent
  ]
})
export class MessageModule { }
