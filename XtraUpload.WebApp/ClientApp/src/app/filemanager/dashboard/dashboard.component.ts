import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { MediaMatcher } from '@angular/cdk/layout';
import { ActivatedRoute } from '@angular/router';
import { MatBottomSheet } from '@angular/material/bottom-sheet';
import { MatSidenav } from '@angular/material/sidenav';
import { takeUntil, finalize } from 'rxjs/operators';
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
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent extends ComponentBase implements OnInit {
  private readonly pageTitle = 'My Files & Folders';
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
    private bottomSheet: MatBottomSheet,
    changeDetectorRef: ChangeDetectorRef,
    media: MediaMatcher) {
    super();
    seoService.setPageTitle(this.pageTitle);
    this.mobileQuery = media.matchMedia('(min-width: 768px)');
    this._mobileQueryListener = () => changeDetectorRef.detectChanges();
    this.mobileQuery.addListener(this._mobileQueryListener);
  }

  ngOnInit() {
    this.initView();
    this.route.queryParamMap
    .pipe(takeUntil(this.onDestroy))
    .subscribe(params => {
      this.getFolderContent(params.get('folder'));
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
    .pipe(takeUntil(this.onDestroy))
    .subscribe(uploadSetting => {
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
    this.mobileQuery.removeListener(this._mobileQueryListener);
    super.ngOnDestroy();
  }

  private initView(): void {
    const storage = this.userstorageService.getProfile();
    if (!storage.itemsDisplay) {
      this.changeDisplay('list');
    }
    else {
      this.displayMode = storage.itemsDisplay;
    }
  }
  getFolderContent(folderId?: string): void {
    this.isBusy = true;
    this.filemanagerService.getFolderContent(folderId)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
    .subscribe(
      data => {
        this.folderContent$.next(data);
      }
    );
  }

  changeDisplay(display: 'list' | 'grid') {
    const userStorage = this.userstorageService.getProfile();
    userStorage.itemsDisplay = display;
    // update the local storage with the new data
    this.displayMode = this.userstorageService.saveUser(userStorage).itemsDisplay;

  }

  openUploadSheet() {
    this.bottomSheet.open(UploadBottomSheetComponent);
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
