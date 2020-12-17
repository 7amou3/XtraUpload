import { Component, ViewChild, OnInit, Inject } from '@angular/core';
import { MatTableDataSource, MatTable } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort, Sort } from '@angular/material/sort';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { IItemInfo, IFolderInfo, IItemsMenu, IFileInfo } from 'app/models';
import { FileManagerService, UploadService } from 'app/services';
import { FileMngContextMenuService } from 'app/services/contextmenu';
import { FilemanagerBase } from '../filemanagerbase';
import { isFile, rowAnimation } from '../helpers';
import { BreakpointObserver } from '@angular/cdk/layout';

@Component({
  selector: 'app-tableview',
  templateUrl: './tableview.component.html',
  styleUrls: ['./tableview.component.scss'],
  animations: [rowAnimation],
})
export class TableviewComponent extends FilemanagerBase implements OnInit {
  isMobile: boolean;
  itemsMenu$ = new Subject<IItemsMenu[]>();
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('itemstable', { static: true }) itemstable: MatTable<IItemInfo>;
  constructor(
    filemanagerService: FileManagerService,
    uploadService: UploadService,
    public ctxMenuService: FileMngContextMenuService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar,
    breakpointObserver: BreakpointObserver,
    @Inject('API_URL') apiUrl: string) {
    super(filemanagerService, uploadService, ctxMenuService, apiUrl);
    breakpointObserver.observe(['(max-width: 600px)']).pipe(takeUntil(this.onDestroy)).subscribe(result => {
      this.isMobile =  result.matches;
      this.displayedColumns = result.matches
              ? ['thumbnail', 'name', 'actions']
              : ['thumbnail', 'name', 'createdAt', 'downloads', 'actions'];
    });
  }
  displayedColumns: string[] = ['thumbnail', 'name', 'createdAt', 'downloads', 'actions'];
  dataSource = new MatTableDataSource<IItemInfo>();
  ngOnInit() {
    this.route.queryParamMap
    .pipe(takeUntil(this.onDestroy))
    .subscribe( () => {
      this.selections = [];
      this.itemAdded = false;
    });
    this.hookEvents();
  }

  protected itemsReceived(itemsInfo: IItemInfo[]) {
    itemsInfo.forEach(item => {
      this.setitemThumbnail(item);
    });
    this.dataSource.data = itemsInfo;
    this.refreshTable();
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

  /** Handles the drop event */
  protected async handleDrop(dropIndex: number): Promise<void> {
    // get the drop folder
    const folder = this.dataSource.data[dropIndex];
    if (this.isdroppableArea(folder) === false) {
      return;
    }
    this.filemanagerService.notifyBusy(true);
    await this.filemanagerService.requestMoveItems(this.selections, folder.id)
    .finally(() => this.filemanagerService.notifyBusy(false));
  }

  /** Invoked when item(s) has moved successfully */
  protected handleMovedItems(movedIds: string[]): void {
      // items moved successfully, remove them..
      movedIds.forEach((id) => {
        const index = this.dataSource.data.findIndex(s => s.id === id);
        if (index !== -1) {
          this.dataSource.data.splice(index, 1);
        }
      });
      this.snackBar.open($localize`File(s)/Folder(s) has been moved successfully`, '', { duration: 3000 });
      this.selections = [];
      this.refreshTable();
  }
  /**Invoked when a new file has been uploaded successfully*/
  protected handleNewFile(file: IFileInfo) {
    if (!file) {
      return;
    }

    this.setitemThumbnail(file);
    this.dataSource.data.push(file);
    this.refreshTable();
  }
  /** invoked when a new sub folder has been added */
  protected handleNewSubFolder(folder: IFolderInfo): void {
    if (!folder) {
      this.snackBar.open($localize`Server Error, please try again.`, '', { duration: 3000 });
      return;
    }

    const folderid = this.route.snapshot.queryParamMap.get('folder') ?? 'root';
    // Add the folder to the table if the route matches the id of it's parent
    if (folder.parentid === folderid) {
      this.dataSource.data.push(folder);
      this.refreshTable();
    }
    this.snackBar.open($localize`The new subfolder ${folder.name} has been added successfully`, '', { duration: 3000 });
  }

  /** invoked when a file has been renamed */
  protected handleRenameFile(file: IFileInfo): void {
    if (!file) {
      return;
    }
    const rfile = this.dataSource.data.find(s => s.id === file.id);
    if (rfile) {
      rfile.name = file.name;
      rfile.lastModified = file.lastModified;
      this.refreshTable();
      this.snackBar.open($localize`The file ${rfile.name} has been renamed successfully`, '', { duration: 3000 });
    }
  }

  /** invoked when a file online availability changed */
  protected handleFileAvailability(file: IFileInfo): void {
    if (!file) {
      return;
    }

    const rfile = this.dataSource.data.find(s => s.id === file.id);
    if (rfile) {
      rfile.status = file.status;
      rfile.lastModified = file.lastModified;
      this.refreshTable();
      this.snackBar.open($localize`The online availability of ${rfile.name} has been changed successfully`, '', { duration: 3000 });
    }
  }

  /** invoked when a folder online availability changed */
  protected handleFolderAvailability(folder: IFolderInfo): void {
    if (!folder) {
      return;
    }

    const rfolder = this.dataSource.data.find(s => s.id === folder.id);
    if (rfolder) {
      rfolder.status = folder.status;
      rfolder.lastModified = folder.lastModified;
      this.refreshTable();
      this.snackBar.open($localize`The online availability of ${rfolder.name} has been changed successfully`, '', { duration: 3000 });
    }
  }
  /** invoked when a folder has been renamed */
  protected handleRenameFolder(folder: IFolderInfo): void {
    if (!folder) {
      return;
    }
    const rfile = this.dataSource.data.find(s => s.id === folder.id);
    if (rfile) {
      rfile.name = folder.name;
      rfile.lastModified = folder.lastModified;
      this.refreshTable();
      this.snackBar.open($localize`The folder ${rfile.name} has been renamed successfully`, '', { duration: 3000 });
    }
  }
  /** invoked when a file has been deleted */
  protected handleDeleteFile(file: IFileInfo): void {
    if (!file) {
      return;
    }
    // Remove the file if it's currently displayed in table
    const index = this.dataSource.data.findIndex(s => s.id === file.id);
    if (index !== -1) {
      this.dataSource.data.splice(index, 1);
      this.refreshTable();
      this.snackBar.open($localize`File(s) has been deleted successfully`, '', { duration: 3000 });
      // Remove from selection
      const sId = this.selections.findIndex(s => s.id === file.id);
      if (index !== -1) {
        this.selections.splice(sId, 1);
      }
    }
  }
  /** invoked when a new sub folder has been added */
  protected handleDeleteSubFolder(folder: IFolderInfo): void {
    if (!folder) {
      return;
    }

    // Remove the folder if it's currently displayed in table
    const index = this.dataSource.data.findIndex(s => s.id === folder.id);
    if (index !== -1) {
      this.dataSource.data.splice(index, 1);
      this.refreshTable();
      this.snackBar.open($localize`Folder(s) has been deleted successfully`, '', { duration: 3000 });
    }
    // Remove if it's content are displayed
    const folderid = this.route.snapshot.queryParamMap.get('folder') ?? 'root';
    if (folderid === folder.id) {
      this.router.navigate(['/filemanager'], { queryParams: { folder: folder.parentid } });
    }
    // Remove from selection
    const sId = this.selections.findIndex(s => s.id === folder.id);
    if (index !== -1) {
      this.selections.splice(sId, 1);
    }
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
}
