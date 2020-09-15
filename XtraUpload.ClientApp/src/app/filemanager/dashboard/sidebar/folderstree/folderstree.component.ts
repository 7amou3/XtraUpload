import { Component, OnInit, ViewChild, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatMenuTrigger } from '@angular/material/menu';
import { merge } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { FileManagerService } from 'app/services';
import { SnavContextMenuService } from 'app/services/contextmenu';
import { IFolderInfo, IFolderNode, IFlatNode, itemAction } from 'app/domain';
import { TreeHelper } from '../../helpers';
import { TreeBase } from '../../treebase';

@Component({
  selector: 'app-folderstree',
  templateUrl: './folderstree.component.html',
  styleUrls: ['./folderstree.component.css']
})
export class FoldersTreeComponent extends TreeBase implements OnInit {
  @Output() folderPathChanged$ = new EventEmitter<IFolderInfo[]>();
  @ViewChild(MatMenuTrigger) contextMenu: MatMenuTrigger;
  /** the folderid on wich the context menu is open, used to apply css rules */
  contextMenuFolderId = '';
  constructor(
    private filemanagerService: FileManagerService,
    public contextMenuService: SnavContextMenuService,
    private route: ActivatedRoute) {
    super();
  }

  ngOnInit(): void {
    const queryParams$ = this.route.queryParamMap.pipe(takeUntil(this.onDestroy));
    const allFolders$ = this.filemanagerService.getAllFolders().pipe(takeUntil(this.onDestroy));
    const treeChanged = this.filemanagerService.folderTreeChanged$.pipe(takeUntil(this.onDestroy));

    merge(treeChanged, allFolders$)
    .subscribe(
      folders => {
        // build the folders tree
        this.folders = [this.rootFolder, ...folders ?? []];
        this.buildFolderTree(this.folders);
      }
    );

    // Wait for the get folder content query to resolve
    allFolders$.toPromise().then(() => {
      // Sets the style for the active selected folder
      queryParams$.subscribe(params => {
        let param = params.get('folder');
        if (!param) {
          param = this.ROOT_NODE_NAME;
        }
        this.selectedFolderId = param;
        // Notify parent component of the selected folder path
        const folder = this.folders.find(s => s.id === this.selectedFolderId);
        if (folder) {
          this.folderPathChanged$.emit(this.getPathToChild(folder));
        }
      });
    }, () => {
      throw new Error('Unknown error occured while waiting for server response.');
    });

    // Add new subfolder to tree on creation event
    this.filemanagerService.subFolderCreated$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(newFolder => {
        this.folderAdded(newFolder);
      });

    // Folder renamed
    this.filemanagerService.folderRenamed$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(folder => {
        this.folderRenamed(folder);
      });

    // Folder deleted
    this.filemanagerService.subFolderDeleted$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(folder => {
        this.folderDeleted(folder);
      });

    // Folder availability changed
    this.filemanagerService.folderAvailabilityChanged$
    .pipe(takeUntil(this.onDestroy))
      .subscribe(folder => {
        this.folderAvailabiltyChanged(folder);
      });
  }
  folderAvailabiltyChanged(model: IFolderInfo): void {
    if (!model) {
      return;
    }
    // update the folder availability in the local collection
    const rfolder = this.folders.find(s => s.id === model.id);
    if (rfolder) {
      rfolder.name = model.name;
      rfolder.isAvailableOnline = model.isAvailableOnline;
    }
  }
  private folderRenamed(model: IFolderInfo): void {
    if (!model) {
      return;
    }

    const parent = this.getParentFolder(model);
    if (parent) {
      // rename the folder in the local collection
      const rfolder = this.folders.find(s => s.id === model.id);
      if (rfolder) {
        rfolder.name = model.name;
        rfolder.lastModified = model.lastModified;
      }
      // rename the folder into the parent and thus into the tree
      if (!parent.children) {
        parent.children = [];
      }
      parent.children.find(s => s.id === model.id).name = model.name;
      // update visual tree without collapsing it
      new TreeHelper(this.treeControl).UpdateTreeSource(this.dataSource, this.folderTreeData);
    }
  }
  private folderDeleted(model: IFolderInfo): void {
    if (!model) {
      return;
    }

    const parent = this.getParentFolder(model);
    if (parent) {
      // remove the folder from the local collection
      const index = this.folders.findIndex(s => s.id === model.id);
      if (index !== -1) {
        this.folders.splice(index, 1);
      }

      const childIndex = parent.children.findIndex(s => s.id === model.id);
      if (index !== -1) {
        parent.children.splice(childIndex, 1);
      }
      // update visual tree without collapsing it
      new TreeHelper(this.treeControl).UpdateTreeSource(this.dataSource, this.folderTreeData);
    }
  }
  /** Add the new subfolder to tree */
  private folderAdded(model: IFolderInfo): void {
    if (!model) {
      return;
    }

    const parent = this.getParentFolder(model);
    if (parent) {
      // add the new subfolder to the local collection
      this.folders.push(model);
      // push the new sub folder into the parent and thus into the tree
      if (!parent.children) {
        parent.children = [];
      }
      parent.children.push(model);
      // update visual tree without collapsing it
      new TreeHelper(this.treeControl).UpdateTreeSource(this.dataSource, this.folderTreeData);
    }
  }

  /** Gets the parent reference from the nested tree datasource array */
  private getParentFolder(model: IFolderInfo): IFolderNode {
    const findParent = (id: string, children: IFolderNode[]): IFolderNode => {
      let parentFolder: IFolderNode;
      for (const child of children) {
        if (child.id === id) {
          parentFolder = child;
          break;
        }
        const subRes = findParent(id, child.children);
        if (subRes) {
          return subRes;
        }
      }
      return parentFolder;
    };
    const parent = findParent(model.parentid, this.folderTreeData);
    return parent;
  }

  /** returns a flat array leading to child location */
  getPathToChild(child: IFolderInfo): IFolderInfo[] {
    const folders: IFolderInfo[] = [];
    if (child.id !== this.ROOT_NODE_NAME) {
      folders.push(child);
    }

    const findParent = (folder: IFolderInfo) => {
      const parentFolder = this.folders.find(s => s.id === folder.parentid);
      if (parentFolder) {
        folders.push(parentFolder);
        findParent(parentFolder);
      }
    };

    // if root folder just add it and return
    if (child.parentid === '') {
      return [child];
    }

    // if a child folder, lookup its path
    findParent(child);
    return folders.reverse();
  }

  /** Handles the right click (or the click on the menu icon) in order to display the file or folder menu */
  onContextMenu(event: MouseEvent, node: IFlatNode): void {
    event.preventDefault();
    event.stopPropagation();

    this.contextMenuFolderId = node.id;
    this.contextMenuService.displayContextMenu(this.contextMenu, event, node);
  }

  onContextMenuClosed() {
    this.contextMenuFolderId = '';
  }

  /** Handles the click action on the menu item (rename, edit, delete...) */
  onMenuItemClick(node: IFlatNode, action: itemAction) {
    const folder = this.folders.find(s => s.id === node.id);
    if (folder) {
      this.contextMenuService.handleMenuItemClick([folder], action);
    }
  }
}
