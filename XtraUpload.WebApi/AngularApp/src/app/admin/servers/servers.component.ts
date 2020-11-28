import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { IStorageServer } from 'app/domain';
import { rowAnimation } from 'app/filemanager/dashboard/helpers';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared';
import { takeUntil } from 'rxjs/operators';
import { AddserverComponent } from './dialogs/addserver/addserver.component';
import { DeleteserverComponent } from './dialogs/deleteserver/deleteserver.component';
import { EditserverComponent } from './dialogs/editserver/editserver.component';

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
      .then(servers => this.dataSource.data = servers)
      .finally(() => this.adminService.notifyBusy(false));
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
  onAdd() {
    const dialogRef = this.dialog.open(AddserverComponent, {
      width: '560px',
      data: this.dataSource.data
    });
    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((result: IStorageServer) => {
        if (!result) {
          return;
        }
        this.dataSource.data.push(result);
        this.refreshTable();
        this.snackBar.open($localize`The server ${result.address} has been added successfully`, '', { duration: 3000 });
      });
  }
  onEdit() {
    const dialogRef = this.dialog.open(EditserverComponent, {
      width: '560px',
      data: { selectedServer: this.selectedServer, serversList: this.dataSource.data }
    });
    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((result: IStorageServer) => {
        if (!result) {
          return;
        }
        const server = this.dataSource.data.find(s => s.id === result.id);
        if (server) {
          server.address = result.address;
          server.state = result.state;
          this.refreshTable();
          this.snackBar.open($localize`The server ${result.address} has been updated successfully`, '', { duration: 3000 });
        }
      });
  }
  onDelete() {
    const dialogRef = this.dialog.open(DeleteserverComponent, {
      width: '500px',
      data: this.selectedServer
    });
    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((server: IStorageServer) => {
        if (!server) {
          return;
        }
        const index = this.dataSource.data.findIndex(s => s.id === server.id);
        if (index !== -1) {
          this.dataSource.data.splice(index, 1);
          this.refreshTable();
          this.snackBar.open($localize`The server ${server.address} has been deleted successfully`, '', { duration: 3000 });
        }
      });
  }
}
