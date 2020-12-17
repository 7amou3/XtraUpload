import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router, } from '@angular/router';
import { OverlayContainer } from '@angular/cdk/overlay';
import { ComponentBase } from 'app/shared/components';
import { UserStorageService } from 'app/services';
import { MediaMatcher } from '@angular/cdk/layout';
import { SidenavService } from 'app/services/sidenav.service';
@Component({
  selector: 'app-full-layout',
  templateUrl: 'container.component.html',
  styleUrls: ['container.component.css']
})

export class ContainerComponent extends ComponentBase implements OnInit {
  mobileQuery: MediaQueryList;
  currentTheme: 'dark' | 'light';
  constructor(
    private overlayContainer: OverlayContainer,
    private userStorageService: UserStorageService,
    private router: Router,
    changeDetectorRef: ChangeDetectorRef,
    media: MediaMatcher,
    private sidenavService: SidenavService
    ) {
    super();
    this.mobileQuery = media.matchMedia('(min-width: 768px)');
    this.mobileQuery.addEventListener("change", () => changeDetectorRef.detectChanges());
  }

  ngOnInit() {
    // get default theme from storage
    let theme = this.userStorageService.profile?.theme;
    // default theme is light
    if (!theme) {
      theme = 'light';
    }
    this.currentTheme = theme;
    this.updateBackDrop(theme);
    if (this.router.routerState.snapshot.url === '/') {
      this.router.navigate(['/login']);
    }
  }
  onOpenCloseNav() {
    this.sidenavService.notifyMenuBtnClick();
  }
  onSelectedTheme(theme: 'dark' | 'light') {
    this.currentTheme = theme;
    // update backdrop
    this.updateBackDrop(theme);
    // update storage
    this.userStorageService.updateTheme(theme);
  }
  private updateBackDrop(theme: 'dark' | 'light') {
    (theme === 'dark') ? this.overlayContainer.getContainerElement().classList.add('dark-theme')
      : this.overlayContainer.getContainerElement().classList.remove('dark-theme');
  }
}

