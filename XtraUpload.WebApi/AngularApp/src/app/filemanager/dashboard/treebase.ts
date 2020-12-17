import {MatTreeFlatDataSource, MatTreeFlattener} from '@angular/material/tree';
import { IFolderInfo, IFlatNode, IFolderNode } from 'app/models';
import { ComponentBase } from 'app/shared/components';
import { FlatTreeControl } from '@angular/cdk/tree';
import { TreeHelper } from './helpers';


/** Base class for XtraUpload tree views */
export abstract class TreeBase extends ComponentBase {
  protected readonly ROOT_NODE_NAME = 'root';
  /** a placeholder for the root folder, because the root folder is not persisted in db */
  protected readonly rootFolder: IFolderInfo = {
    id: this.ROOT_NODE_NAME,
    name: $localize`My Folders`,
    parentid: '',
    thumbnail: '',
    createdAt: new Date(),
    lastModified: new Date(),
    hasPassword: false,
    status: true
  };
  /** the current selected folder from the tree, used to apply css rules */
  selectedFolderId = this.ROOT_NODE_NAME;
  protected folders: IFolderInfo[] = [];
  treeControl = new FlatTreeControl<IFlatNode>(node => node.level, node => node.expandable);
  private _transformer = (node: IFolderNode, level: number) => {
    return {
      expandable: !!node.children && node.children.length > 0,
      name: node.name,
      level: level,
      id: node.id
    };
  }
  treeFlattener = new MatTreeFlattener(
    this._transformer, node => node.level, node => node.expandable, node => node.children);
  dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);
  protected folderTreeData: IFolderNode[] = [{
    id: this.ROOT_NODE_NAME,
    name: $localize`My Folders`,
    children: []
  }];
  hasChild = (_: number, node: IFlatNode) => node.expandable;

  protected buildFolderTree(folders: IFolderInfo[]): void {
    if (!folders) {
      return;
    }

    const roots: IFolderNode[] = [], children = {};

    // find the top level nodes and hash the children based on parent
    for (let i = 0; i < folders.length; ++i) {
      const item = folders[i],
        p = item.parentid,
        node: IFolderNode[] = !p ? roots : (children[p] || (children[p] = []));

      node.push({ id: item.id, name: item.name, children: [] });
    }

    // recursively build the tree
    const findChildren = (parent: IFolderNode): void => {
      if (children[parent.id]) {
        parent.children = children[parent.id];
        parent.children.forEach(child => {
          findChildren(child);
        });
      }
    };

    // enumerate through to handle the case where there are multiple roots
    roots.forEach(root => {
      findChildren(root);
    });

    // Display the tree
    this.folderTreeData = [...roots];
    if (this.dataSource.data.length === 0) {
      this.dataSource.data = [...roots];
    }
    else {
      // update visual tree without collapsing it
      new TreeHelper(this.treeControl).UpdateTreeSource(this.dataSource, this.folderTreeData);
    }
  }
}
