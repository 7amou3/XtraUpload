import { Component, OnInit, Input } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AdminService } from 'app/services';
import { takeUntil } from 'rxjs/operators';
import { IItemCount } from 'app/models';
import { FormBuilder } from '@angular/forms';
import { Subject } from 'rxjs';
import { ChartBase } from './chart.base';

@Component({
  selector: 'app-fileuploads',
  templateUrl: './line.chart.component.html',
  styleUrls: ['./line.chart.component.css']
})
export class FileuploadsComponent extends ChartBase implements OnInit {
  @Input() filesCount$ = new Subject<IItemCount[]>();
  constructor(
    private snackBar: MatSnackBar,
    private fb: FormBuilder,
    private adminService: AdminService) {
    super();
  }

  ngOnInit(): void {
    this.lineChartData[0].label = $localize`Uploads`;
    this.chartTitle = $localize`File uploads per day`;
    this.itemsSearchFormGroup = this.fb.group({
      start: this.start,
      end: this.end
    });

    this.filesCount$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(
        data => {
          this.populateChart(data);
        }
      );
  }
  onSearchItemsSubmit() {
    this.queryUploadStats();
  }

  private async queryUploadStats() {
    this.isBusy = true;
    await this.adminService.uploadStats({ start: this.start.value, end: this.end.value })
      .then((data) => this.populateChart(data))
      .catch((error) => this.handleError(error, this.snackBar))
      .finally(() => this.isBusy = false);
  }
}
