import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IPage, IPageHeader } from 'app/domain';
import { Observable } from 'rxjs';

@Injectable()
export class StaticPageService {
    constructor(private http: HttpClient) { }
    getPage(url: string): Observable<IPage> {
        return this.http.get<IPage>('setting/page/' + url);
    }
}
