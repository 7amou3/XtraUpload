import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from './material-module/material.module';
import { FooterComponent, SpinnerComponent, MessageComponent, PageNotFoundComponent } from './components';
import { PipeModule, IsLoggedInDirective} from './helpers';

@NgModule({
  imports: [
    PipeModule,
    CommonModule,
    MaterialModule 
  ],
  declarations: [
    MessageComponent,
    SpinnerComponent,
    FooterComponent,
    IsLoggedInDirective,
    PageNotFoundComponent
  
  ],
  exports: [
    PipeModule,
    MessageComponent,
    SpinnerComponent,
    FooterComponent,
    IsLoggedInDirective,
    PageNotFoundComponent,
    MaterialModule
  ]
})

export class SharedModule { }