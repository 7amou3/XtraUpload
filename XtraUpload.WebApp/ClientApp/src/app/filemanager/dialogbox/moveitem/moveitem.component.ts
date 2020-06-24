import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FileManagerService } from 'app/services';
import { IItemInfo, IFolderNode } from 'app/domain';
import { takeUntil, finalize } from 'rxjs/operators';
import { TreeBase } from 'app/filemanager/dashboard/treebase';

@Component({
  selector: 'app-moveitem',
  templateUrl: './moveitem.component.html',
  styleUrls: ['./moveitem.component.css']
})
export class MoveItemComponent extends TreeBase implements OnInit {
  constructor(
    private filemanagerService: FileManagerService,
    private dialogRef: MatDialogRef<MoveItemComponent>,
    @Inject(MAT_DIALOG_DATA) private items: IItemInfo[]) {
      super();
     }

  ngOnInit(): void {
    this.filemanagerService.getAllFolders()
    .pipe(takeUntil(this.onDestroy))
    .subscribe(
      folders => {
        // build the folders tree
        this.folders = [this.rootFolder, ...folders ?? []];
        this.buildFolderTree(this.folders);
        this.treeControl.dataNodes.forEach(node => {
          this.treeControl.expand(node);
        });
      });
  }
  onItemClick(node: IFolderNode) {
    this.selectedFolderId = node.id;
  }
  onMove() {
    this.isBusy = true;
    this.filemanagerService.requestMoveItems(this.items, this.selectedFolderId)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
    .subscribe(
      () => {
        this.dialogRef.close();
      },
      error => this.handleError(error)
    );
  }
}
