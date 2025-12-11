import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { TreeTableNode } from 'src/app/common/interfaces/tree-table-node.interface';
import { TreeTableNodeDictionary } from 'src/app/common/interfaces/tree-table-node-dictionary.interface';
import { TreeTableDataFront } from 'src/app/common/interfaces/tree-table-front-data.interface';
import {LicenseUsageInRealTimeTreeTableData, LicenseUsageInRealTimeTreeTableTypes} from 'src/client/customer-licensing';

@Injectable()
export class LicensingsTreeTableService {

  constructor() { }

  public arrayOfLicensedApps: Subject<Array<any>> = new Subject();


  nextArrayOfLicensedModules(array: Array<any>) {
    return this.arrayOfLicensedApps.next(array);
  }

  createNodesOfUsers(input: Array<LicenseUsageInRealTimeTreeTableData>) {
    const users: Array<TreeTableNode> = [];
    input.forEach(data => {
      if (data.typeDescription === LicenseUsageInRealTimeTreeTableTypes.User) {
        users.push(
          {
            leaf: true,
            data: {
              name: data.name,
              parent: data.parent,
              startTime: new Date(data.startTime).toLocaleString(),
              type: data.typeDescription
            } as TreeTableDataFront
          } as TreeTableNode
        );
      }
    });
    return users;
  }

  createDictionaryOfAppToListOfUsers(input: Array<TreeTableNode>) {
    const dictionary = {} as TreeTableNodeDictionary;
    input.forEach(user => {
      if (!(user.data.parent in dictionary)) {
        dictionary[user.data.parent] = [user];
      } else {
        dictionary[user.data.parent].push(user);
      }
    });
    return dictionary;
  }

  createNodesOfApps(input: Array<LicenseUsageInRealTimeTreeTableData>) {
    const usersApps: TreeTableNodeDictionary = this.createDictionaryOfAppToListOfUsers(
      this.createNodesOfUsers(input)
    );
    const apps: Array<TreeTableNode> = [];
    input.forEach(data => {
      if (data.typeDescription === LicenseUsageInRealTimeTreeTableTypes.LooseApp
        || data.typeDescription === LicenseUsageInRealTimeTreeTableTypes.BundledApp) {
        const isLeaf = !(data.description in usersApps);
        const userList = !isLeaf ? usersApps[data.description] : null;
        apps.push(
          {
            leaf: isLeaf,
            children: !isLeaf ? userList : null,
            data: {
              name: data.name,
              parent: data.parent,
              description: data.description,
              type: data.typeDescription,
              total: data.typeDescription == LicenseUsageInRealTimeTreeTableTypes.LooseApp ? data.total : null,
              additional: data.additional,
              consumed: data.consumed > 0 ? data.consumed : null
            } as TreeTableDataFront
          } as TreeTableNode
        );
      }
    });
    return apps;
  }

  createDictionaryOfBundleToListOfApps(input: Array<TreeTableNode>) {
    const dictionary = {} as TreeTableNodeDictionary;
    input.forEach(app => {
      if (app.data.type == LicenseUsageInRealTimeTreeTableTypes.BundledApp && !(app.data.parent in dictionary)) {
        dictionary[app.data.parent] = [app];
      } else if (app.data.type == LicenseUsageInRealTimeTreeTableTypes.BundledApp) {
        dictionary[app.data.parent].push(app);
      }
    });
    return dictionary;
  }

  createNodesOfBundles(input: Array<LicenseUsageInRealTimeTreeTableData>) {
    const apps: TreeTableNodeDictionary = this.createDictionaryOfBundleToListOfApps(
      this.createNodesOfApps(input)
    );
    const bundles: Array<TreeTableNode> = [];
    input.forEach(data => {
      if (data.typeDescription == LicenseUsageInRealTimeTreeTableTypes.Bundle) {
        const isLeaf = !(data.description in apps);
        bundles.push(
          {
            leaf: isLeaf,
            children: !isLeaf ? apps[data.description] : null,
            data: {
              name: data.name,
              type: data.typeDescription,
              description: data.description,
              total: data.total,
              additional: data.additional,
              consumed: data.consumed > 0 ? data.consumed : null
            } as TreeTableDataFront
          } as TreeTableNode
        );
      }
    });
    return bundles;
  }

  createDataArray(input: Array<LicenseUsageInRealTimeTreeTableData>) {
    const output = this.createNodesOfBundles(input);
    this.createNodesOfApps(input).forEach(app => {
      if (app.data.type == LicenseUsageInRealTimeTreeTableTypes.LooseApp) {
        output.push(app);
      }
    });
    return output;
  }

}
