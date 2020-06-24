import { Component, OnInit } from '@angular/core';
import { ComponentBase } from 'app/shared';
import { StaticPageService } from 'app/services';
import { Router, ActivatedRoute } from '@angular/router';
import { takeUntil, finalize } from 'rxjs/operators';
import { IPage } from 'app/domain';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-page',
  templateUrl: './page.component.html',
  styleUrls: ['./page.component.css']
})
export class PageComponent extends ComponentBase implements OnInit {
  page: IPage;
  constructor(
    private titleService: Title,
    private route: ActivatedRoute,
    private router: Router,
    private pageService: StaticPageService) {
    super();
  }

  ngOnInit(): void {
    this.isBusy = true;
    this.route.paramMap
    .pipe(takeUntil(this.onDestroy))
    .subscribe(r => {
       this.pageService.getPage(r.get('name'))
       .pipe(
         takeUntil(this.onDestroy),
         finalize(() => this.isBusy = false))
       .subscribe(page => {
         this.titleService.setTitle(page.name);
         this.page = page;
       },
       () => {
        this.router.navigate(['/']);
       });
    });
  }

}
