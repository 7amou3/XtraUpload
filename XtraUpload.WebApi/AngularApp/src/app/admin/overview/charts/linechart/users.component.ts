import { Component, OnInit, Input } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ChartBase } from './chart.base';
import { FormBuilder } from '@angular/forms';
import { AdminService } from 'app/services';
import { IItemCount } from 'app/domain';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-users',
  templateUrl: './line.chart.component.html',
  styleUrls: ['./line.chart.component.css']
})
export class UsersComponent extends ChartBase implements OnInit {
  @Input() usersCount$ = new Subject<IItemCount[]>();
  constructor(
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    private adminService: AdminService) {
    super();
  }

  ngOnInit(): void {
    this.lineChartData[0].label = $localize`Users`;
    this.chartTitle = $localize`Users per day`;
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

  private async queryUserStats() {
    this.isBusy = true;
    await this.adminService.userStats({ start: this.start.value, end: this.end.value })
      .then((data) => this.populateChart(data))
      .catch((error) => this.handleError(error, this.snackBar))
      .finally(() => this.isBusy = false);
  }

}
