import { Component } from '@angular/core';
import { Angulartics2GoogleGlobalSiteTag } from 'angulartics2/gst';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  constructor(siteTag: Angulartics2GoogleGlobalSiteTag) {
    siteTag.startTracking();
  }
}
