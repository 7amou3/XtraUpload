import { Pipe, PipeTransform } from '@angular/core';
import { formatDate } from '@angular/common';
import { UserStorageService } from 'app/services'

@Pipe({
  name: 'localDate'
})
export class LocalDatePipe implements PipeTransform {
  constructor(private storageService: UserStorageService) {}

  transform(value: any, format: string) {
    if (!value) {
      return '';
    }
    if (!format) {
      format = 'mediumDate';
    }

    return formatDate(value, format, this.storageService.userlanguage.culture);
  }
}