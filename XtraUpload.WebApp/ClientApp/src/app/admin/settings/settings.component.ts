import { Component, OnInit } from '@angular/core';
import { FormControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ComponentBase } from 'app/shared';
import { AdminService } from 'app/services';
import { takeUntil, finalize } from 'rxjs/operators';
import { IEmailSettings } from 'app/domain';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent extends ComponentBase implements OnInit {
  jwtFormGroup: FormGroup;
  hideSecretKey = true;
  secretKey = new FormControl('', [Validators.required, Validators.maxLength(32) ]);
  validFor = new FormControl('', [Validators.required, Validators.min(1)]);
  issuer = new FormControl('', [Validators.required]);
  audience = new FormControl('', [Validators.required]);

  uploadFormGroup: FormGroup;
  uploadPath = new FormControl('', [Validators.required]);
  chunkSize = new FormControl('', [Validators.required, Validators.min(1)]);
  expiration = new FormControl('', [Validators.required, Validators.min(1)]);
  emailFormGroup: FormGroup;
  server = new FormControl('', [Validators.required]);
  port = new FormControl('', [Validators.required, Validators.min(1)]);
  username = new FormControl('', [Validators.required]);
  password = new FormControl('', [Validators.required]);
  senderName = new FormControl('', [Validators.required, Validators.minLength(4)]);
  adminEmail = new FormControl('', [Validators.required, Validators.email]);
  supportEmail = new FormControl('', [Validators.required, Validators.email]);
  hidepassword = true;
  hdOptsFormGroup: FormGroup;
  memoryThreshold = new FormControl('', [Validators.required, Validators.min(1)]);
  storageThreshold = new FormControl('', [Validators.required, Validators.min(1)]);
  pageSettingFormGroup: FormGroup;
  title = new FormControl('', [Validators.required, Validators.min(3)]);
  description = new FormControl('', [Validators.required, Validators.min(3)]);
  keywords = new FormControl('', [Validators.required, Validators.min(3)]);
  socialAuthFormGroup: FormGroup;
  googleClientId =  new FormControl('', [Validators.required, Validators.min(3)]);
  facebookAppId = new FormControl('', [Validators.required, Validators.min(3)]);
  jwtBusy = false;
  hdBusy = false;
  emailBusy = false;
  uploadBusy = false;
  appSettingBusy = false;
  socialAuthBusy = false;
  constructor(
    private fb: FormBuilder,
    private adminService: AdminService,
    private snackBar: MatSnackBar
  ) {
    super();
  }

  ngOnInit(): void {
    this.jwtFormGroup = this.fb.group({
      audience: this.audience,
      issuer: this.issuer,
      validFor: this.validFor,
      secretKey: this.secretKey
    });
    this.uploadFormGroup = this.fb.group({
      uploadPath: this.uploadPath,
      chunkSize: this.chunkSize,
      expiration: this.expiration
    });
    this.emailFormGroup = this.fb.group({
      server: this.server,
      port: this.port,
      username: this.username,
      password: this.password,
      senderName: this.senderName,
      adminEmail: this.adminEmail,
      supportEmail: this.supportEmail
    });
    this.hdOptsFormGroup = this.fb.group({
      memoryThreshold: this.memoryThreshold,
      storageThreshold: this.storageThreshold
    });
    this.pageSettingFormGroup = this.fb.group({
      title: this.title,
      description: this.description,
      keywords: this.keywords
    });
    this.socialAuthFormGroup = this.fb.group({
      facebookAppId: this.facebookAppId,
      googleClientId: this.googleClientId
    });
    this.adminService.notifyBusy(true);
    this.adminService.getSettings()
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.adminService.notifyBusy(false)))
    .subscribe((data: any) => {
      // page settings
      this.title.setValue(data.appSettings.title);
      this.description.setValue(data.appSettings.description);
      this.keywords.setValue(data.appSettings.keywords);
      // email settings
      this.server.setValue(data.emailSettings.smtp.server);
      this.port.setValue(data.emailSettings.smtp.port);
      this.username.setValue(data.emailSettings.smtp.username);
      this.password.setValue(data.emailSettings.smtp.password);
      this.senderName.setValue(data.emailSettings.sender.name);
      this.adminEmail.setValue(data.emailSettings.sender.admin);
      this.supportEmail.setValue(data.emailSettings.sender.support);
      // upload settings
      this.uploadPath.setValue(data.uploadOptions.uploadPath);
      this.chunkSize.setValue(data.uploadOptions.chunkSize);
      this.expiration.setValue(data.uploadOptions.expiration);
      // hardware settings
      this.memoryThreshold.setValue(data.hardwareCheckOptions.memoryThreshold);
      this.storageThreshold.setValue(data.hardwareCheckOptions.storageThreshold);
      // jwt settings
      this.audience.setValue(data.jwtIssuerOptions.audience);
      this.issuer.setValue(data.jwtIssuerOptions.issuer);
      this.secretKey.setValue(data.jwtIssuerOptions.secretKey);
      this.validFor.setValue(data.jwtIssuerOptions.validFor);
      // Social Auth settings
      this.facebookAppId.setValue(data.socialAuthSettings.facebookAuth.appId);
      this.googleClientId.setValue(data.socialAuthSettings.googleAuth.clientId);
    });
  }
  onJwtSubmit(jwtParams) {
    this.jwtBusy = true;
    this.adminService.updateJwtOpts(jwtParams)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.jwtBusy = false))
    .subscribe(
      () => {
        this.showSuccessMsg('Jwt Options');
      },
      error => {
        throw Error(error.error?.title);
      }
    );
  }
  onUploadSettingSubmit(uploadParams) {
    this.uploadBusy = true;
    this.adminService.updateUploadOpts(uploadParams)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.uploadBusy = false))
    .subscribe(
      () => {
        this.showSuccessMsg('Upload Options');
      },
      error => {
        throw Error(error.error?.title);
      }
    );
  }
  onEmailSubmit(emailParams: IEmailSettings) {
    this.emailBusy = true;
    this.adminService.updateEmailOpts(emailParams)
    .pipe(
      takeUntil(this.onDestroy),
      finalize(() => this.emailBusy = false))
    .subscribe(
      () => {
        this.showSuccessMsg('Email Options');
      },
      error => {
        throw Error(error.error?.title);
      }
    );
  }
  onHDOptionsSubmit(hardwareteParams) {
    this.hdBusy = true;
    this.adminService.updateHardwareOpts(hardwareteParams)
    .pipe(takeUntil(this.onDestroy),
          finalize(() => this.hdBusy = false))
    .subscribe(
      () => {
        this.showSuccessMsg('Hardware Options');
      },
      error => {
        throw Error(error.error?.title);
      }
    );
  }
  onPageSettingsSubmit(pageSettingParams) {
    this.appSettingBusy = true;
    this.adminService.updateAppSettings(pageSettingParams)
    .pipe(takeUntil(this.onDestroy),
      finalize(() => this.appSettingBusy = false))
    .subscribe(
      () => {
        this.showSuccessMsg('Page Settings');
      },
      error => {
        throw Error(error.error?.title);
      }
    );
  }
  onSocialAuthSubmit(socialAuthParams) {
    this.socialAuthBusy = true;
    this.adminService.updateSocialAuthSettings(socialAuthParams)
    .pipe(takeUntil(this.onDestroy),
      finalize(() => this.socialAuthBusy = false))
    .subscribe(
      () => {
        this.showSuccessMsg('Social Auth');
      },
      error => {
        throw Error(error.error?.title);
      }
    );
  }
  showSuccessMsg(section: string) {
    this.snackBar.open(`${section} has been updated successfully`, '', { duration: 3000 });
  }
}
