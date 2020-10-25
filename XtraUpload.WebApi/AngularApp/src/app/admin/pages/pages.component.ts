import { Component, OnInit, ViewChild } from '@angular/core';
import { ComponentBase } from 'app/shared';
import { AdminService } from 'app/services';
import { MatTableDataSource, MatTable } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { takeUntil, finalize } from 'rxjs/operators';
import { IPage } from 'app/domain';
import { rowAnimation } from 'app/filemanager/dashboard/helpers';
import { EditpageComponent } from './dialogs/editpage/editpage.component';
import { AddpageComponent } from './dialogs/addpage/addpage.component';
import { DeletepageComponent } from './dialogs/deletepage/deletepage.component';

@Component({
  selector: 'app-pages',
  templateUrl: './pages.component.html',
  styleUrls: ['./pages.component.css'],
  animations: [rowAnimation]
})
export class PagesComponent extends ComponentBase implements OnInit {
  private selectedPage: IPage;
  displayedColumns: string[] = ['name', 'footerVisible', 'createdAt', 'updatedAt', 'actions'];
  dataSource = new MatTableDataSource<IPage>();
  @ViewChild('itemstable', { static: true }) itemstable: MatTable<IPage>;
  constructor( private adminService: AdminService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog) {
    super();
  }

  ngOnInit(): void {
    this.adminService.notifyBusy(true);
    this.adminService.getPages()
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.adminService.notifyBusy(false)))
      .subscribe(
        (pages) => {
          this.dataSource.data = pages;
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
  onItemClick(item: IPage) {
    this.selectedPage = item;
  }

  onAdd() {
    const dialogRef = this.dialog.open(AddpageComponent, {
      width: '800px',
      data: this.dataSource.data
    });
    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((result: IPage) => {
        if (!result) {
          return;
        }
        this.dataSource.data.push(result);
        this.refreshTable();
        this.snackBar.open(`The page ${result.name} has been added successfully`, '', { duration: 3000 });
      });
  }
  onEdit() {
    const dialogRef = this.dialog.open(EditpageComponent, {
      width: '800px',
      data: {selectedPage: this.selectedPage, fullPageList: this.dataSource.data}
    });
    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((result: IPage) => {
        if (!result) {
          return;
        }
        const page = this.dataSource.data.find(s => s.id === result.id);
        if (page) {
          page.name = result.name;
          page.content = result.content;
          page.updatedAt = new Date();
          page.url = result.url;
          page.visibleInFooter = result.visibleInFooter;
          this.refreshTable();
          this.snackBar.open(`The page ${page.name} has been updated successfully`, '', { duration: 3000 });
        }
      });
  }
  onDelete() {
    const dialogRef = this.dialog.open(DeletepageComponent, {
      width: '500px',
      data: this.selectedPage
    });
    dialogRef.afterClosed()
      .pipe(takeUntil(this.onDestroy))
      .subscribe((page: IPage) => {
        if (!page) {
          return;
        }
        const index = this.dataSource.data.findIndex(s => s.id === page.id);
        if (index !== -1) {
          this.dataSource.data.splice(index, 1);
          this.refreshTable();
          this.snackBar.open(`The page ${page.name} has been deleted successfully`, '', { duration: 3000 });
        }
      });
  }
}
