import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FileManagerService } from 'app/services';
import { IItemInfo, IFolderNode } from 'app/models';
import { TreeBase } from 'app/filemanager/dashboard/treebase';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-moveitem',
  templateUrl: './moveitem.component.html',
  styleUrls: ['./moveitem.component.css']
})
export class MoveItemComponent extends TreeBase implements OnInit {
  constructor(
    private snackBar: MatSnackBar,
    private filemanagerService: FileManagerService,
    private dialogRef: MatDialogRef<MoveItemComponent>,
    @Inject(MAT_DIALOG_DATA) private items: IItemInfo[]) {
      super();
     }

  ngOnInit() {
    this.filemanagerService.getAllFolders()
    .pipe(takeUntil(this.onDestroy))
    .subscribe(folders => {
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
  async onMove() {
    this.isBusy = true;
    await this.filemanagerService.requestMoveItems(this.items, this.selectedFolderId)
    .then(() => {
        this.dialogRef.close();
    })
    .catch(error => this.handleError(error, this.snackBar))
    .finally(() => this.isBusy = false);
  }
}
