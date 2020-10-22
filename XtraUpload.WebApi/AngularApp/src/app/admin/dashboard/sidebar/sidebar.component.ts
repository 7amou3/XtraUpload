import { Component, OnInit } from '@angular/core';
import { FlatTreeControl } from '@angular/cdk/tree';
import { MatTreeFlattener, MatTreeFlatDataSource } from '@angular/material/tree';
import { Router, ActivatedRoute } from '@angular/router';
import { ComponentBase } from 'app/shared';

const TREE_DATA: IMenuNode[] = [
  {
    name: 'Dashboard',
    iconname: 'poll',
    url: '/administration/overview'
  },
  {
    name: 'Users',
    iconname: 'supervisor_account',
    children: [
      { name: 'Manage Users', url: '/administration/users' },
      { name: 'Groups', url: '/administration/groups' }
    ]
  },
  {
    name: 'Files',
    iconname: 'file_copy',
    children: [
      { name: 'Manage Files', url: '/administration/files' },
      { name: 'Extensions', url: '/administration/extensions' }
    ]
  },
  {
    name: 'Storage Servers',
    iconname: 'storage',
    url: '/administration/servers'
  },
  {
    name: 'Settings',
    iconname: 'settings',
    url: '/administration/settings'
  },
  {
    name: 'Pages',
    iconname: 'description',
    url: '/administration/pages'
  }
];

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent extends ComponentBase implements OnInit {
  selectedUrl: string;
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
    private route: ActivatedRoute) {
    super();
    this.dataSource.data = TREE_DATA;
  }

  hasChild = (_: number, node: IFlatNode) => node.expandable;
  ngOnInit() {
    this.selectedUrl = this.router.url;
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
