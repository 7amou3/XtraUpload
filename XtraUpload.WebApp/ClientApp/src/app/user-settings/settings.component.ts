import { Component, OnInit, ViewChild, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ComponentBase } from 'app/shared';
import { MatSidenav } from '@angular/material';
import { Title } from '@angular/platform-browser';
import { MediaMatcher } from '@angular/cdk/layout';
import { SidenavService } from 'app/services';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent extends ComponentBase implements OnInit {
  mobileQuery: MediaQueryList;
  private _mobileQueryListener: () => void;
  private readonly pageTitle = 'Settings';
  @ViewChild('sidenav') sidenav: MatSidenav;
  subTitle = '';
  constructor(
    private sidenavService: SidenavService,
    changeDetectorRef: ChangeDetectorRef,
    media: MediaMatcher,
    private titleService: Title
  ) {
    super();
    titleService.setTitle(this.pageTitle);
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
  }
  ngOnDestroy(): void {
    this.mobileQuery.removeListener(this._mobileQueryListener);
    super.ngOnDestroy();
  }
  onActivate(ev) {
    switch (ev.constructor.name) {
      case 'OverviewComponent':
        this.subTitle = 'Overview';
        break;
      case 'AvatarComponent':
        this.subTitle = 'Avatar';
        break;
      case 'ChangePasswordComponent':
        this.subTitle = 'Password';
        break;
      case 'UserinfoComponent':
        this.subTitle = 'User Info';
        break;
      default:
        break;
    }
  }
}
