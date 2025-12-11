import { TreeTableDataFront } from './tree-table-front-data.interface';

export interface TreeTableNode {
    data: TreeTableDataFront;
    leaf: boolean;
    children: Array<TreeTableNode>;
    expanded: boolean;
}
