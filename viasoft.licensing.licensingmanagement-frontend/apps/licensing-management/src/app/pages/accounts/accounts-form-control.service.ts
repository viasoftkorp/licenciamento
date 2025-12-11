import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class AccountsFormControlService {

  constructor() { }

  refreshGridSubject: Subject<void> = new Subject<void>();

  refreshGrid() {
    this.refreshGridSubject.next();
  }
}
