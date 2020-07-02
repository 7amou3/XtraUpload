import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatTable } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { takeUntil, finalize } from 'rxjs/operators';
import { rowAnimation } from 'app/filemanager/dashboard/helpers';
import { IFileExtension, IEditExtension } from 'app/domain';
import { ComponentBase } from 'app/shared';
import { AdminService } from 'app/services';
import { EditComponent } from './dialogs/edit/edit.component';
import { DeleteComponent } from './dialogs/delete/delete.component';
import { AddComponent } from './dialogs/add/add.component';

@Component({
  selector: 'app-extensions',
  templateUrl: './extensions.component.html',
  styleUrls: ['./extensions.component.css'],
  animations: [rowAnimation],
})
export class ExtensionsComponent extends ComponentBase implements OnInit {
  selectedExt: IFileExtension;
  displayedColumns: string[] = ['id', 'name', 'actions'];
  dataSource = new MatTableDataSource<IFileExtension>();
  @ViewChild('itemstable', { static: true }) itemstable: MatTable<IFileExtension>;
  constructor(
    private adminService: AdminService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog) {
    super();
  }

  ngOnInit(): void {
    this.adminService.notifyBusy(true);
    this.adminService.getFileExtensions()
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.adminService.notifyBusy(false)))
      .subscribe(
        (ext) => {
          this.dataSource.data = ext.sort((a, b) => (a.id - b.id));
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
  onItemClick(item: IFileExtension) {
    this.selectedExt = item;
  }
  onAddExtension() {
    const dialogRef = this.dialog.open(AddComponent, {
      width: '500px',
      data: this.dataSource.data
    });
    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((result: IFileExtension) => {
        if (!result) {
          return;
        }
        this.dataSource.data.push(result);
        this.refreshTable();
        this.snackBar.open(`The extension ${result.name} has been added successfully`, '', { duration: 3000 });
      });
  }
  onEdit() {
    const dialogRef = this.dialog.open(EditComponent, {
      width: '500px',
      data: {selectedExt: this.selectedExt, fullExtList: this.dataSource.data}
    });
    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((result: IEditExtension) => {
        if (!result) {
          return;
        }
        const ext = this.dataSource.data.find(s => s.id === result.id);
        if (ext) {
          ext.name = result.newExt;
          this.refreshTable();
          this.snackBar.open(`The extension ${ext.name} has been renamed successfully`, '', { duration: 3000 });
        }
      });
  }
  onDelete() {
    const dialogRef = this.dialog.open(DeleteComponent, {
      width: '500px',
      data: this.selectedExt
    });
    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((result: IFileExtension) => {
        if (!result) {
          return;
        }
        const index = this.dataSource.data.findIndex(s => s.id === result.id);
        if (index !== -1) {
          this.dataSource.data.splice(index, 1);
          this.refreshTable();
          this.snackBar.open(`The extension ${result.name} has been deleted successfully`, '', { duration: 3000 });
        }
      });
  }
}
