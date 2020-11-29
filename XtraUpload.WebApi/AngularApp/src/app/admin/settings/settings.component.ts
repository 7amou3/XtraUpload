import { Component, OnInit, ElementRef, Inject, Input, ViewChild, ChangeDetectorRef } from '@angular/core';
import { FormControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { takeUntil, debounceTime } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, ActivatedRoute } from '@angular/router';
import { DOCUMENT } from '@angular/common';
import { fromEvent } from 'rxjs';
import { IEmailSettings } from 'app/domain';
import { ComponentBase } from 'app/shared';
import { AdminService } from 'app/services';
import { MatSidenav } from '@angular/material/sidenav';
import { MediaMatcher } from '@angular/cdk/layout';

interface Link {
  /* id of the section*/
  fragment: string;
  /* If the anchor is in view of the page */
  active: boolean;

  /* name of the anchor */
  name: string;
  /* top offset px of the anchor */
  top?: number;
}
@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent extends ComponentBase implements OnInit {
  @Input() container: string;
  links: Link[] = [];
  jwtFormGroup: FormGroup;
  hideSecretKey = true;
  secretKey = new FormControl('', [Validators.required, Validators.maxLength(32)]);
  validFor = new FormControl('', [Validators.required, Validators.min(1)]);
  issuer = new FormControl('', [Validators.required]);
  audience = new FormControl('', [Validators.required]);
  emailFormGroup: FormGroup;
  server = new FormControl('', [Validators.required]);
  port = new FormControl('', [Validators.required, Validators.min(1)]);
  username = new FormControl('', [Validators.required]);
  password = new FormControl('', [Validators.required]);
  senderName = new FormControl('', [Validators.required, Validators.minLength(4)]);
  adminEmail = new FormControl('', [Validators.required, Validators.email]);
  supportEmail = new FormControl('', [Validators.required, Validators.email]);
  hidepassword = true;
  hdOptsFormGroup: FormGroup;
  memoryThreshold = new FormControl('', [Validators.required, Validators.min(1)]);
  storageThreshold = new FormControl('', [Validators.required, Validators.min(1)]);
  pageSettingFormGroup: FormGroup;
  title = new FormControl('', [Validators.required, Validators.min(3)]);
  description = new FormControl('', [Validators.required, Validators.min(3)]);
  keywords = new FormControl('', [Validators.required, Validators.min(3)]);
  socialAuthFormGroup: FormGroup;
  googleClientId = new FormControl('', [Validators.required, Validators.min(3)]);
  facebookAppId = new FormControl('', [Validators.required, Validators.min(3)]);
  jwtBusy = false;
  hdBusy = false;
  emailBusy = false;
  uploadBusy = false;
  appSettingBusy = false;
  socialAuthBusy = false;
  private _urlFragment = '';
  private _scrollContainer: any;
  mobileQuery: MediaQueryList;
  private _mobileQueryListener: () => void;
  @ViewChild('snva') sidenav: MatSidenav;
  constructor(
    private fb: FormBuilder,
    private adminService: AdminService,
    private snackBar: MatSnackBar,
    router: Router,
    private _route: ActivatedRoute,
    private _element: ElementRef,
    @Inject(DOCUMENT) private document: Document,
    changeDetectorRef: ChangeDetectorRef,
    media: MediaMatcher,
  ) {
    super();
    this.mobileQuery = media.matchMedia('(min-width: 768px)');
    this._mobileQueryListener = () => changeDetectorRef.detectChanges();
    this.mobileQuery.addEventListener('change', () => this._mobileQueryListener);
    // TODO: move content table management to a component (Single Responsibility Principle)
    const links: Link[] = [
      { fragment: 'pagesettings', name: $localize`Page Settings`, active: true },
      { fragment: 'jwtsettings', name: $localize`Jwt Settings`, active: false },
      { fragment: 'socialauth', name: $localize`Social Auth`, active: false },
      { fragment: 'emailsettings', name: $localize`Email Settings`, active: false },
      { fragment: 'hardwareoptions', name: $localize`Hardware Options`, active: false }
    ];
    this.links.push(...links);
  }

  ngOnInit(): void {
    // On init, the sidenav content element doesn't yet exist, so it's not possible
    // to subscribe to its scroll event until next tick (when it does exist).
    Promise.resolve().then(() => {
      this.links.forEach(link => {
        link.top = this.document.querySelectorAll('#' + link.fragment)[0].getBoundingClientRect().y;
      });
      this._scrollContainer = this.document.querySelectorAll('mat-sidenav-content')[0];
      fromEvent(this._scrollContainer, 'scroll')
        .pipe(
          takeUntil(this.onDestroy),
          debounceTime(20))
        .subscribe(() => this.onScroll());
    });
    this._route.fragment
      .pipe(takeUntil(this.onDestroy))
      .subscribe(fragment => {
        this._urlFragment = fragment;
        const target = document.getElementById(this._urlFragment);
        if (target) {
          target.scrollIntoView();
        }
      });
    this.jwtFormGroup = this.fb.group({
      audience: this.audience,
      issuer: this.issuer,
      validFor: this.validFor,
      secretKey: this.secretKey
    });
    this.emailFormGroup = this.fb.group({
      server: this.server,
      port: this.port,
      username: this.username,
      password: this.password,
      senderName: this.senderName,
      adminEmail: this.adminEmail,
      supportEmail: this.supportEmail
    });
    this.hdOptsFormGroup = this.fb.group({
      memoryThreshold: this.memoryThreshold,
      storageThreshold: this.storageThreshold
    });
    this.pageSettingFormGroup = this.fb.group({
      title: this.title,
      description: this.description,
      keywords: this.keywords
    });
    this.socialAuthFormGroup = this.fb.group({
      facebookAppId: this.facebookAppId,
      googleClientId: this.googleClientId
    });
    this.adminService.notifyBusy(true);
    this.adminService.getSettings()
      .then((data: any) => {
        // page settings
        this.title.setValue(data.appInfo.title);
        this.description.setValue(data.appInfo.description);
        this.keywords.setValue(data.appInfo.keywords);
        // email settings
        this.server.setValue(data.emailSettings.smtp.server);
        this.port.setValue(data.emailSettings.smtp.port);
        this.username.setValue(data.emailSettings.smtp.username);
        this.password.setValue(data.emailSettings.smtp.password);
        this.senderName.setValue(data.emailSettings.sender.name);
        this.adminEmail.setValue(data.emailSettings.sender.admin);
        this.supportEmail.setValue(data.emailSettings.sender.support);
        // hardware settings
        this.memoryThreshold.setValue(data.hardwareCheckOptions.memoryThreshold);
        this.storageThreshold.setValue(data.hardwareCheckOptions.storageThreshold);
        // jwt settings
        this.audience.setValue(data.jwtIssuerOptions.audience);
        this.issuer.setValue(data.jwtIssuerOptions.issuer);
        this.secretKey.setValue(data.jwtIssuerOptions.secretKey);
        this.validFor.setValue(data.jwtIssuerOptions.validFor);
        // Social Auth settings
        this.facebookAppId.setValue(data.socialAuthSettings.facebookAuth.appId);
        this.googleClientId.setValue(data.socialAuthSettings.googleAuth.clientId);
      })
      .finally(() => this.adminService.notifyBusy(false));
  }
  /** don't implement inerface */
  ngOnDestroy(): void {
    this.mobileQuery.removeEventListener('change', this._mobileQueryListener);
    super.ngOnDestroy();
  }
  private onScroll(): void {
    for (let i = 0; i < this.links.length; i++) {
      this.links[i].active = this.isLinkActive(this.links[i], this.links[i + 1]);
    }
  }
  private isLinkActive(currentLink: any, nextLink: any): boolean {
    // A link is considered active if the page is scrolled passed the anchor without also
    // being scrolled passed the next link
    const scrollOffset = this.getScrollOffset();
    return scrollOffset >= currentLink.top && !(nextLink && nextLink.top < scrollOffset);
  }
  /** Gets the scroll offset of the scroll container */
  private getScrollOffset(): number | void {
    const { top } = this._element.nativeElement.getBoundingClientRect();
    if (typeof this._scrollContainer.scrollTop !== 'undefined') {
      return this._scrollContainer.scrollTop + 150;
    } else if (typeof this._scrollContainer.pageYOffset !== 'undefined') {
      return this._scrollContainer.pageYOffset + top;
    }
  }
  async onJwtSubmit(jwtParams) {
    this.jwtBusy = true;
    await this.adminService.updateJwtOpts(jwtParams)
      .then(() => this.showSuccessMsg($localize`Jwt Options`))
      .catch(error => this.handleError(error, this.snackBar))
      .finally(() => this.jwtBusy = false);
  }

  async onEmailSubmit(emailParams: IEmailSettings) {
    this.emailBusy = true;
    await this.adminService.updateEmailOpts(emailParams)
      .then(() => this.showSuccessMsg($localize`Email Options`))
      .catch(error => this.handleError(error, this.snackBar))
      .finally(() => this.emailBusy = false);
  }
  async onHDOptionsSubmit(hardwareteParams) {
    this.hdBusy = true;
    await this.adminService.updateHardwareOpts(hardwareteParams)
      .then(() => this.showSuccessMsg($localize`Hardware Options`))
      .catch(error => this.handleError(error, this.snackBar))
      .finally(() => this.hdBusy = false);
  }
  async onPageSettingsSubmit(pageSettingParams) {
    this.appSettingBusy = true;
    await this.adminService.updateAppInfo(pageSettingParams)
      .then(() => this.showSuccessMsg($localize`Page Settings`))
      .catch(error => this.handleError(error, this.snackBar))
      .finally(() => this.appSettingBusy = false);
  }
  async onSocialAuthSubmit(socialAuthParams) {
    this.socialAuthBusy = true;
    await this.adminService.updateSocialAuthSettings(socialAuthParams)
      .then(() => this.showSuccessMsg($localize`Social Auth`))
      .catch(error => this.handleError(error, this.snackBar))
      .finally(() => this.socialAuthBusy = false);
  }
  showSuccessMsg(section: string) {
    this.snackBar.open($localize`${section} has been updated successfully`, '', { duration: 3000 });
  }
}
