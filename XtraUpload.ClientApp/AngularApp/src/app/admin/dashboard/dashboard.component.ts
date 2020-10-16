import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { MediaMatcher } from '@angular/cdk/layout';
import { Router, NavigationEnd } from '@angular/router';
import { takeUntil, finalize, delay, filter } from 'rxjs/operators';
import { AdminService, SidenavService, SeoService } from 'app/services';
import { ComponentBase } from 'app/shared';

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
    private seoService: SeoService,
    private router: Router
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
    // Subscribe to navigation to set component title
    this.setSubPageTitle(this.router.url);
    this.router.events
        .pipe(filter(e => e instanceof NavigationEnd), takeUntil(this.onDestroy))
        .subscribe( (event: NavigationEnd) => {
            this.setSubPageTitle(event.urlAfterRedirects);
          }
        );
  }
  ngOnDestroy(): void {
    this.mobileQuery.removeListener(this._mobileQueryListener);
    super.ngOnDestroy();
  }
  setSubPageTitle(title: string) {
    switch (title) {
      case '/administration/overview':
        this.subTitle = 'Dashboard'; break;
      case '/administration/files':
        this.subTitle = 'Files'; break;
      case '/administration/users':
        this.subTitle = 'Users'; break;
      case '/administration/settings':
        this.subTitle = 'Settings'; break;
      case '/administration/groups':
        this.subTitle = 'User group'; break;
      case '/administration/extensions':
        this.subTitle = 'Extensions'; break;
      case '/administration/pages':
        this.subTitle = 'Pages'; break;
      case '/administration/servers':
          this.subTitle = 'Storage Servers'; break;
      default:
        this.subTitle = 'Dashboard'; break;
    }
  }

}
