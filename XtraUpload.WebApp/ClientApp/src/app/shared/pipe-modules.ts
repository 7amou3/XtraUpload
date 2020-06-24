import {NgModule} from '@angular/core';
import { TruncatePipe, BytesPipe, CounterPipe } from '../shared';

@NgModule({
  declarations: [
    TruncatePipe,
    BytesPipe,
    CounterPipe
  ],
  exports: [
    TruncatePipe,
    BytesPipe,
    CounterPipe
  ]
})
export class PipeModule {}
