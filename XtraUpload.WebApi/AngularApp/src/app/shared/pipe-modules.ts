import {NgModule} from '@angular/core';
import { TruncatePipe, BytesPipe, CounterPipe, StorageStatePipe, LocalDatePipe } from '../shared';

@NgModule({
  declarations: [
    TruncatePipe,
    BytesPipe,
    CounterPipe,
    StorageStatePipe,
    LocalDatePipe
  ],
  exports: [
    TruncatePipe,
    BytesPipe,
    CounterPipe,
    StorageStatePipe,
    LocalDatePipe
  ]
})
export class PipeModule {}
