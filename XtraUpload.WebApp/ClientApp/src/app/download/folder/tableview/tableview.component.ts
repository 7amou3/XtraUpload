import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { FilemanagerBase } from 'app/filemanager/dashboard/filemanagerbase';
import { IFileInfo, IFolderInfo, IItemsMenu, IItemInfo } from 'app/domain';
import { FileManagerService } from 'app/services';
import { FileMngContextMenuService } from 'app/services/contextmenu';
import { ActivatedRoute, Router } from '@angular/router';
import { BreakpointObserver } from '@angular/cdk/layout';
import { Subject } from 'rxjs';
import { MatSort, MatTable, MatTableDataSource, Sort } from '@angular/material';
import { takeUntil } from 'rxjs/operators';
import { isFile, rowAnimation } from 'app/filemanager/dashboard/helpers';

@Component({
  selector: 'app-tableview',
  templateUrl: './tableview.component.html',
  styleUrls: ['./tableview.component.css'],
  animations: [rowAnimation],
})
export class TableviewComponent extends FilemanagerBase  implements OnInit {
  mainFolderId: string;
  isMobile: boolean;
  itemsMenu$ = new Subject<IItemsMenu[]>();
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('itemstable', { static: true }) itemstable: MatTable<IItemInfo>;
  constructor(
    filemanagerService: FileManagerService,
    public ctxMenuService: FileMngContextMenuService,
    private route: ActivatedRoute,
    private router: Router,
    breakpointObserver: BreakpointObserver,
    @Inject('API_URL') apiUrl: string
    ) {
    super(filemanagerService, ctxMenuService, apiUrl);
    breakpointObserver.observe(['(max-width: 600px)']).pipe(takeUntil(this.onDestroy)).subscribe(result => {
      this.isMobile =  result.matches;
      this.displayedColumns = result.matches
              ? ['thumbnail', 'name']
              : ['thumbnail', 'name', 'createdAt', 'downloads'];
    });
   }
   displayedColumns: string[] = ['thumbnail', 'name', 'createdAt', 'downloads'];
   dataSource = new MatTableDataSource<IItemInfo>();
  ngOnInit(): void {
    this.route.queryParamMap
    .pipe(takeUntil(this.onDestroy))
    .subscribe(params => {
      this.mainFolderId = params.get('id');
    });
    this.hookEvents();
  }

  /** Occurs when table sort requested */
  matSortChange(sort: Sort) {
    // sort by item name
    if (sort.active === 'name') {
      if (sort.direction === 'asc') {
        this.dataSource.data.sort((a, b) => (a.name > b.name) ? 1 : -1);
      }
      else {
        this.dataSource.data.sort((a, b) => (a.name < b.name) ? 1 : -1);
      }
    }
  }
  /** This wrapper is set because Angular won't bind to external function isFile in template */
  ifFile(item: IItemInfo) {
    return isFile(item);
  }
  /** every crud operation on table should call this method */
  private refreshTable() {
    try {
      this.dataSource.sort = this.sort;
      this.itemstable.renderRows();
    }
    catch (ex) {
      // console.log(ex);
    }
  }
  protected handleNewFile(file: IFileInfo) {
    throw new Error("Method not implemented.");
  }
  protected itemsReceived(itemsInfo: IItemInfo[]): void {
    itemsInfo.forEach(item => {
      this.setitemThumbnail(item);
    });
    this.dataSource.data = itemsInfo;
    this.refreshTable();
  }
  protected handleDrop(dropIndex: number): void {
    throw new Error("Method not implemented.");
  }
  protected handleNewSubFolder(folder: IFolderInfo): void {
    throw new Error("Method not implemented.");
  }
  protected handleDeleteSubFolder(folder: IFolderInfo): void {
    throw new Error("Method not implemented.");
  }
  protected handleDeleteFile(file: IFileInfo): void {
    throw new Error("Method not implemented.");
  }
  protected handleRenameFile(file: IFileInfo): void {
    throw new Error("Method not implemented.");
  }
  protected handleRenameFolder(folder: IFolderInfo): void {
    throw new Error("Method not implemented.");
  }
  protected handleFileAvailability(file: IFileInfo): void {
    throw new Error("Method not implemented.");
  }
  protected handleFolderAvailability(folder: IFolderInfo): void {
    throw new Error("Method not implemented.");
  }
  protected handleMovedItems(itemsIds: string[]): void {
    throw new Error("Method not implemented.");
  }

  onFolderClick(folder: IFolderInfo) {
    this.router.navigate(['/folder'], { queryParams: { id: this.mainFolderId, sub: folder.id } });
  }
}
