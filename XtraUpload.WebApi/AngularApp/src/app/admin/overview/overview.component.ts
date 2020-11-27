import { Component, Inject, OnInit } from '@angular/core';
import { ComponentBase } from 'app/shared';
import { AdminService } from 'app/services';
import { IAdminOverView, IItemCount, IFileTypeCount } from 'app/domain';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.css']
})
export class OverviewComponent extends ComponentBase implements OnInit {
  adminOverview: IAdminOverView;
  serverUrl: string;
  filesCount$ = new Subject<IItemCount[]>();
  usersCount$ = new Subject<IItemCount[]>();
  fileTypesCount$ = new Subject<IFileTypeCount[]>();
  constructor(
    private adminService: AdminService,
    @Inject('BASE_URL') baseUrl: string) {
    super();
    this.serverUrl = baseUrl;
  }

  ngOnInit(): void {
    this.adminService.notifyBusy(true);
    this.adminService.Overview({ start: this.subtractDate(14), end: new Date() })
      .then((data) => {
        this.adminOverview = data;
        this.filesCount$.next(data.filesCount);
        this.usersCount$.next(data.usersCount);
        this.fileTypesCount$.next(data.fileTypesCount);
      })
      .catch((error) => this.handleError(error))
      .finally(() => this.adminService.notifyBusy(false));
  }

}
