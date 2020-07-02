import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IItemInfo, IBulkDelete } from 'app/domain';
import { isFile } from '../../dashboard/helpers';
import { ComponentBase } from 'app/shared';
import { FileManagerService } from 'app/services';
import { takeUntil, finalize } from 'rxjs/operators';

@Component({
  selector: 'app-deleteitem',
  templateUrl: './deleteitem.component.html'
})
export class DeleteItemComponent extends ComponentBase {
  message: string;
  constructor(
    private fileManagerService: FileManagerService,
    private dialogRef: MatDialogRef<DeleteItemComponent>,
    @Inject(MAT_DIALOG_DATA) private items: IItemInfo[]) {
      super();
      if (items.length > 1) {
        let fileCount = 0, folderCount = 0;
        items.forEach(f => {
          if (isFile(f)) {
            fileCount++;
          }
          else folderCount++;
        });
        this.message = folderCount === 1 ? `1 folder` :
        folderCount > 1 ? `${folderCount} folders` : '';

        this.message += folderCount > 0 && fileCount > 0 ? ', ' : '';

        this.message += fileCount === 1 ? `1 file` :
        fileCount > 1 ? `${fileCount} files` : '';
      }
      else {
        this.message = items[0].name;
      }
  }

  onDelete() {
    this.isBusy = true;
    this.fileManagerService.deleteItems(this.items)
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.isBusy = false))
      .subscribe(
        (deletedItems: IBulkDelete) => {
          this.dialogRef.close(deletedItems);
        }
      );
  }

}
