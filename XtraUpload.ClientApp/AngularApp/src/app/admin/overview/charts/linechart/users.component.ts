import { Component, OnInit, Input, AfterViewInit } from '@angular/core';
import { ChartBase } from './chart.base';
import { FormBuilder } from '@angular/forms';
import { AdminService } from 'app/services';
import { IItemCount } from 'app/domain';
import { Subject } from 'rxjs';
import { takeUntil, finalize } from 'rxjs/operators';

@Component({
  selector: 'app-users',
  templateUrl: './line.chart.component.html',
  styleUrls: ['./line.chart.component.css']
})
export class UsersComponent extends ChartBase implements OnInit {
  @Input() usersCount$ = new Subject<IItemCount[]>();
  constructor(
    private fb: FormBuilder,
    private adminService: AdminService) {
    super();
   }

  ngOnInit(): void {
    this.lineChartData[0].label = 'Users';
    this.chartTitle = 'Users per day';
    this.itemsSearchFormGroup = this.fb.group({
      start: this.start,
      end: this.end
    });

    this.usersCount$
    .pipe(takeUntil(this.onDestroy))
      .subscribe(
        data => {
          this.populateChart(data);
        }
      );
  }
  onSearchItemsSubmit() {
    this.queryUserStats();
  }

  private queryUserStats() {
    this.isBusy = true;
    this.adminService.userStats({start: this.start.value, end: this.end.value})
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.isBusy = false))
    .subscribe(
      (data) => {
        this.populateChart(data);
      },
      (error) => this.handleError(error)
    );
  }

}
