import { Component, OnInit, Inject } from '@angular/core';
import { IItemInfo, IFolderInfo, IFileInfo } from 'app/domain';
import { FilemanagerBase } from '../filemanagerbase';
import { FileManagerService, UploadService } from 'app/services';
import { FileMngContextMenuService } from 'app/services/contextmenu';
import { rowAnimation } from '../helpers';

/** Only table view is suported, grid view will be available once cdk support it
    see issue: https://github.com/angular/components/issues/13372 */
@Component({
  selector: 'app-gridview',
  templateUrl: './gridview.component.html',
  styleUrls: ['./gridview.component.css'],
  animations: [rowAnimation]
})
export class GridviewComponent extends FilemanagerBase implements OnInit {
  /** Flag to apply css rule on the view */
  dragging = false;
  itemsContent: IItemInfo[] = [];
  constructor(
    filemanagerService: FileManagerService,
    uploadService: UploadService,
    public ctxMenuService: FileMngContextMenuService,
    @Inject('API_URL') apiUrl: string) {
    super(filemanagerService, uploadService, ctxMenuService, apiUrl);
  }

  ngOnInit(): void {
    this.hookEvents();
  }

  protected itemsReceived(itemsInfo: IItemInfo[]) {
    itemsInfo.forEach(item => {
      this.setitemThumbnail(item);
    });
    this.itemsContent = itemsInfo;
  }

  /** Handles the drop event */
  protected handleDrop(index: number): void {
    console.log(index);
  }

  /** invoked when a new sub folder has been added */
  protected handleNewSubFolder(folder: IFolderInfo): void {

  }
  /** invoked when a new sub folder has been added */
  protected handleDeleteSubFolder(folder: IFolderInfo): void {

  }
  /** invoked when a file has been deleted */
  protected handleDeleteFile(file: IFileInfo): void {

  }
  /** invoked when a file has been renamed */
  protected handleRenameFile(file: IFileInfo): void {

  }
   /** invoked when a folder has been renamed */
  protected handleRenameFolder(folder: IFolderInfo): void {

  }
  /** invoked when a file online availability changed */
  protected  handleFileAvailability(file: IFileInfo): void {

  }
   /** invoked when a folder online availability changed */
   protected handleFolderAvailability(folder: IFolderInfo): void {

   }
   /** Invoked when item(s) has moved successfully */
   protected handleMovedItems(itemsIds: string[]): void {}
  /** Invoked when a new file has been successfully uploaded */
  protected handleNewFile(file: IFileInfo) {

  }
}
