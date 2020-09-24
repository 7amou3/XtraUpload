import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IPage } from 'app/domain';

@Injectable()
export class StaticPageService {
    constructor(private http: HttpClient) { }
    getPage(name: string) {
        return this.http.get<IPage>('setting/page/' + name);
    }
}
