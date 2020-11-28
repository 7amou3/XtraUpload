import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ChartType, ChartOptions } from 'chart.js';
import { SingleDataSet, Label, monkeyPatchChartJsTooltip, monkeyPatchChartJsLegend, BaseChartDirective } from 'ng2-charts';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { ComponentBase } from 'app/shared';
import { AdminService } from 'app/services';
import { Subject } from 'rxjs';
import { IFileTypeCount } from 'app/domain';
import { takeUntil } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-piechart',
  templateUrl: './piechart.component.html',
  styleUrls: ['./piechart.component.css']
})
export class PiechartComponent extends ComponentBase implements OnInit {
  @Input() fileTypesCount$ = new Subject<IFileTypeCount[]>();
  filesSearchFormGroup: FormGroup;
  public start = new FormControl(this.subtractDate(14), [Validators.required]);
  public end = new FormControl(new Date(), [Validators.required]);

  public pieChartOptions: ChartOptions = {
    responsive: true,
  };
  public pieChartLabels: Label[] = ['Multimedia', 'Documents', 'Archives', 'Others'];
  public pieChartData: SingleDataSet = [50, 50, 50, 50];
  public pieChartType: ChartType = 'pie';
  public pieChartLegend = true;
  public pieChartPlugins = [];
  @ViewChild('baseChart') chart: BaseChartDirective;

  constructor(
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    private adminService: AdminService
  ) {
    super();
    monkeyPatchChartJsTooltip();
    monkeyPatchChartJsLegend();
  }

  ngOnInit() {
    this.filesSearchFormGroup = this.fb.group({
      start: this.start,
      end: this.end
    });

    this.fileTypesCount$
      .pipe(takeUntil(this.onDestroy))
      .subscribe(
        pieData => {
          this.populatePieChart(pieData);
        });
  }
  private populatePieChart(pieData: IFileTypeCount[]) {
    for (let i = 0; i < pieData.length; i++) {
      this.pieChartData[i] = pieData[i].itemCount;
      this.pieChartLabels[i] = this.convertToStr(pieData[i].fileType);
    }
    if (this.chart !== undefined) {
      this.chart.update();
    }
  }
  reloadChart() {
    if (this.chart !== undefined) {
      this.chart.chart.destroy();
      this.chart.data = this.pieChartData;
      this.chart.labels = this.pieChartLabels;
      this.chart.ngOnInit();
    }
  }
  rangeFilter(d: Date): boolean {
    return d.getTime() < new Date().getTime();
  }
  convertToStr(enumVal: number): string {
    switch (enumVal) {
      case 0: return 'Others';
      case 1: return 'Archives';
      case 2: return 'Documents';
      case 3: return 'Multimedia';
    }
  }
  async onSearchItemsSubmit() {
    this.isBusy = true;
    await this.adminService.filetypeStat({ start: this.start.value, end: this.end.value })
      .then((data) => this.populatePieChart(data))
      .catch((error) => this.handleError(error, this.snackBar))
      .finally(() => this.isBusy = false);
  }
}
