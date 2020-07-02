import { Component, OnInit, Inject } from '@angular/core';
import { ICheckResource, IHealthCheck } from 'app/domain';
import { ComponentBase } from 'app/shared';
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
  healthCheck: IHealthCheck;
  healthEndpoint: string;
  dataSource = new MatTableDataSource<ICheckResource>();
  isMobile: boolean;
  constructor(
    private adminService: AdminService,
    breakpointObserver: BreakpointObserver,
    @Inject('BASE_URL') baseUrl: string
  ) {
    super();
    breakpointObserver.observe(['(max-width: 600px)']).pipe(takeUntil(this.onDestroy)).subscribe(result => {
      this.isMobile =  result.matches;
      this.displayedColumns = result.matches
              ? ['component', 'status']
              : ['component', 'status', 'description'];
    });
    this.healthEndpoint = baseUrl + 'health';
  }
  displayedColumns: string[] = ['component', 'status', 'description'];

  ngOnInit(): void {
    this.adminService.healthCheck()
      .pipe(takeUntil(this.onDestroy))
      .subscribe(
        data => {
          this.populateTable(data);
        },
        error => {
          // Unhealthy resources return a 50x http status code
          const health = error.error as IHealthCheck;
          if (health.status != null) {
            this.populateTable(health);
          }
        }
      );
  }
  private populateTable(data: IHealthCheck) {
    this.dataSource.data = [...data.checks, { component: 'Overall status', status: data.status, description: 'run for: ' + data.duration }];
  }
}
