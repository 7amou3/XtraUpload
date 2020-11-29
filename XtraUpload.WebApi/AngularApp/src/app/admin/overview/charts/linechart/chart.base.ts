import { ComponentBase } from 'app/shared';
import { ChartDataSets, ChartOptions } from 'chart.js';
import { Label, Color } from 'ng2-charts';
import { IItemCount } from 'app/domain';
import { FormControl, Validators, FormGroup } from '@angular/forms';

export abstract class ChartBase extends ComponentBase {
    public lineChartData: ChartDataSets[] = [{ data: [], label: 'N/A' }];
    public lineChartLabels: Label[] = [];
    public lineChartLegend = true;
    public lineChartType = 'line';
    public lineChartOptions: ChartOptions = {
        responsive: true,
        scales: {
            yAxes: [{
               ticks: {
                 min: 0
                }
            }]
          }
    };
    public lineChartColors: Color[] = [
        {
            borderColor: 'black',
            borderWidth: 1,
            backgroundColor: 'rgba(80, 226, 252,0.3)',
        },
    ];
    public chartTitle = 'N/A';
    //#region Chart search
    itemsSearchFormGroup: FormGroup;
    public start = new FormControl(this.subtractDate(14), [Validators.required]);
    public end = new FormControl(new Date(), [Validators.required]);
    //#endregion
    rangeFilter(d: Date): boolean {
		if (!d) return;
        return d.getTime() < new Date().getTime();
    }
    protected populateChart(data: IItemCount[]) {
        const yAxis = this.lineChartData[0].data as any;
        // do not update chart if we got the same data back from the server
        if (yAxis.length === data.length && yAxis.every((value: number, i: number) => value === data[i].itemCount)) {
            return;
        }
        // empty chart axis
        yAxis.splice(0, yAxis.length);
        this.lineChartLabels.splice(0, this.lineChartLabels.length);

        data.forEach(count => {
            yAxis.push(count.itemCount);
            this.lineChartLabels.push(new Date(count.date).toLocaleDateString());
        });
    }
}
