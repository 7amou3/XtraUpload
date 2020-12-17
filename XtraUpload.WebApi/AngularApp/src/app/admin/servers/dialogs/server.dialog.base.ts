
import { Directive, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { serverState } from 'app/models';
import { AdminService } from 'app/services';
import { ComponentBase } from 'app/shared/components';
import { takeUntil } from 'rxjs/operators';

@Directive()
export abstract class ServerDialogBase extends ComponentBase implements OnInit {
    dialogTitle: string;
    serverInfoFormGroup: FormGroup;
    uploadOptsFormGroup: FormGroup;
    hdOptsFormGroup: FormGroup;
    checkingConnectivity = false;
    addressReachable = false;
    /** must be https, allowed ip or host name */
    address = new FormControl('https://', [Validators.required, /*Validators.pattern(/^https:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&\=]*)/)*/]);
    optionControl = new FormControl({ value: '', disabled: true }, Validators.required);
    uploadPath = new FormControl({ value: '', disabled: true }, [Validators.required]);
    chunkSize = new FormControl({ value: '', disabled: true }, [Validators.required, Validators.min(1)]);
    expiration = new FormControl({ value: '', disabled: true }, [Validators.required, Validators.min(1)]);
    memoryThreshold = new FormControl({ value: '', disabled: true }, [Validators.required, Validators.min(1)]);
    storageThreshold = new FormControl({ value: '', disabled: true }, [Validators.required, Validators.min(1)]);
    serverOptions: IServerOption[] = [
        { name: $localize`Active`, state: serverState.Active, hint: $localize`Uploads and downloads are enabled` },
        { name: $localize`Passive`, state: serverState.Passive, hint: $localize`Downloads are available, uploads are disabled` },
        { name: $localize`Disabled`, state: serverState.Disabled, hint: $localize`Server is reachable, but no uploads/downloads are allowed` }
    ]

    constructor(protected adminService: AdminService, protected fb: FormBuilder,) {
        super();

    }
    async onConnectivityCheck() {
        if (this.checkingConnectivity) return;
        this.checkingConnectivity = true;

        // Check connectivity
        await this.adminService.checkstorageconnectivity(this.address.value)
            .then(async (result: any) => {
                this.addressReachable = result?.state == 0;
                // Check request status
                if (result.state != 0) {
                    this.message$.next({ errorMessage: result?.error });
                    return;
                }
                this.address.setErrors(null);
                this.message$.next({ successMessage: $localize`The storage server is up and running` });

                // Retrieve storage server upload configuration
                this.adminService.getUploadConfigrConfig(this.address.value)
                    .then((uploadCfg: any) => {
                        // Check request status
                        if (uploadCfg.state != 0) {
                            this.message$.next({ errorMessage: uploadCfg?.error });
                            return;
                        }
                        this.switchControls(true);
                        this.uploadPath.setValue(uploadCfg.uploadOpts.uploadPath);
                        this.chunkSize.setValue(uploadCfg.uploadOpts.chunkSize);
                        this.expiration.setValue(uploadCfg.uploadOpts.expiration);
                    });
                // Retrieve storage server hardware configuration
                this.adminService.getHardwareConfig(this.address.value)
                    .then((harddwareCfg: any) => {
                        // Check request status
                        if (harddwareCfg.state != 0) {
                            this.message$.next({ errorMessage: harddwareCfg?.error });
                            return;
                        }
                        this.memoryThreshold.setValue(harddwareCfg.hardwareOptions.memoryThreshold);
                        this.storageThreshold.setValue(harddwareCfg.hardwareOptions.storageThreshold);
                    });
            })
            .finally(() => this.checkingConnectivity = false);

    }
    protected switchControls(enabled: boolean) {
        if (enabled) {
            this.uploadPath.enable();
            this.chunkSize.enable();
            this.expiration.enable();
            this.optionControl.enable();
            this.memoryThreshold.enable();
            this.storageThreshold.enable();
        }
        else {
            this.uploadPath.disable();
            this.chunkSize.disable();
            this.expiration.disable();
            this.optionControl.disable();
            this.memoryThreshold.disable();
            this.storageThreshold.disable();
        }
    }
    ngOnInit() {
        this.serverInfoFormGroup = this.fb.group({
            address: this.address,
            state: this.optionControl,
        });
        this.uploadOptsFormGroup = this.fb.group({
            uploadPath: this.uploadPath,
            chunkSize: this.chunkSize,
            expiration: this.expiration
        });
        this.hdOptsFormGroup = this.fb.group({
            memoryThreshold: this.memoryThreshold,
            storageThreshold: this.storageThreshold
        });
        this.address.valueChanges
            .pipe(takeUntil(this.onDestroy))
            .subscribe(() => {
                this.addressReachable = false;
                this.serverInfoFormGroup.setErrors({ 'checkAddress': true });
                this.switchControls(false);
            });
        this.Init();
    }
    /** Invoked when component is ready */
    protected abstract Init();

}

interface IServerOption {
    name: string;
    hint: string;
    state: serverState;
}