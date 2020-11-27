import { Component, OnInit } from '@angular/core';
import { IStorageServer } from 'app/domain';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared';
import { finalize, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-servershealth',
  templateUrl: './servershealth.component.html',
  styleUrls: ['./servershealth.component.css']
})

export class ServershealthComponent extends ComponentBase implements OnInit {
  panelOpenState = false;
  serversHealth: IStorageServer[];
  constructor(private adminService: AdminService) {
    super();
  }

  ngOnInit(): void {
    this.adminService.notifyBusy(true);
    this.adminService.getStorageServers()
      .then(servers => this.serversHealth = servers)
      .finally(() => this.adminService.notifyBusy(false));
  }
  onServerStatus(eventOutput, server: IServerHealth) {
    server.status = eventOutput;
  }
}
export interface IServerHealth extends IStorageServer {
  status: string;
}
