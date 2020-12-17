import {NgModule} from '@angular/core';
import { TruncatePipe } from './truncate.pipe';
import { BytesPipe } from './bytes.pipe';
import { CounterPipe } from './counter.pipe';
import { StorageStatePipe } from './storage.state.pipe';
import { LocalDatePipe } from './local.date.pipe';

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
