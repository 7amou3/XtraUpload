import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { takeUntil, take, finalize } from 'rxjs/operators';
import { ComponentBase } from 'app/shared';
import { FileManagerService, ProgressNotificationService, IProgressInfo } from 'app/services';
import { IFileInfo } from 'app/domain';
import { interval, Subscription } from 'rxjs';
import { timeInterval} from 'rxjs/operators';
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
    private titleService: Title,
    private route: ActivatedRoute,
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
      params => {
        const fileId = params.get('id');
        if (!fileId) {
          this.router.navigate(['/']);
        }
        else {
          this.fileMngService.getFile(fileId)
          .pipe(
            takeUntil(this.onDestroy),
            finalize(() => this.isBusy = false))
          .subscribe(
            file => {
              this.fileItem = file;
              this.titleService.setTitle('Download ' + file.name);
              this.startCountDownTimer(file.waitTime);
            },
            (err) => {
              if (err.error?.errorContent?.message) {
                this.message$.next({errorMessage: err.error.errorContent.message});
              }
              throw err;
            }
          );
        }
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
            this.progressPercent = (( (waitTime - timer.value) / waitTime) * 100 );
            this.timerInProgress = true;
          }
        },
        err => console.log(err),
    );
  }
  onDownloadRequested() {
    this.requestInProgress = true;
    if (!this.timerInProgress) {
      this.fileMngService.generateDownloadLink(this.fileItem.id)
      .pipe(
        takeUntil(this.onDestroy),
        finalize(() => this.requestInProgress = false))
      .subscribe(
        data => {
          this.downloadurl = data.downloadurl;
        },
        (error) => {
          if (error.error?.errorContent?.message) {
            this.message$.next({errorMessage: error.error.errorContent.message});
          }
          throw error;
        }
      );
    }
  }
  startDownload() {
    this.fileMngService.startDownload(this.downloadurl)
    .pipe(takeUntil(this.onDestroy))
    .subscribe(response => {
      const blob = new Blob([response], {type: this.fileItem.mimeType});
      FileSaver.saveAs(blob, this.fileItem.name);
    });
  }
}
