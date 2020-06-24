import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

/** Notifies the progress of an http request/response */
export class ProgressNotificationService {

  private $progressChanged = new Subject<IProgressInfo>();
  constructor() { }
  getProgress$(): Observable<IProgressInfo> {
    return this.$progressChanged.asObservable();
  }

  setProgress(progress: IProgressInfo): void {
    this.$progressChanged.next(progress);
  }
}

export interface IProgressInfo {
  status: 'Started' | 'InProgress' | 'Error' | 'Completed';
  currentProgress: number;
}
