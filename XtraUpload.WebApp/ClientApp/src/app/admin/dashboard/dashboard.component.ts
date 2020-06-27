import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { MatSidenav } from '@angular/material';
import { ComponentBase } from 'app/shared';
import { MediaMatcher } from '@angular/cdk/layout';
import { AdminService, SidenavService, SeoService } from 'app/services';
import { takeUntil, finalize, delay } from 'rxjs/operators';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent extends ComponentBase implements OnInit {
  mobileQuery: MediaQueryList;
  private _mobileQueryListener: () => void;
  @ViewChild('snav') sidenav: MatSidenav;
  private readonly pageTitle = 'Administration';
  subTitle = '';
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    media: MediaMatcher,
    private adminService: AdminService,
    private sidenavService: SidenavService,
    private seoService: SeoService
  ) {
    super();
    seoService.setPageTitle(this.pageTitle);
    this.mobileQuery = media.matchMedia('(min-width: 768px)');
    this._mobileQueryListener = () => changeDetectorRef.detectChanges();
    this.mobileQuery.addListener(this._mobileQueryListener);
  }

  ngOnInit(): void {
    this.adminService.serviceBusy$
    .pipe(
      takeUntil(this.onDestroy),
      delay(0),
      finalize(() => this.isBusy = false))
    .subscribe(isbusy => {
      this.isBusy = isbusy;
    });
    // Subscribe to header menu click event
    this.sidenavService.subscribeMenuBtnClick()
    .pipe(takeUntil(this.onDestroy))
    .subscribe(() => {
      this.sidenav.toggle();
    });
  }
  ngOnDestroy(): void {
    this.mobileQuery.removeListener(this._mobileQueryListener);
    super.ngOnDestroy();
  }
  onActivate(ev) {
    switch (ev.constructor.name) {
      case 'OverviewComponent':
        this.subTitle = 'Dashboard'; break;
      case 'FilesComponent':
        this.subTitle = 'Files'; break;
      case 'UserListComponent':
        this.subTitle = 'Users'; break;
      case 'SettingsComponent':
        this.subTitle = 'Settings'; break;
      case 'GroupsComponent':
        this.subTitle = 'User group'; break;
      case 'ExtensionsComponent':
        this.subTitle = 'Extensions'; break;
      case 'PagesComponent':
        this.subTitle = 'Pages'; break;
      default:
        this.subTitle = 'Dashboard'; break;
    }
  }

}
