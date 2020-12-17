import { Component, OnInit } from '@angular/core';
import { ComponentBase } from 'app/shared/components';
import { Observable } from 'rxjs';
import { startWith } from 'rxjs/operators';
import { ProgressNotificationService, IProgressInfo } from '../../services/progress.notification.service';

@Component({
  selector: 'app-http-progress',
  templateUrl: './progress.component.html'
})
export class ProgressComponent extends ComponentBase implements OnInit {
  httpProgress$: Observable<IProgressInfo>;

  constructor(private progressNotif: ProgressNotificationService) {
    super();
  }

  ngOnInit() {
    this.httpProgress$ = this.progressNotif.getProgress$()
    .pipe(
      startWith({ status: 'Completed', currentProgress: 0 })
      );
  }
}
