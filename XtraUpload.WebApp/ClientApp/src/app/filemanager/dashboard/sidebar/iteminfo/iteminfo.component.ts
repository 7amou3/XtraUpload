import { Component, OnInit, Input, Inject } from '@angular/core';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';
import { Subject, Observable } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { FileManagerService } from 'app/services';
import { IItemInfo, IFileInfo, IFolderInfo } from 'app/domain';
import { isFile } from '../../helpers';
import { ComponentBase } from 'app/shared';

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
  constructor(
    private fileMngService: FileManagerService,
    @Inject('API_URL') private apiUrl: string) {
    super();
  }

  ngOnInit(): void {
    this.itemInfo$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(
        item => {
          this.itemInfo = Object.assign(item);
          this.isFile = isFile(item);
          if (this.isFile) {
            if ((item as IFileInfo).mimeType.startsWith('image')) {
              item.thumbnail = this.apiUrl + 'file/mediumthumb/' + item.id;
            }
          } else {
            this.itemInfo.thumbnail = 'assets/images/folder128px.png';
          }
          this.simplePath = '';
          for (let i = 0; i < this.folderPath.length; i++) {
            this.simplePath += this.folderPath[i].name + (i !== (this.folderPath.length - 1) ? ' > ' : '');
          }
        }
      );
  }
  onAvailableItemChange(event: MatSlideToggleChange) {
    let serviceCall$: Observable<IItemInfo> = this.fileMngService.updateFolderAvailability({ itemId: this.itemInfo.id, available: event.checked });
    if (isFile(this.itemInfo)) {
      serviceCall$ = this.fileMngService.updateFileAvailability({ itemId: this.itemInfo.id, available: event.checked });
    }

    serviceCall$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(/*Item info will be pulled from itemInfo$ input*/);    
  }
}
