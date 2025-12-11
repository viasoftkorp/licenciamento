import {Subject} from "rxjs";
import {Injectable} from "@angular/core";

@Injectable()

export class SharedService {

  private updatedGateway = new Subject<any>();
  private isDisabled = new Subject<boolean>();

  constructor() { }

  public seeIfDisabled = this.isDisabled.asObservable();
  public putGateway = this.updatedGateway.asObservable();

  public hideSaveButton(data: boolean) {
    this.isDisabled.next(data);
  }

  public updateGateway() {
    this.updatedGateway.next(true);
  }
}
