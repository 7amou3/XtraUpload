import { Component, OnInit } from '@angular/core';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { NgxDropzoneChangeEvent } from 'ngx-dropzone';
import { RejectedFile } from 'ngx-dropzone/lib/ngx-dropzone.service';
import { ComponentBase } from 'app/shared';

@Component({
  selector: 'app-avatar',
  templateUrl: './avatar.component.html',
  styleUrls: ['./avatar.component.css']
})
export class AvatarComponent extends ComponentBase implements OnInit {
  selectedImg: File;
  croppedImage: any = '';
  constructor() {
    super();
   }

  ngOnInit(): void {
  }
  onSelect(event: NgxDropzoneChangeEvent): void {
    this.selectedImg = event.addedFiles[0];
  }
  imageCropped(event: ImageCroppedEvent) {
    this.croppedImage = event.base64;
  }
  imageLoaded() {
    // show cropper
  }
  cropperReady() {
    // cropper ready
  }
  loadImageFailed() {
    // show message
  }
  onDeleteImage() {
    this.selectedImg = null;
  }
}
