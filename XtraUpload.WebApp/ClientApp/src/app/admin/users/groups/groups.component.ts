import { Component, OnInit, ViewChild } from '@angular/core';
import { IUserRoleClaims, IRoleClaim } from 'app/domain';
import { MatTableDataSource, MatTable, MatSnackBar, MatDialog } from '@angular/material';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared';
import { takeUntil, finalize } from 'rxjs/operators';
import { AddgroupComponent } from './dialogs/addgroup/addgroup.component';
import { DeletegroupComponent } from './dialogs/deletegroup/deletegroup.component';
import { EditgroupComponent } from './dialogs/editgroup/editgroup.component';
import { rowAnimation } from 'app/filemanager/dashboard/helpers';
import { BreakpointObserver } from '@angular/cdk/layout';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.css'],
  animations: [rowAnimation],
})
export class GroupsComponent extends ComponentBase implements OnInit {
  isMobile: boolean;
  selectedGroup: IUserRoleClaims;
  dataSource = new MatTableDataSource<IUserRoleClaims>();
  @ViewChild('itemstable', { static: true }) itemstable: MatTable<IUserRoleClaims>;
  constructor(
    private adminService: AdminService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private breakpointObserver: BreakpointObserver) {
    super();
    breakpointObserver.observe(['(max-width: 600px)']).subscribe(result => {
      this.isMobile = result.matches;
      this.displayedColumns = result.matches
        ? ['name', 'AdminAreaAccess', 'FileManagerAccess', 'actions']
        : ['name', 'AdminAreaAccess', 'FileManagerAccess', 'DownloadSpeed', 'StorageSpace', 'MaxFileSize', 'FileExpiration', 'ConcurrentUpload', 'WaitTime', 'DownloadTTW', 'actions'];
    });
  }
  displayedColumns: string[] = ['name', 'AdminAreaAccess', 'FileManagerAccess', 'DownloadSpeed', 'StorageSpace', 'MaxFileSize', 'FileExpiration', 'ConcurrentUpload', 'WaitTime', 'DownloadTTW', 'actions']; 

  ngOnInit(): void {
    this.adminService.notifyBusy(true);
    this.adminService.getUsersGroups()
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.adminService.notifyBusy(false)))
      .subscribe(
        (userRole) => {
          this.dataSource.data = userRole.sort((a, b) => (a.role.id - b.role.id));
        }
      );
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
  onItemClick(item: IUserRoleClaims) {
    this.selectedGroup = item;
  }
  onAddGroup() {
    const dialogRef = this.dialog.open(AddgroupComponent, {
      width: '500px',
      data: this.dataSource.data
    });
    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((result: IUserRoleClaims) => {
        if (!result) {
          return;
        }
        this.dataSource.data.push(result);
        this.refreshTable();
        this.snackBar.open(`The user group ${result.role.name} has been added successfully`, '', { duration: 3000 });
      });
  }
  onEdit() {
    const dialogRef = this.dialog.open(EditgroupComponent, {
      width: '550px',
      data: {selectedGroup: this.selectedGroup, fullGroupList: this.dataSource.data}
    });
    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((result: IUserRoleClaims) => {
        if (!result) {
          return;
        }
        const roleClaims = this.dataSource.data.find(s => s.role.id === result.role.id);
        if (roleClaims) {
          roleClaims.role.name = result.role.name;
          roleClaims.claims = result.claims;
          this.refreshTable();
          this.snackBar.open(`The user group ${roleClaims.role.name} has been updated successfully`, '', { duration: 3000 });
        }
      });
  }
  onDelete() {
    const dialogRef = this.dialog.open(DeletegroupComponent, {
      width: '500px',
      data: {selectedGroup: this.selectedGroup, fullGroupList: this.dataSource.data}
    });
    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((result: IUserRoleClaims) => {
        if (!result) {
          return;
        }
        const index = this.dataSource.data.findIndex(s => s.role.id === result.role.id);
        if (index !== -1) {
          this.dataSource.data.splice(index, 1);
          this.refreshTable();
          this.snackBar.open(`The user group ${result.role.name} has been deleted successfully`, '', { duration: 3000 });
        }
      });
  }
  getClaim(claims: IRoleClaim[], claim: string) {
    return claims.find(s => s.claimType === claim)?.claimValue;
  }

}
