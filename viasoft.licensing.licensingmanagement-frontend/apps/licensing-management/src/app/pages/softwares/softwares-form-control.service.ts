import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class SoftwaresFormControlService {

    public softwaresSubject: Subject<void> = new Subject<void>();
    public softwareFormSubject: Subject<void> = new Subject<void>();
    public saveSoftwaresSubject: Subject<void> = new Subject<void>();
    public softwareInvalidSubject: Subject<void> = new Subject<void>();
    public gridRefreshSubject: Subject<void> = new Subject<void>();

    openSoftwares() {
        return this.softwaresSubject.next();
    }

    softwareFormIsValid() {
        return this.softwareFormSubject.next();
    }

    softwareFormIsInvalid() {
        return this.softwareInvalidSubject.next();
    }

    saveSoftwares() {
        return this.saveSoftwaresSubject.next();
    }

    refreshGrid() {
        return this.gridRefreshSubject.next();
    }

}

