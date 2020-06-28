import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable()
export class HeaderService {
    private avatarChanged$ = new Subject();
    constructor(private http: HttpClient) { }

    subscribeAvatarChange() {
        return this.avatarChanged$.asObservable();
    }
    notifyAvatarChanged() {
        this.avatarChanged$.next();
    }
    getAvatarUrl() {
        return this.http.get<string>('file/avatarurl')
        .pipe(
            map((result: any) => result.url)
        );
    }
}
