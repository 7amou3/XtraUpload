import { Component, OnInit } from '@angular/core';
import { ComponentBase } from 'app/shared';
import { StaticPageService, SeoService } from 'app/services';
import { Router, ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { IPage } from 'app/domain';

@Component({
  selector: 'app-page',
  templateUrl: './page.component.html'
})
export class PageComponent extends ComponentBase implements OnInit {
  page: IPage;
  constructor(
    private seoService: SeoService,
    private route: ActivatedRoute,
    private router: Router,
    private pageService: StaticPageService) {
    super();
  }

  ngOnInit(): void {
    this.isBusy = true;
    this.route.paramMap
    .pipe(takeUntil(this.onDestroy))
    .subscribe(async r => {
       await this.pageService.getPage(r.get('url'))
       .then(page => {
        this.seoService.setPageTitle(page.name);
        this.page = page;
       })
       .catch(() => this.router.navigate(['/404']))
       .finally(() => this.isBusy = false);
    });
  }

}
