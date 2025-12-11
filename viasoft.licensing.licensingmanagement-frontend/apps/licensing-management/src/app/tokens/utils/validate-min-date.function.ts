import { AbstractControl } from "@angular/forms";

export function validateMinDate(control: AbstractControl): {[key: string]: any} | null  {
    let currentDate = new Date();
    currentDate.setHours(0, 0, 0, 0)
    if (control.value && control.value < currentDate) {
      return { 'invalidDate': true };
    }
    return null;
}
