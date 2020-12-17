import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuTrigger } from '@angular/material/menu';
import { itemAction, IFlatNode, } from 'app/models';
import { ContextMenuBase } from './contextmenu.base';
import { FileManagerService } from 'app/services';

const rootFolderMenu = [
  { description: $localize`Open`, icon: 'subdirectory_arrow_right', action: itemAction.openFolder },
  { description: $localize`New Folder`, icon: 'create_new_folder', action: itemAction.create },
];

const folderMenu = [
  ...rootFolderMenu,
  { description: $localize`Info`, icon: 'info', action: itemAction.info },
  { description: $localize`Rename`, icon: 'edit', action: itemAction.rename },
  { description: $localize`Download`, icon: 'get_app', action: itemAction.download },
  { description: $localize`Move Selected`, icon: 'open_with', action: itemAction.move },
  // { description: 'Set Password', icon: 'lock', action: itemAction.setPassword },
  { description: $localize`Delete`, icon: 'delete', class: 'text-danger', action: itemAction.delete }
];

@Injectable()
export class SnavContextMenuService extends ContextMenuBase {

  constructor(fileManagerService: FileManagerService, router: Router, dialog: MatDialog) {
    super(fileManagerService, router, dialog);
  }

  displayContextMenu(contextMenu: MatMenuTrigger, event: MouseEvent, item: IFlatNode): void {
    // set the coordiante of the menu
    this.contextMenuPosition.x = event.clientX + 'px';
    this.contextMenuPosition.y = event.clientY + 'px';
    contextMenu.menuData = { itemsMenu: this.itemsMenu$, selectedItem: item };
    contextMenu.menu.focusFirstItem('mouse');
    contextMenu.openMenu();
    this.populateContextMenu(item);
  }

  private populateContextMenu(node: IFlatNode) {
    if (node.id === 'root') {
      this.itemsMenu$.next(rootFolderMenu);
    }
    // tslint:disable-next-line:one-line
    else {
      this.itemsMenu$.next(folderMenu);
    }
  }
}
