import { Pipe, PipeTransform } from '@angular/core';
import { isNumeric } from 'rxjs/internal-compatibility';

@Pipe({
    name: 'storagestate'
})

export class StorageStatePipe implements PipeTransform {

    transform(state: number): string {
        var result = 'Unknown';
        if (!isNumeric(state)) {
            return result;
        }
        switch (state) {
            case 1:
                result = $localize`Active`; break;
            case 2:
                result = $localize`Passive`; break;
            case 3:
                result = $localize`Disabled`; break;
        }
        return result;
    }
}
