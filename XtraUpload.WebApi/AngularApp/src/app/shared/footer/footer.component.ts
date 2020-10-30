import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { IPageHeader } from 'app/domain';
import { UserStorageService } from 'app/services';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})

export class FooterComponent implements OnInit {
  pageName: string;
  footerLinks: IPageHeader[];
  constructor(
    private router: Router,
    private storageService: UserStorageService) {
   }
  currentYear = new Date();
  ngOnInit(): void {
    this.pageName = this.storageService.getAppInfo().title;
    this.footerLinks = this.storageService.getPageLinks();
  }

}
