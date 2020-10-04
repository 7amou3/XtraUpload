import { Component, OnInit, Input } from '@angular/core';
import { FileManagerService, SeoService } from 'app/services';
import { TreeBase } from 'app/filemanager/dashboard/treebase';
import { takeUntil } from 'rxjs/operators';
import { IFlatNode } from 'app/domain';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent extends TreeBase implements OnInit {

  @Input() folderId: string;
  selectedFolderId: string;
  constructor(
    private filemanagerService: FileManagerService,
    private seoService: SeoService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    super();
   }

  ngOnInit(): void {
    this.route.queryParamMap
    .pipe(takeUntil(this.onDestroy))
    .subscribe(
      params => {
        const folderId = params.get('id');
        const subfolderId = params.get('sub');
        this.selectedFolderId = subfolderId ?? folderId;
      }
    );
    this.filemanagerService.getFolderTreeById(this.folderId)
    .pipe(takeUntil(this.onDestroy))
    .subscribe(
      folders => {
        this.seoService.setPageTitle('Download ' + folders[folders.length - 1].name);
        const rootFolder = {
          id: this.folderId,
          name: folders[folders.length - 1].name,
          parentid: '',
          thumbnail: '',
          createdAt: new Date(),
          lastModified: new Date(),
          hasPassword: false,
          status: true
        };
        // build the folders tree
        this.folders = [rootFolder, ...folders ?? []];
        this.buildFolderTree(this.folders);
        this.treeControl.dataNodes.forEach(node => {
          this.treeControl.expand(node);
        });
      });
  }

  onItemClick(node: IFlatNode) {
    const folder = this.folders.find(s => s.id === node.id);
    if (folder && this.folderId === folder.id) {
      this.router.navigate(['/folder'], { queryParams: { id: this.folderId } });
    }
    else this.router.navigate(['/folder'], { queryParams: { id: this.folderId, sub: folder.id }});

  }

}
