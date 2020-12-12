import { OnDestroy, Directive } from '@angular/core';
import { FormControl, FormGroup, } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { IGenericMessage } from 'app/domain';

@Directive()
export abstract class ComponentBase implements OnDestroy {
  /** Indicate wether the component is busy (sending data over the wire, heavy computation...) */
  isBusy = false;
  /** Used to display a success/error message to the user about the current operation result */
  message$ = new Subject<IGenericMessage>();
  /** Should be called when subscribing to an observable */
  protected readonly onDestroy = new Subject<void>();

  /** validate a view input  */
  getErrorMessage(fc: FormControl) {
    return fc.hasError('required') ? $localize`You must enter a value` :
      fc.hasError('minlength') ? $localize`Must be at least ${fc.errors.minlength.requiredLength} characters long` :
        '';
  }
  /** Reset a form to it's initial state */
  resetForm(formGroup: FormGroup) {
    formGroup.reset();
    Object.keys(formGroup.controls).forEach(
       field => {
          formGroup.get(field).setErrors(null);
       }
     );
 }
 subtractDate(day: number): Date {
   const currentDate = new Date();
   const subtracted =  currentDate.getTime()  - (day * 8.64e+7);
   return new Date(subtracted);
 }
 handleError(error, snackBar: MatSnackBar) {
  if (error) {
    snackBar.open('Error 400: ' + error);
  } else {
    snackBar.open(error);
  }
 }
  ngOnDestroy() {
    this.onDestroy.next();
    this.onDestroy.complete();
  }
}
