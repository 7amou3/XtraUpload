import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { ComponentBase } from 'app/shared';
import { ActivatedRoute, Router } from '@angular/router';
import { FileManagerService, SidenavService } from 'app/services';
import { takeUntil } from 'rxjs/operators';
import { IItemInfo } from 'app/domain';
import { MatSidenav } from '@angular/material/sidenav';
import { MediaMatcher } from '@angular/cdk/layout';
import { ReplaySubject } from 'rxjs';

@Component({
  selector: 'app-folder',
  templateUrl: './folder.component.html',
  styleUrls: ['./folder.component.css']
})
export class FolderComponent extends ComponentBase implements OnInit {
  folderId: string;
  subfolderId: string;
  mobileQuery: MediaQueryList;
  private _mobileQueryListener: () => void;
  @ViewChild('snav') sidenav: MatSidenav;
  /** Content of the current folder */
  folderContent$ = new ReplaySubject<IItemInfo[]>();
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fileMngService: FileManagerService,
    private sidenavService: SidenavService,
    changeDetectorRef: ChangeDetectorRef,
    media: MediaMatcher,
  ) {
    super();
    this.mobileQuery = media.matchMedia('(min-width: 768px)');
    this._mobileQueryListener = () => changeDetectorRef.detectChanges();
    this.mobileQuery.addListener(this._mobileQueryListener);
   }

  ngOnInit(): void {
    this.isBusy = true;
    this.route.queryParamMap
    .pipe(takeUntil(this.onDestroy))
    .subscribe(
      async params => {
        this.folderId = params.get('id');
        this.subfolderId = params.get('sub');
        if (!this.folderId) {
          this.router.navigate(['/404']);
        }
        else {
          await this.fileMngService.getPublicFolderContent(this.folderId, this.subfolderId)
          .then((items) => {
              this.folderContent$.next(items);
          })
          .catch((err) => {
            if (err.error?.errorContent?.message) {
              this.message$.next({errorMessage: err.error.errorContent.message});
            }
            else throw err;
          })
          .finally(() => this.isBusy = false);
        }
      }
    );
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

}
