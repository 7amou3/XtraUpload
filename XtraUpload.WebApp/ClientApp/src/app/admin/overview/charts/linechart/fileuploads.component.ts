import { Component, OnInit, Input, AfterViewInit } from '@angular/core';
import { AdminService } from 'app/services';
import { takeUntil, finalize } from 'rxjs/operators';
import { IItemCount } from 'app/domain';
import { FormBuilder} from '@angular/forms';
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
    private fb: FormBuilder,
    private adminService: AdminService) {
    super();
   }

  ngOnInit(): void {
    this.lineChartData[0].label = 'Uploads';
    this.chartTitle = 'File uploads per day';
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

  private queryUploadStats() {
    this.isBusy = true;
    this.adminService.uploadStats({start: this.start.value, end: this.end.value})
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
