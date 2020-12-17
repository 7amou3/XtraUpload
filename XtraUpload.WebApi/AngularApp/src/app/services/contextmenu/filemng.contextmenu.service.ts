import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuTrigger } from '@angular/material/menu';
import { isFile } from '../../filemanager/dashboard/helpers';
import { IItemsMenu, itemAction, IItemInfo } from 'app/models';
import { ContextMenuBase } from './contextmenu.base';
import { FileManagerService } from 'app/services';

const itemsMenu: IItemsMenu[] = [
    { description: $localize`Move Selected`, icon: 'open_with', action: itemAction.move },
    { description: $localize`Delete`, icon: 'delete', class: 'text-danger', action: itemAction.delete }
];
const fileMenu: IItemsMenu[] = [
    { description: $localize`Info`, icon: 'info', action: itemAction.info },
    { description: $localize`Rename`, icon: 'edit', action: itemAction.rename },
    { description: $localize`Download`, icon: 'get_app', action: itemAction.download },
    ...itemsMenu
];
const folderMenu: IItemsMenu[] = [
    { description: $localize`Open`, icon: 'subdirectory_arrow_right', action: itemAction.openFolder },
    { description: $localize`New Folder`, icon: 'create_new_folder', action: itemAction.create },
    ...fileMenu
];

@Injectable()
export class FileMngContextMenuService extends ContextMenuBase {

    constructor(fileManagerService: FileManagerService, router: Router, dialog: MatDialog) {
        super(fileManagerService, router, dialog);
    }

    displayContextMenu(contextMenu: MatMenuTrigger, event: MouseEvent, items: IItemInfo[]): void {
        this.contextMenuPosition.x = event.clientX + 'px';
        this.contextMenuPosition.y = event.clientY + 'px';
        contextMenu.menuData = { itemsMenu: this.itemsMenu$, selectedItems: items };
        contextMenu.menu.focusFirstItem('mouse');
        contextMenu.openMenu();
        this.populateContextMenu(items);
    }

    private populateContextMenu(items: IItemInfo[]) {
        // Multi-select
        if (items.length > 1) {
            this.itemsMenu$.next(itemsMenu);
        }
        else if (isFile(items[0])) {
            this.itemsMenu$.next(fileMenu);
        }
        // tslint:disable-next-line:one-line
        else {
            this.itemsMenu$.next(folderMenu);
        }
    }

}
