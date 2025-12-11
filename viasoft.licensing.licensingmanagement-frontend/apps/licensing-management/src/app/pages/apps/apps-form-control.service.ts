import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class AppsFormControlService {
    public appsSubject: Subject<void> = new Subject<void>();
    public saveAppsSubject: Subject<void> = new Subject<void>();
    public gridRefreshSubject: Subject<void> = new Subject<void>();


    openSoftwares() {
        return this.appsSubject.next();
    }

    saveApps() {
        return this.saveAppsSubject.next();
    }

    refreshGrid() {
        return this.gridRefreshSubject.next();
    }

}
