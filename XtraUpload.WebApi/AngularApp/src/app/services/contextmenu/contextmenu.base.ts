import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { ComponentBase } from 'app/shared/components';
import { takeUntil } from 'rxjs/operators';
import { RenameItemComponent, DeleteItemComponent, PasswordComponent, CreatefolderComponent,
    MoveItemComponent } from 'app/filemanager/dialogbox';
import { itemAction, IItemsMenu, IItemInfo, IRenameItemModel, ISetPasswordItemModel, ICreateFolderModel,
    IFolderInfo } from 'app/models';
import { FileManagerService } from 'app/services';
import { isFile } from 'app/filemanager/dashboard/helpers';

export abstract class ContextMenuBase extends ComponentBase {
    protected itemsMenu$ = new Subject<IItemsMenu[]>();
    public contextMenuPosition = { x: '0px', y: '0px' };
    public itemInfoRequested$ = new Subject<IItemInfo>();
    constructor(
        private fileManagerService: FileManagerService,
        private router: Router,
        private dialog: MatDialog) {
        super();

    }
    handleMenuItemClick(items: IItemInfo[], action: itemAction) {
        switch (action) {
            case itemAction.info:
                this.itemInfoRequested$.next(items[0]);
                break;
            case itemAction.openFolder:
                this.navigateToFolder(items[0]);
                break;
            case itemAction.download:
                this.downloadItem(items[0]);
                break;
            case itemAction.create:
                this.openCreateFolderDialog(items[0]);
                break;
            case itemAction.rename:
                this.openRenameDialog(items[0]);
                break;
            case itemAction.move:
                this.openMoveDialog(items);
                break;
            case itemAction.delete:
                this.openDeleteDialog(items);
                break;
        }
    }
    downloadItem(item: IItemInfo) {
        //this.router.navigate(['/folder'], { queryParams: { f: item.id } });
        if (isFile(item)) {
            window.open('/file?id=' + item.id, '_blank');
        }
        else {
            window.open('/folder?id=' + item.id, '_blank');
        }
    }
    private navigateToFolder(item: IItemInfo): void {
        if (item.id === 'root') {
            this.router.navigate(['/filemanager']);
        }
        else {
            this.router.navigate(['/filemanager'], { queryParams: { folder: item.id } });
        }
    }
    private openCreateFolderDialog(item: IItemInfo): void {
        const dialogRef = this.dialog.open(CreatefolderComponent, {
            width: '500px',
            data: item as IFolderInfo
        });
        dialogRef.afterClosed()
            .pipe(takeUntil(this.onDestroy))
            .subscribe(async (subFolder: ICreateFolderModel) => {
                if (subFolder) {
                    // request create sub folder
                    await this.fileManagerService.createSubFolder(subFolder);
                }
            });
    }
    private openRenameDialog(item: IItemInfo): void {
        const dialogRef = this.dialog.open(RenameItemComponent, {
            width: '500px',
            data: item
        });

        dialogRef.afterClosed()
            .pipe(takeUntil(this.onDestroy))
            .subscribe(async (result: IRenameItemModel) => {
                if (result) {
                    if (isFile(item)) {
                        // request rename the file
                        await this.fileManagerService.renameFile(result);
                    }
                    else await this.fileManagerService.renameFolder(result);
                }
            });
    }
    private openPasswordDialog(item: IItemInfo): void {
        const dialogRef = this.dialog.open(PasswordComponent, {
            width: '500px',
            data: item
        });

        dialogRef.afterClosed()
            .pipe(takeUntil(this.onDestroy))
            .subscribe((result: ISetPasswordItemModel) => {
                console.log(result);
                if (result) {
                    // set item password
                }
            });
    }
    private openMoveDialog(items: IItemInfo[]): void {
        this.dialog.open(MoveItemComponent, {
            width: '500px',
            data: items,
            autoFocus: false
        });

    }
    private openDeleteDialog(items: IItemInfo[]): void {
        this.dialog.open(DeleteItemComponent, {
            width: '500px',
            data: items
        });
    }
}
