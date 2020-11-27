import { Input, ViewChild, Directive } from '@angular/core';
import { MatMenuTrigger, MatMenu } from '@angular/material/menu';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { takeUntil } from 'rxjs/operators';
import { ReplaySubject } from 'rxjs';
import { isFile } from './helpers';
import { IItemInfo, itemAction, IFolderInfo, IFileInfo } from 'app/domain';
import { ComponentBase } from 'app/shared';
import { FileManagerService, UploadService } from 'app/services';
import { FileMngContextMenuService } from 'app/services/contextmenu';

/** Manages the common functionalities of a filemanager component  */
@Directive()
export abstract class FilemanagerBase extends ComponentBase {
  @Input() folderContent$: ReplaySubject<IItemInfo[]>;
  /** Selected items (files and folders) */
  selections: IItemInfo[] = [];
  /** Flag to apply css rule on the view */
  dragging = false;
  /** An item (file or folder) has been added to the selections array */
  itemAdded = false;
  @ViewChild(MatMenuTrigger) contextMenu: MatMenuTrigger;
  @ViewChild(MatMenu) matMenu: MatMenu;

  constructor(
    protected filemanagerService: FileManagerService,
    protected uploadService: UploadService,
    public ctxMenuService: FileMngContextMenuService,
    private apiUrl: string) {
    super();
  }
  protected hookEvents() {
    this.folderContent$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(
        items => {
          this.itemsReceived(items);
        }
      );
    this.filemanagerService.subFolderCreated$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(newFolder => {
        this.setitemThumbnail(newFolder);
        this.handleNewSubFolder(newFolder);
      });

    this.filemanagerService.subFolderDeleted$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(folder => {
        this.handleDeleteSubFolder(folder);
      });

    this.filemanagerService.fileDeleted$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(file => {
        this.handleDeleteFile(file);
      });

    this.filemanagerService.fileRenamed$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(file => {
        this.handleRenameFile(file);
      });

    this.filemanagerService.folderRenamed$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(folder => {
        this.handleRenameFolder(folder);
      });
    this.uploadService.fileUploaded$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(file => {
        this.handleNewFile(file);
      });
    this.filemanagerService.fileAvailabilityChanged$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(file => {
        this.handleFileAvailability(file);
      });
    this.filemanagerService.folderAvailabilityChanged$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(folder => {
        this.handleFolderAvailability(folder);
      });
      this.filemanagerService.itemsMoved$
      .pipe(takeUntil(this.onDestroy))
      .subscribe((itemsIds: string[]) => {
        this.handleMovedItems(itemsIds);
      });
  }

  /**Invoked when a new file has been uploaded successfully*/
  protected abstract handleNewFile(file: IFileInfo);
  /** invoked when items (folders/files) has been received from parent component */
  protected abstract itemsReceived(itemsInfo: IItemInfo[]): void;
  /** invoked on drop event */
  protected abstract handleDrop(dropIndex: number): void;
  /** invoked when a new sub folder has been added */
  protected abstract handleNewSubFolder(folder: IFolderInfo): void;
  protected abstract handleDeleteSubFolder(folder: IFolderInfo): void;
  /** invoked when a file has been deleted */
  protected abstract handleDeleteFile(file: IFileInfo): void;
  /** invoked when a file has been renamed */
  protected abstract handleRenameFile(file: IFileInfo): void;
  /** invoked when a folder has been renamed */
  protected abstract handleRenameFolder(folder: IFolderInfo): void;
  /** invoked when a file online availability changed */
  protected abstract handleFileAvailability(file: IFileInfo): void;
  /** invoked when a folder online availability changed */
  protected abstract handleFolderAvailability(folder: IFolderInfo): void;
  /** Invoked when item(s) has moved successfully */
  protected abstract handleMovedItems(itemsIds: string[]): void;
  /** Set the thumbnail for items */
  protected setitemThumbnail(item: IItemInfo) {
    if (!item) {
      return;
    }

    if (isFile(item)) {
      if (item.mimeType.startsWith('image')) {
        // Add '/' at the end of url
        const address = (item as IFileInfo).storageServer?.address?.replace(/\/?$/, '/');
        item.thumbnail = address + 'api/file/smallthumb/' + item.id;
      }
      else {
        if (item.extension) {
          // the server return the extension as '.pdf' '.docx'.. we need to subtract the '.'
          item.thumbnail = 'assets/images/extensions/' + item.extension.split('.')[1] + '.png';
        }
        else item.thumbnail = 'assets/images/extensions/unknown.png';
      }
    }
  }
  removeItemFromSelection(selectedRow: IItemInfo): void {
    if (this.itemAdded === false) {
      this.selections.forEach((file, i) => {
        if (file === selectedRow) {
          this.selections.splice(i, 1);
        }
      });
    }
    this.itemAdded = false;
  }
  isdroppableArea(item: IItemInfo): boolean {
    return !isFile(item) && !this.selections.includes(item);
  }
  /** Description shown when items being dragged */
  getDragDesc(): string {
    let dragDesc = '';
    let filesCount = 0;
    let foldersCount = 0;
    this.selections.forEach(item => {
      if (isFile(item)) {
        filesCount++;
      }
      else {
        foldersCount++;
      }
    });
    dragDesc = foldersCount === 1 ? $localize`1 folder` :
      foldersCount > 1 ? $localize`${foldersCount} folders` : '';

    dragDesc += foldersCount > 0 && filesCount > 0 ? ', ' : '';

    dragDesc += filesCount === 1 ? $localize`1 file` :
      filesCount > 1 ? $localize`${filesCount} files` : '';

    return dragDesc;
  }
  /** Handles the right click (or the click on the menu icon) in order to display the file or folder menu */
  onContextMenu(event: MouseEvent): void {
    event.preventDefault();
    event.stopPropagation();
    if (this.contextMenu.menuOpen) {
      this.contextMenu.closeMenu();
    }
    // display the menu
    this.ctxMenuService.displayContextMenu(this.contextMenu, event, this.selections);
  }

  /** Handles the mouse down event on the item (row, grid) */
  onItemMouseDown(selectedRow: IItemInfo) {
    if (!selectedRow) {
      return;
    }
    if (this.contextMenu.menuOpen) {
      this.contextMenu.closeMenu();
    }
    // push the selected element if it's not in the array
    if (!this.selections.includes(selectedRow)) {
      this.selections.push(selectedRow);
      this.itemAdded = true;
    }
  }
  /** Handles the clic on an item */
  onItemClick(selectedRow: IItemInfo): void {
    if (!selectedRow) {
      return;
    }

    this.removeItemFromSelection(selectedRow);
  }

  /** Handles the drag start event */
  onDragStart(item: IItemInfo): void {
    this.dragging = true;
    this.itemAdded = true;
    this.removeItemFromSelection(item);
  }

  /** Handles the drop event (must be overidden by derived components) */
  onDrop(event: CdkDragDrop<string[]>): void {
    this.dragging = false;
    this.handleDrop(event.currentIndex);
  }

  /** Handles the click action on the menu item (rename, edit, delete...) */
  onMenuItemClick(items: IItemInfo[], action: itemAction) {
    this.ctxMenuService.handleMenuItemClick(items, action);
  }

}
