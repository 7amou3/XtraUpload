import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { MediaMatcher } from '@angular/cdk/layout';
import { ActivatedRoute } from '@angular/router';
import { MatBottomSheet } from '@angular/material/bottom-sheet';
import { MatSidenav } from '@angular/material/sidenav';
import { MatSnackBar } from '@angular/material/snack-bar';
import { takeUntil } from 'rxjs/operators';
import { Subject, ReplaySubject, merge } from 'rxjs';
import { ComponentBase } from 'app/shared';
import { IItemInfo, IFolderInfo, itemAction, IUploadSettings} from 'app/domain';
import { FileManagerService, UserStorageService, SeoService } from 'app/services';
import { FileMngContextMenuService, SnavContextMenuService } from 'app/services/contextmenu';
import { UploadBottomSheetComponent } from './upload-bottom-sheet/upload-bottom-sheet.component';
import { SidenavService } from 'app/services/sidenav.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent extends ComponentBase implements OnInit {
  private readonly pageTitle = $localize`My Files & Folders`;
  mobileQuery: MediaQueryList;
  private _mobileQueryListener: () => void;
  @ViewChild('sidenav') sidenav: MatSidenav;
  @ViewChild('snav') mainNav: MatSidenav;
  /** Content of the current folder */
  folderContent$ = new ReplaySubject<IItemInfo[]>();
  /** Selected item */
  itemInfo$ = new Subject<IItemInfo>();
  uploadSetting$ = new Subject<IUploadSettings>();
  displayMode: 'list' | 'grid';
  /** flat array pointing to the path of the selected folder (used for breadcrumb) */
  pathSelectedFolder: IFolderInfo[] = [];
  constructor(private route: ActivatedRoute,
    private seoService: SeoService,
    private filemanagerService: FileManagerService,
    private mainCtxMenuService: FileMngContextMenuService,
    private snavCtxMenuService: SnavContextMenuService,
    private userstorageService: UserStorageService,
    private sidenaveService: SidenavService,
    private snackBar: MatSnackBar,
    private bottomSheet: MatBottomSheet,
    changeDetectorRef: ChangeDetectorRef,
    media: MediaMatcher) {
    super();
    seoService.setPageTitle(this.pageTitle);
    this.mobileQuery = media.matchMedia('(min-width: 768px)');
    this._mobileQueryListener = () => changeDetectorRef.detectChanges();
    this.mobileQuery.addEventListener('change', () => this._mobileQueryListener);
  }

  async ngOnInit() {
    this.initView();
    this.route.queryParamMap
    .pipe(takeUntil(this.onDestroy))
    .subscribe(async params => {
      await this.getFolderContent(params.get('folder'));
    });
    this.filemanagerService.serviceBusy$
    .pipe(takeUntil(this.onDestroy))
    .subscribe(isbusy => {
      this.isBusy = isbusy;
    });
    // Display sidenav menu on request
    merge(
      this.mainCtxMenuService.itemInfoRequested$,
      this.snavCtxMenuService.itemInfoRequested$
    )
    .pipe(takeUntil(this.onDestroy))
    .subscribe(item => {
      this.itemInfo$.next(item);
      this.sidenav.open();
    });
    // Get the upload setting
    this.filemanagerService.getUploadSetting()
    .then(uploadSetting => {
      this.uploadSetting$.next(uploadSetting);
    });
    // Subscribe to header menu click event
    this.sidenaveService.subscribeMenuBtnClick()
    .pipe(takeUntil(this.onDestroy))
    .subscribe(() => {
      this.mainNav.toggle();
    });
  }
  ngOnDestroy(): void {
    this.mobileQuery.removeEventListener('change', this._mobileQueryListener);
    super.ngOnDestroy();
  }

  private initView(): void {
    const storage = this.userstorageService.profile;
    if (!storage.itemsDisplay) {
      this.changeDisplay('list');
    }
    else {
      this.displayMode = storage.itemsDisplay;
    }
  }
  async getFolderContent(folderId?: string): Promise<void> {
    this.isBusy = true;
    await this.filemanagerService.getFolderContent(folderId)
    .then( data => this.folderContent$.next(data))
    .catch(error => this.handleError(error, this.snackBar))
    .finally(() => this.isBusy = false);
  }

  changeDisplay(display: 'list' | 'grid') {
    const profile = this.userstorageService.profile;
    profile.itemsDisplay = display;
    this.displayMode = display;
    // update the local storage with the new data
    this.userstorageService.profile = profile;
  }

  openUploadSheet() {
    this.bottomSheet.open(UploadBottomSheetComponent, {disableClose: true, panelClass: 'custom-bottom-sheet'});
  }
  openCreateFolder() {
    const currentFolderId = this.route.snapshot.queryParamMap.get('folder');
    let folder: IFolderInfo;
    // root folder
    if (!currentFolderId) {
      folder = this.pathSelectedFolder[0];
    }
    else {
      folder = this.pathSelectedFolder.find(s => s.id === currentFolderId);
    }
    // request to create subfolder
    if (folder) {
      this.mainCtxMenuService.handleMenuItemClick([folder], itemAction.create);
    }
  }
  onfolderPathChanged(childfolderPath: IFolderInfo[]) {
    this.pathSelectedFolder = childfolderPath;
  }
  onBreadCrumbClick(folder: IFolderInfo) {
    this.mainCtxMenuService.handleMenuItemClick([folder], itemAction.openFolder);
  }
}
