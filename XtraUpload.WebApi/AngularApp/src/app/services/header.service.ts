import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { IAvatarData } from '../domain';

@Injectable()
export class HeaderService {
    private avatarChanged$ = new Subject<IAvatarData>();
    constructor() { }

    subscribeAvatarChange() {
        return this.avatarChanged$.asObservable();
    }
    notifyAvatarChanged(data: IAvatarData) {
        this.avatarChanged$.next(data);
    }
}
