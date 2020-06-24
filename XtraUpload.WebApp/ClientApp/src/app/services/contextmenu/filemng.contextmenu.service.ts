import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatMenuTrigger, MatDialog } from '@angular/material';
import { isFile } from '../../filemanager/dashboard/helpers';
import { IItemsMenu, itemAction, IItemInfo } from '../../domain';
import { ContextMenuBase } from './contextmenu.base';
import { FileManagerService } from '../../services';

const itemsMenu: IItemsMenu[] = [
    { description: 'Move Selected', icon: 'open_with', action: itemAction.move },
    { description: 'Delete', icon: 'delete', class: 'text-danger', action: itemAction.delete }
];
const fileMenu: IItemsMenu[] = [
    { description: 'Info', icon: 'info', action: itemAction.info },
    { description: 'Rename', icon: 'edit', action: itemAction.rename },
    { description: 'Download', icon: 'get_app', action: itemAction.download },
    ...itemsMenu
];
const folderMenu: IItemsMenu[] = [
    { description: 'Open', icon: 'subdirectory_arrow_right', action: itemAction.openFolder },
    { description: 'New Folder', icon: 'create_new_folder', action: itemAction.create },
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
