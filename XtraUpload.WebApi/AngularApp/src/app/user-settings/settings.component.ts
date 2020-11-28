import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { MediaMatcher } from '@angular/cdk/layout';
import { takeUntil, filter } from 'rxjs/operators';
import { Router, NavigationEnd } from '@angular/router';
import { SidenavService, SeoService } from 'app/services';
import { ComponentBase } from 'app/shared';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent extends ComponentBase implements OnInit {
  mobileQuery: MediaQueryList;
  private _mobileQueryListener: () => void;
  private readonly pageTitle = $localize`Settings`;
  @ViewChild('sidenav') sidenav: MatSidenav;
  subTitle = '';
  constructor(
    private sidenavService: SidenavService,
    changeDetectorRef: ChangeDetectorRef,
    media: MediaMatcher,
    private router: Router,
    private seoService: SeoService
  ) {
    super();
    seoService.setPageTitle(this.pageTitle);
    this.mobileQuery = media.matchMedia('(min-width: 768px)');
    this._mobileQueryListener = () => changeDetectorRef.detectChanges();
    this.mobileQuery.addListener(this._mobileQueryListener);
  }

  ngOnInit(): void {
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
      case '/settings/overview':
        this.subTitle = $localize`Overview`;
        break;
      case '/settings/avatar':
        this.subTitle = $localize`Avatar`;
        break;
      case '/settings/password':
        this.subTitle = $localize`Password`;
        break;
      /*case '/settings/userinfo':
        this.subTitle = $localize`User Info`;
        break;*/
      default:
        break;
    }
  }
}
