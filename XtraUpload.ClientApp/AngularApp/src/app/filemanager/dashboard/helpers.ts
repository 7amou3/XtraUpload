import { IItemInfo, IFileInfo, IFolderNode, IFlatNode } from '../../domain';
import { trigger, animate, transition, style } from '@angular/animations';
import { FlatTreeControl } from '@angular/cdk/tree';

/** type guard, checks wether the item is a file or a folder */
export function isFile(item: IItemInfo): item is IFileInfo {
  return (item as IFileInfo).folderId !== undefined;
}
/** function that accepts an item from the array and returns a promise. */
export function forEachPromise<T>(items, fn, context?) {
  return items.reduce(function (promise, item) {
      return promise.then(function () {
          return fn(item, context);
      });
  }, Promise.resolve());
}

export const rowAnimation =
  trigger('rowAnimation', [
    transition('void => *', [
      style({ opacity: 0, transform: 'translateX(-100%)' }),
      animate('0.4s ease-out', style({ opacity: '.2', transform: 'translateX(0)' }))
    ])
  ]);
/** Update the tree without collapsing it */
export class TreeHelper {
  private expandedNodes: IFolderNode[];

  constructor(private treeControl: FlatTreeControl<IFlatNode>) {
    return this;
  }

  UpdateTreeSource(datasource: any, newData: IFolderNode[]) {
    this.saveExpandedNodes();
    datasource.data = newData;
    this.restoreExpandedNodes();
  }
  private saveExpandedNodes() {
    this.expandedNodes = new Array<IFolderNode>();
    this.treeControl.dataNodes.forEach(node => {
      if (node.expandable && this.treeControl.isExpanded(node)) {
        this.expandedNodes.push(node);
      }
    });
  }

  private restoreExpandedNodes() {
    this.expandedNodes.forEach(node => {
      this.treeControl.expand(this.treeControl.dataNodes.find(n => n.id === node.id));
    });
  }

}
