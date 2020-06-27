import { Component, OnInit } from '@angular/core';
import { UserStorageService } from 'app/services';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html'
})
export class FooterComponent implements OnInit {
  pageName: string;
  constructor(private storageService: UserStorageService) {
   }
  currentYear = new Date();
  ngOnInit(): void {
    this.pageName = this.storageService.getPageSetting().title;
  }

}
