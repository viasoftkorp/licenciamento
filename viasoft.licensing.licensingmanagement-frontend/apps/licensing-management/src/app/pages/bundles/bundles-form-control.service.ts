import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class BundlesFormControlService {
    public bundlesSubject: Subject<void> = new Subject<void>();
    public bundleFormSubject: Subject<boolean> = new Subject();
    public saveBundlesSubject: Subject<void> = new Subject<void>();


    openSoftwares() {
        return this.bundlesSubject.next();
    }

    bundleFormIsValid() {
        return this.bundleFormSubject.next(true);
    }

    bundleFormIsInvalid() {
        return this.bundleFormSubject.next(false);
    }

    saveBundles() {
        return this.saveBundlesSubject.next();
    }

}
