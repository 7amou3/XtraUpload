import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { takeUntil } from 'rxjs/operators';
import { ComponentBase } from 'app/shared';
import { FileManagerService, ProgressNotificationService, IProgressInfo, SeoService } from 'app/services';
import { IFileInfo } from 'app/domain';
import { interval, Subscription } from 'rxjs';
import { timeInterval } from 'rxjs/operators';
import * as FileSaver from 'file-saver';

@Component({
  selector: 'app-file',
  templateUrl: './file.component.html',
  styleUrls: ['./file.component.css']
})
export class FileComponent extends ComponentBase implements OnInit {
  fileItem: IFileInfo;
  timerInProgress = true;
  timeToWait: string;
  progressPercent: number;
  requestInProgress = false;
  downloadurl: string;
  downloadProgress: IProgressInfo;
  private downloadTimer$: Subscription;
  private downloadTimeCounter = 0;
  downloadSpeed = 0;
  downloadState: 'unknwown' | 'started' | 'ended';
  constructor(
    private seoService: SeoService,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private router: Router,
    private fileMngService: FileManagerService,
    private progressService: ProgressNotificationService
  ) {
    super();
  }

  ngOnInit(): void {
    this.isBusy = true;
    this.route.queryParamMap
      .pipe(takeUntil(this.onDestroy))
      .subscribe(
        async params => {
          const fileId = params.get('id');
          if (!fileId) {
            this.router.navigate(['/404']);
          }
          // Get file info from the server
          await this.fileMngService.getFile(fileId)
            .then(file => {
              this.fileItem = file;
              this.seoService.setPageTitle($localize`Download` + ' ' + file.name);
              this.startCountDownTimer(file.waitTime);
            })
            .catch((err) => {
              if (err.errors) {
                this.message$.next({ errorMessage: err.error });
              }
              throw err;
            })
            .finally(() => this.isBusy = false);
        }
      );
    this.progressService.getProgress$()
      .pipe(takeUntil(this.onDestroy))
      .subscribe(
        data => {
          if (data.status === 'Started') {
            this.downloadState = 'started';
            this.startDownloadCounter();
          }
          if (data.status === 'Completed' || data.status === 'Error') {
            this.downloadState = 'ended';
            this.downloadTimer$?.unsubscribe();
            this.downloadTimer$ = null;
            this.downloadTimeCounter = 0;
            this.downloadSpeed = 0;
          }
          this.downloadSpeed = ((data.currentProgress * this.fileItem.size) / 100) / this.downloadTimeCounter;
          this.downloadProgress = data;
        },
        (err) => {
          this.downloadState = 'unknwown';
          this.downloadTimeCounter = 0;
          this.downloadSpeed = 0;
        }
      );
  }
  startDownloadCounter() {
    const seconds = interval(1000);
    this.downloadTimer$ = seconds.pipe(timeInterval())
      .pipe(takeUntil(this.onDestroy))
      .subscribe(
        timer => {
          this.downloadTimeCounter++;
        }
      );
  }
  /** Start the countdown timer for download link */
  startCountDownTimer(waitTime: number) {
    const seconds = interval(1000);
    const timer$ = seconds.pipe(timeInterval())
      .pipe(takeUntil(this.onDestroy))
      .subscribe(
        timer => {
          if (timer.value === waitTime) {
            timer$.unsubscribe();
            this.timerInProgress = false;
          }
          else {
            this.timeToWait = (waitTime - timer.value).toFixed();
            this.progressPercent = (((waitTime - timer.value) / waitTime) * 100);
            this.timerInProgress = true;
          }
        },
        err => console.log(err),
      );
  }
  async onDownloadRequested() {
    this.requestInProgress = true;
    if (!this.timerInProgress) {
      await this.fileMngService.generateDownloadLink(this.fileItem.id)
        .then(data => {
          this.downloadurl = data.downloadurl;
        })
        .catch((error) => {
          if (error.error) {
            this.message$.next({ errorMessage: error.error });
          }
          else throw error;
        })
        .finally(() => this.requestInProgress = false);
    }
  }
  async startDownload() {
    await this.fileMngService.startDownload(this.downloadurl)
      .then(response => {
        const blob = new Blob([response], { type: this.fileItem.mimeType });
        FileSaver.saveAs(blob, this.fileItem.name);
      })
      .catch(error => this.handleError(error, this.snackBar));
  }
}
