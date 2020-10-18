import {NgModule} from '@angular/core';
import { TruncatePipe, BytesPipe, CounterPipe, StorageStatePipe } from '../shared';

@NgModule({
  declarations: [
    TruncatePipe,
    BytesPipe,
    CounterPipe,
    StorageStatePipe
  ],
  exports: [
    TruncatePipe,
    BytesPipe,
    CounterPipe,
    StorageStatePipe
  ]
})
export class PipeModule {}
