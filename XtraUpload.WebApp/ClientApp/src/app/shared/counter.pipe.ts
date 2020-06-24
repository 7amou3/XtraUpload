import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
 name: 'counter'
})

export class CounterPipe implements PipeTransform {

transform(timeInSeconds: number): string {
    const pad = function(num, size) { return ('000' + num).slice(size * -1); };
    const minutes = Math.floor(timeInSeconds / 60) % 60;
    const seconds = Math.floor(timeInSeconds - minutes * 60);

    return pad(minutes, 2) + ':' + pad(seconds, 2);
   }
}
