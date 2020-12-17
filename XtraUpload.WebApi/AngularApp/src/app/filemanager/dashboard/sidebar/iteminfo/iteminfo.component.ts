import { Component, OnInit, Input } from '@angular/core';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { FileManagerService } from 'app/services';
import { IItemInfo, IFileInfo, IFolderInfo } from 'app/models';
import { isFile } from '../../helpers';
import { ComponentBase } from 'app/shared/components';

@Component({
  selector: 'app-iteminfo',
  templateUrl: './iteminfo.component.html',
  styleUrls: ['./iteminfo.component.css']
})
export class IteminfoComponent extends ComponentBase implements OnInit {
  @Input() itemInfo$: Subject<IItemInfo>;
  @Input() folderPath: IFolderInfo[];
  simplePath: string;
  itemInfo: IItemInfo;
  isFile: boolean;
  downloadUrl: string;
  constructor(
    private snackBar: MatSnackBar,
    private fileMngService: FileManagerService) {
    super();
  }

  ngOnInit(): void {
    const baseurl = document.getElementsByTagName('base')[0].href;
    this.itemInfo$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(
        item => {
          this.itemInfo = Object.assign(item);
          this.isFile = isFile(item);
          if (this.isFile) {
            if ((item as IFileInfo).mimeType.startsWith('image')) {
              const storageUrl = (item as IFileInfo).storageServer.address.replace(/\/?$/, '/');
              item.thumbnail = storageUrl + 'api/file/mediumthumb/' + item.id;
            }
          }
          this.simplePath = '';
          this.downloadUrl = baseurl + (this.isFile ? 'file?id=' : 'folder?id=') + this.itemInfo.id
          for (let i = 0; i < this.folderPath.length; i++) {
            this.simplePath += this.folderPath[i].name + (i !== (this.folderPath.length - 1) ? ' > ' : '');
          }
        }
      );
  }
  async onAvailableItemChange(event: MatSlideToggleChange) {
    let serviceCall: Promise<IFileInfo | IFolderInfo>;
    if (isFile(this.itemInfo)) {
      serviceCall = this.fileMngService.updateFileAvailability({ itemId: this.itemInfo.id, available: event.checked });
    }
    else {
      serviceCall = this.fileMngService.updateFolderAvailability({ itemId: this.itemInfo.id, available: event.checked });
    }
    
    await serviceCall.then(/*Item info will be pulled from itemInfo$ input*/)
    .catch(error => this.handleError(error, this.snackBar));
  }
}
