import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IPage } from 'app/models';

@Injectable()
export class StaticPageService {
    constructor(private http: HttpClient) { }
    async getPage(url: string): Promise<IPage> {
        return this.http.get<IPage>('setting/page/' + url).toPromise();
    }
}
