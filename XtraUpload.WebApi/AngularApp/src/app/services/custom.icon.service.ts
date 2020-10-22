import { DomSanitizer } from "@angular/platform-browser";
import { MatIconRegistry } from "@angular/material/icon";
import { Injectable, Inject } from "@angular/core";

@Injectable()
export class CustomIconService {
  constructor(
    @Inject('BASE_URL') private baseUrl: string,
    private matIconRegistry: MatIconRegistry,
    private domSanitizer: DomSanitizer
  ) {}
  init() {
    this.matIconRegistry.addSvgIcon(
      "xu-folder",
      this.domSanitizer.bypassSecurityTrustResourceUrl(this.baseUrl + "/assets/images/svg/xu-folder.svg")
    )
    .addSvgIcon(
      "facebook",
      this.domSanitizer.bypassSecurityTrustResourceUrl(this.baseUrl + "/assets/images/svg/facebook.svg")
    )
    .addSvgIcon(
      "google",
      this.domSanitizer.bypassSecurityTrustResourceUrl(this.baseUrl + "/assets/images/svg/google.svg")
    )
    .addSvgIcon(
      "xu-servers",
      this.domSanitizer.bypassSecurityTrustResourceUrl(this.baseUrl + "/assets/images/svg/xu-servers.svg")
    );
  }
}
