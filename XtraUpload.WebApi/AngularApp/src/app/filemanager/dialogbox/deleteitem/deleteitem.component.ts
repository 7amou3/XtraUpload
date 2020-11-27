import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IItemInfo, IBulkDelete } from 'app/domain';
import { isFile } from '../../dashboard/helpers';
import { ComponentBase } from 'app/shared';
import { FileManagerService } from 'app/services';

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
      this.message = folderCount === 1 ? $localize`1 folder` :
        folderCount > 1 ? $localize`${folderCount} folders` : '';

      this.message += folderCount > 0 && fileCount > 0 ? ', ' : '';

      this.message += fileCount === 1 ? $localize`1 file` :
        fileCount > 1 ? $localize`${fileCount} files` : '';
    }
    else {
      this.message = items[0].name;
    }
  }

  async onDelete() {
    this.isBusy = true;
    await this.fileManagerService.deleteItems(this.items)
      .then((deletedItems: IBulkDelete) => {
        this.dialogRef.close(deletedItems);
      })
      .catch(error => this.handleError(error))
      .finally(() => this.isBusy = false);
  }

}
