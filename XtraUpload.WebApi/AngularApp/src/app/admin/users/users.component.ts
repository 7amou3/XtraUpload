import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatTable } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { BreakpointObserver } from '@angular/cdk/layout';
import { SelectionModel } from '@angular/cdk/collections';
import { Subject, Observable } from 'rxjs';
import { takeUntil, debounceTime, switchMap } from 'rxjs/operators';
import { ComponentBase } from 'app/shared';
import { AdminService } from 'app/services';
import { IPaging, IProfile, IFilteredUser, ISearchFile, IUserRoleClaims, IEditProfile, IProfileClaim } from 'app/domain';
import { rowAnimation } from 'app/filemanager/dashboard/helpers';
import { DeleteuserComponent } from './dialogs/deleteuser/deleteuser.component';
import { EdituserComponent } from './dialogs/edituser/edituser.component';

@Component({
  selector: 'app-userlist',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css'],
  animations: [rowAnimation],
})
export class UserListComponent extends ComponentBase implements OnInit {

  isMobile: boolean;
  usersSearchFormGroup: FormGroup;
  public start = new FormControl();
  public end = new FormControl();
  public user = new FormControl();
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  private pageSize = 25;
  private pageIndex = 0;
  pageSizeOptions: number[] = [this.pageSize, this.pageSize * 2, this.pageSize * 3, this.pageSize * 4];
  page$ = new Subject<PageEvent>();
  selection = new SelectionModel<IProfile>(true, []);
  @ViewChild('itemstable', { static: true }) itemstable: MatTable<IProfile>;
  // filter File extensions
  filteredOptions: Observable<string[]>;
  // Filtered Users
  filteredUsers: Observable<IFilteredUser>;
  groups: IUserRoleClaims[];
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
        ? ['select', 'avatar', 'email', 'actions']
        : ['select', 'avatar', 'userName', 'role', 'email', 'emailConfirmed', 'accountSuspended', 'createdAt', 'actions'];
    });
  }
  displayedColumns: string[] = ['select', 'userName', 'role', 'email', 'emailConfirmed', 'createdAt'];
  dataSource = new MatTableDataSource<IProfileClaim>();

  ngOnInit(): void {
    this.usersSearchFormGroup = this.fb.group({
      start: this.start,
      end: this.end,
      user: this.user
    });
    // get filtered users
    this.filteredUsers = this.user.valueChanges
      .pipe(
        debounceTime(300),
        switchMap(async value => await this.adminService.searchUser(value))
      );
    const initPage: PageEvent = { pageIndex: this.pageIndex, pageSize: this.pageSize, length: 100 };
    this.page$.next(initPage);
    this.dataSource.paginator = this.paginator;

    // get users list
    this.adminService.notifyBusy(true);
    this.adminService.getUsers(initPage, this.usersSearchFormGroup.value)
      .then(paging => this.PopulateTable(paging))
      .finally(() => this.adminService.notifyBusy(false));
    // Get groups
    this.adminService.getUsersGroups()
      .then(groups => this.groups = groups);
  }
  /** every crud operation on table should call this method */
  private refreshTable() {
    try {
      this.itemstable.renderRows();
    }
    catch (ex) {
      // console.log(ex);
    }
  }
  displayUserName(user: IProfile) {
    if (user) { return user.userName; }
  }
  private PopulateTable(paging: IPaging<IProfileClaim>) {
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
  checkboxLabel(row?: IProfile): string {
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
    this.queryUsers(pageEvent, this.usersSearchFormGroup.value);
  }
  private async queryUsers(pageEvent: PageEvent, search: ISearchFile) {
    this.selection.clear();
    this.isBusy = true;
    await this.adminService.getUsers(pageEvent, search)
      .then(paging => this.PopulateTable(paging))
      .finally(() => this.isBusy = false);
  }
  onSearchItemsSubmit() {
    const initPage: PageEvent = { pageIndex: this.pageIndex, pageSize: this.pageSize, length: 100 };
    this.queryUsers(initPage, this.usersSearchFormGroup.value);
  }
  onUserEdit(user: IProfile) {
    const dialogRef = this.dialog.open(EdituserComponent, {
      width: '500px',
      data: { user: user, groups: this.groups }
    });

    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((editedProfile) => {
        if (!editedProfile) {
          return;
        }
        const row = this.dataSource.data.find(s => s.id === editedProfile.id);
        if (row) {
          row.roleName = editedProfile.selectedGroup.role.name;
          row.userName = editedProfile.userName;
          row.accountSuspended = editedProfile.suspendAccount;
          row.emailConfirmed = editedProfile.emailConfirmed;
          row.email = editedProfile.email;
          this.refreshTable();
          this.snackBar.open(`The user ${editedProfile.userName} has been updated successfully`, '', { duration: 3000 });
        }
      });
  }
  onUserDelete(items: IProfile[]) {
    const dialogRef = this.dialog.open(DeleteuserComponent, {
      width: '500px',
      data: items
    });
    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((profiles: IProfile[]) => {
        if (!profiles) {
          return;
        }
        this.selection.clear();
        profiles.forEach(p => {
          const index = this.dataSource.data.findIndex(s => s.id === p.id);
          if (index !== -1) {
            this.dataSource.data.splice(index, 1);
          }
        });
        this.refreshTable();
        this.snackBar.open(`${profiles.length} User(s) has been deleted successfully`, '', { duration: 3000 });
      });
  }
}
