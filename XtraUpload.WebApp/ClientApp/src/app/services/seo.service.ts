import { Injectable } from '@angular/core';
import { UserStorageService } from './user.storage.service';
import { Title, Meta } from '@angular/platform-browser';
import { IWebSetting } from 'app/domain';

@Injectable({providedIn: 'root'})
export class SeoService {
    constructor(
        private titleService: Title,
        private meta: Meta,
        private userStorage: UserStorageService) {}
    setPageTitle(title: string) {
        const pageSetting = this.userStorage.getPageSetting();
        if (pageSetting) {
            title += ' - ' + pageSetting.title;
        }
        this.titleService.setTitle(title);
    }
    setMetaPage(webSetting: IWebSetting) {
        this.meta.addTag({name: 'description', content: webSetting.description});
        this.meta.addTag({name: 'keywords', content: webSetting.keywords});
    }
}
