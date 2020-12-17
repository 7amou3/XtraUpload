import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ICheckResource, IHealthCheck } from 'app/models';
import { ComponentBase } from 'app/shared/components';
import { AdminService } from 'app/services';
import { takeUntil } from 'rxjs/operators';
import { MatTableDataSource } from '@angular/material/table';
import { BreakpointObserver } from '@angular/cdk/layout';

@Component({
  selector: 'app-healthchek',
  templateUrl: './healthchek.component.html',
  styleUrls: ['./healthchek.component.css']
})
export class HealthchekComponent extends ComponentBase implements OnInit {
  @Input() serverUrl: string;
  @Output() serverStatus = new EventEmitter<string>();
  healthCheck: IHealthCheck;
  healthEndpoint: string;
  dataSource = new MatTableDataSource<ICheckResource>();
  isMobile: boolean;
  constructor(
    private adminService: AdminService,
    breakpointObserver: BreakpointObserver
  ) {
    super();
    breakpointObserver.observe(['(max-width: 600px)']).pipe(takeUntil(this.onDestroy)).subscribe(result => {
      this.isMobile = result.matches;
      this.displayedColumns = result.matches
        ? ['component', 'status']
        : ['component', 'status', 'description'];
    });
  }
  displayedColumns: string[] = ['component', 'status', 'description'];

  ngOnInit(): void {
    this.serverUrl = this.serverUrl.replace(/\/?$/, '/');
    this.healthEndpoint = this.serverUrl + 'health';
    this.adminService.healthCheck(this.healthEndpoint)
      .then(data => {
        this.populateTable(data);
        this.serverStatus.emit(data.status);
      })
      .catch(error => {
        // Unhealthy resources return a 50x http status code
        const health = error.error as IHealthCheck;
        if (health.status != null) {
          this.populateTable(health);
        }
        this.serverStatus.emit('Unhealthy');
      });
  }
  private populateTable(data: IHealthCheck) {
    this.dataSource.data = [...data.checks, { component: 'Overall status', status: data.status, description: 'run for: ' + data.duration }];
  }
}
