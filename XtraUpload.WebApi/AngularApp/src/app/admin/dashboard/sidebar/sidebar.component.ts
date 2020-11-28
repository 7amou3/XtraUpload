import { Component, OnInit } from '@angular/core';
import { FlatTreeControl } from '@angular/cdk/tree';
import { MatTreeFlattener, MatTreeFlatDataSource } from '@angular/material/tree';
import { Router, ActivatedRoute } from '@angular/router';
import { ComponentBase } from 'app/shared';
import { UserStorageService } from 'app/services';

const TREE_DATA: IMenuNode[] = [
  {
    name: $localize`Dashboard`,
    iconname: 'poll',
    url: '/administration/overview'
  },
  {
    name: $localize`Users`,
    iconname: 'supervisor_account',
    children: [
      { name: $localize`Manage Users`, url: '/administration/users' },
      { name: $localize`Groups`, url: '/administration/groups' }
    ]
  },
  {
    name: $localize`Files`,
    iconname: 'file_copy',
    children: [
      { name: $localize`Manage Files`, url: '/administration/files' },
      { name: $localize`Extensions`, url: '/administration/extensions' }
    ]
  },
  {
    name: $localize`Storage Servers`,
    iconname: 'storage',
    children: [
      { name: $localize`Servers`, url: '/administration/servers' },
      { name: $localize`Health`, url: '/administration/servershealth' }
    ]
  },
  {
    name: $localize`Settings`,
    iconname: 'settings',
    url: '/administration/settings'
  },
  {
    name: $localize`Pages`,
    iconname: 'description',
    url: '/administration/pages'
  }
];

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent extends ComponentBase implements OnInit {
  selectedUrl: string;
  softVersion: string;
  treeControl = new FlatTreeControl<IFlatNode>(node => node.level, node => node.expandable);
  private _transformer = (node: IMenuNode, level: number) => {
    return {
      expandable: !!node.children && node.children.length > 0,
      name: node.name,
      iconname: node.iconname,
      url: node.url,
      level: level,
    };
  }
  treeFlattener = new MatTreeFlattener(this._transformer, node => node.level, node => node.expandable, node => node.children);

  dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private userstorgae: UserStorageService) {
    super();
    this.dataSource.data = TREE_DATA;
  }

  hasChild = (_: number, node: IFlatNode) => node.expandable;
  ngOnInit() {
    this.selectedUrl = this.router.url;
    const appInfo = this.userstorgae.getAppInfo();
    if (appInfo) {
      this.softVersion = appInfo.version;
    }
  }
  onMenuItemClick(node: IFlatNode) {
    this.selectedUrl = node.url;
    this.router.navigate([node.url]);
  }
}

interface IMenuNode {
  name: string;
  iconname?: string;
  url?: string;
  children?: IMenuNode[];
}
/** Flat node with expandable and level information */
interface IFlatNode {
  expandable: boolean;
  name: string;
  level: number;
  url: string;
}
