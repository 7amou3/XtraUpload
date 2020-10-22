import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class SidenavService {
    private menuBtnClicked$ = new Subject();
    constructor() { }

    notifyMenuBtnClick() {
        this.menuBtnClicked$.next();
    }
    subscribeMenuBtnClick() {
        return this.menuBtnClicked$.asObservable();
    }
}
