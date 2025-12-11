import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { FilterAppDetail } from '../../tokens/interfaces/filter-app-detail.interface';

@Injectable()
export class LicensingsFormControlServices {
    public licensingsSubject: Subject<void> = new Subject();
    public licensingsForm: Subject<boolean> = new Subject();
    public saveLicensingsSubject: Subject<void> = new Subject();
    public gridRefresherSubject: Subject<void> = new Subject();
    public filterAppDetail: Subject<FilterAppDetail> = new Subject();

    openSoftwares() {
        return this.licensingsSubject.next();
    }

    licensingsFormIsValid() {
        return this.licensingsForm.next(true);
    }

    licensingsFormIsInvalid() {
        return this.licensingsForm.next(false);
    }

    saveLicensings() {
        return this.saveLicensingsSubject.next();
    }

    refreshGrid() {
        return this.gridRefresherSubject.next();
    }

    nextFilterAppDetail(filterAppDetail: FilterAppDetail) {
        return this.filterAppDetail.next(filterAppDetail);
    }
}
