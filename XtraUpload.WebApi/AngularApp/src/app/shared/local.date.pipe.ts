import { Pipe, PipeTransform } from '@angular/core';
import { formatDate } from '@angular/common';
import { LanguageService } from '../services/language.service'

@Pipe({
  name: 'localDate'
})
export class LocalDatePipe implements PipeTransform {
  constructor(private langService: LanguageService) {}

  transform(value: any, format: string) {
    if (!value) {
      return '';
    }
    if (!format) {
      format = 'mediumDate';
    }

    return formatDate(value, format, this.langService.locale);
  }
}