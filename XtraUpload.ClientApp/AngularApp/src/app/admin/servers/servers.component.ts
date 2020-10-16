import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { IStorageServer } from 'app/domain';
import { rowAnimation } from 'app/filemanager/dashboard/helpers';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared';
import { finalize, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-servers',
  templateUrl: './servers.component.html',
  styleUrls: ['./servers.component.css'],
  animations: [rowAnimation]
})
export class ServersComponent extends ComponentBase implements OnInit {
  private selectedServer: IStorageServer;
  displayedColumns: string[] = ['address', 'state', 'actions'];
  dataSource = new MatTableDataSource<IStorageServer>();
  @ViewChild('itemstable', { static: true }) itemstable: MatTable<IStorageServer>;
  constructor(private adminService: AdminService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog) {
    super();
  }

  ngOnInit(): void {
    this.adminService.notifyBusy(true);
    this.adminService.getStorageServers()
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.adminService.notifyBusy(false)))
      .subscribe(
        (servers) => {
          console.log(servers)
          this.dataSource.data = servers;
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
  onItemClick(item: IStorageServer) {
    this.selectedServer = item;
  }
  onAdd() {}
  onEdit() {}
  onDelete() {}
}
