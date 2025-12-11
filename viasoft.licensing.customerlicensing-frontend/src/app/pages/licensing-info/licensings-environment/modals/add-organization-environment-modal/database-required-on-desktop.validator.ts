import { FormGroup } from '@angular/forms';

export const DatabaseRequiredOnDesktopTypeValidator = (form: FormGroup) => {
  const isDesktop: boolean = form.get('isDesktop').value;
  if (isDesktop) {
    const databaseName: string = form.get('databaseName').value;
    if (!databaseName) {
      return {
        emptyDatabase: true
      };
    }
  }
  return null;
}
