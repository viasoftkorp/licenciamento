import { FormGroup } from '@angular/forms';

export const DatabaseRequiredOnDesktopTypeValidator = (form: FormGroup) => {
  const isDesktop: boolean = form.get('isDesktop').value;
  if (isDesktop) {
    const databaseName: string = form.get('databaseName').value;
    if (!databaseName || databaseName.length === 0) {
      return {
        emptyDatabase: true
      };
    }
  }
  return null;
}
