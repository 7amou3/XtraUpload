import { Component, OnInit } from '@angular/core';
import { IPageHeader } from 'app/models';
import { UserStorageService } from 'app/services';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})

export class FooterComponent implements OnInit {
  pageName: string;
  footerLinks: IPageHeader[];
  constructor(
    private storageService: UserStorageService) {
   }
  currentYear = new Date();
  ngOnInit(): void {
    this.pageName = this.storageService.appinfo.title;
    this.footerLinks = this.storageService.pagelinks;
  }

}
