import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatTable } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatPaginator, PageEvent, } from '@angular/material/paginator';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { BreakpointObserver } from '@angular/cdk/layout';
import { SelectionModel } from '@angular/cdk/collections';
import { Subject, Observable } from 'rxjs';
import { takeUntil, finalize, startWith, map, debounceTime, switchMap } from 'rxjs/operators';
import { ComponentBase } from 'app/shared';
import { AdminService } from 'app/services';
import { IFileInfoExtended, IPaging, IProfile, IFilteredUser, ISearchFile, IFileInfo } from 'app/domain';
import { rowAnimation } from 'app/filemanager/dashboard/helpers';
import { DeleteFileComponent } from './delete-file/delete-file.component';

@Component({
  selector: 'app-files',
  templateUrl: './files.component.html',
  styleUrls: ['./files.component.css'],
  animations: [rowAnimation],
})
export class FilesComponent extends ComponentBase implements OnInit {
  isMobile: boolean;
  filesSearchFormGroup: FormGroup;
  public start = new FormControl();
  public end = new FormControl();
  public user = new FormControl();
  public fileExtension = new FormControl();
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  private pageSize = 25;
  private pageIndex = 0;
  pageSizeOptions: number[] = [this.pageSize, this.pageSize * 2, this.pageSize * 3, this.pageSize * 4];
  page$ = new Subject<PageEvent>();
  selection = new SelectionModel<IFileInfoExtended>(true, []);
  @ViewChild('itemstable', { static: true }) itemstable: MatTable<IFileInfoExtended>;
  private extentionsOpts: string[] = [];
  // filter File extensions
  filteredOptions: Observable<string[]>;
  // Filtered Users
  filteredUsers: Observable<IFilteredUser>;
  constructor(
    private fb: FormBuilder,
    private adminService: AdminService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private breakpointObserver: BreakpointObserver
  ) {
    super();
    breakpointObserver.observe(['(max-width: 600px)']).subscribe(result => {
      this.isMobile = result.matches;
      this.displayedColumns = result.matches
        ? ['select', 'name']
        : ['select', 'name', 'userName', 'downloadCount', 'extension', 'createdAt'];
    });
  }
  displayedColumns: string[] = ['select', 'name', 'userName', 'downloadCount', 'extension', 'createdAt'];
  dataSource = new MatTableDataSource<IFileInfoExtended>();

  ngOnInit(): void {

    this.filesSearchFormGroup = this.fb.group({
      start: this.start,
      end: this.end,
      user: this.user,
      fileExtension: this.fileExtension
    });
    // we pull all file extensions from the server at once (small db table)
    this.filteredOptions = this.fileExtension.valueChanges.pipe(
      startWith(''),
      map(value => this.extFilter(value))
    );
    // get filtered users
    this.filteredUsers = this.user.valueChanges
      .pipe(
        debounceTime(300),
        switchMap(value => this.adminService.searchUser(value))
      );
    const initPage: PageEvent = { pageIndex: this.pageIndex, pageSize: this.pageSize, length: 100 };
    this.page$.next(initPage);
    this.dataSource.paginator = this.paginator;

    // get file extensions list
    this.adminService.getFileExtensions()
      .pipe(takeUntil(this.onDestroy))
      .subscribe(
        extensions => {
          this.extentionsOpts = extensions.map(s => s.name);
        });
    // get files list
    this.adminService.notifyBusy(true);
    this.adminService.getFiles(initPage, this.filesSearchFormGroup.value)
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.adminService.notifyBusy(false)))
      .subscribe(
        paging => {
          this.PopulateTable(paging);
        });
  }
  /** every crud operation on table should call this method */
  private refreshTable() {
    try {
      this.itemstable.renderRows();
    } catch (ex) {
      // console.log(ex);
    }
  }
  displayUserName(user: IProfile) {
    if (user) { return user.userName; }
  }
  private extFilter(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.extentionsOpts.filter(ext => ext.toLowerCase().includes(filterValue));
  }
  private PopulateTable(paging: IPaging<IFileInfoExtended>) {
    this.page$.next({ pageIndex: this.pageIndex, pageSize: this.pageSize, length: paging.totalItems });
    this.dataSource.data = paging.items;
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }
  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  /** The label for the checkbox on the passed row */
  checkboxLabel(row?: IFileInfoExtended): string {
    if (!row) {
      return `${this.isAllSelected() ? 'select' : 'deselect'} all`;
    }
    return `${this.selection.isSelected(row) ? 'deselect' : 'select'}`;
  }
  rangeFilter(d: Date): boolean {
    return d.getTime() < new Date().getTime();
  }
  onTableEvent(pageEvent: PageEvent) {
    this.pageSize = pageEvent.pageSize;
    this.pageIndex = pageEvent.pageIndex;
    this.queryFiles(pageEvent, this.filesSearchFormGroup.value);
  }
private queryFiles(pageEvent: PageEvent, search: ISearchFile) {
  this.selection.clear();
  this.isBusy = true;
  this.adminService.getFiles(pageEvent, search)
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.isBusy = false))
      .subscribe(paging => {
        this.PopulateTable(paging);
      });
}
  onSearchItemsSubmit() {
    const initPage: PageEvent = { pageIndex: this.pageIndex, pageSize: this.pageSize, length: 100 };
    this.queryFiles(initPage, this.filesSearchFormGroup.value);
  }
  onDelete() {
    const dialogRef = this.dialog.open(DeleteFileComponent, {
      width: '500px',
      data: this.selection.selected
    });
    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((deletedFiles: IFileInfo[]) => {
        if (!deletedFiles) {
          return;
        }
        this.selection.clear();
        deletedFiles.forEach(result => {
          const index = this.dataSource.data.findIndex(s => s.id === result.id);
          if (index !== -1) {
            this.dataSource.data.splice(index, 1);
            this.refreshTable();
          }
        });
        this.snackBar.open(`${deletedFiles.length} file(s) has been deleted successfully`, '', { duration: 3000 });
      });
  }
}
