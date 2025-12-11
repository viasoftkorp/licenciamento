import { Subject } from 'rxjs';
import { VsGridGetInput } from '@viasoft/components';

export interface RequestFromCheckBoxGrid {
    selectedIds: Array<string> | null;
    unSelectedIds: Array<string> | null;
    allSelected: boolean;
    hasItemSelected: boolean;
    currentGet: VsGridGetInput;
}

export abstract class GridCheckBoxValidation {
    totalCount: number;
    currentGet: VsGridGetInput;
    selectedIds = [];
    unSelectedIds = [];
    ghostSelectionMode = false;
    allOptionsSelected = false;
    hasSelectedItem = false;

    states: RequestFromCheckBoxGrid;
    gridCheckBoxRequest = new Subject<RequestFromCheckBoxGrid>();

    constructor() { }

    selected(id: string) {
        if (!this.ghostSelectionMode) {
            this.selectedIds.push(id);
            this.unSelectedIds.splice(this.unSelectedIds.findIndex(i => i === id), 1);
            this.hasSelectedItem = true;
        } else {
            this.unSelectedIds.splice(this.unSelectedIds.findIndex(i => i === id), 1);
        }
        this.emitCurrentState();
    }

    unSelected(id: string) {
        this.allOptionsSelected = false;
        if (this.selectedIds.length === 0) {
            this.ghostSelectionMode = true;
            this.unSelectedIds.push(id);
            if (this.totalCount === this.unSelectedIds.length) {
                this.unSelectedIds = [];
                this.ghostSelectionMode = false;
                this.hasSelectedItem = false;
            }
        } else {
            this.selectedIds.splice(this.selectedIds.findIndex(index => index === id), 1);
            if (this.selectedIds.length === 0) {
                this.hasSelectedItem = false;
            }
        }
        this.emitCurrentState();
    }

    allSelected(state: boolean, currentGet: VsGridGetInput) {
        if (!state) {
            this.ghostSelectionMode = false;
        }
        this.selectedIds = [];
        this.unSelectedIds = [];
        this.hasSelectedItem = state;
        this.allOptionsSelected = state;
        this.currentGet = currentGet;
        this.emitCurrentState();
    }

    emitCurrentState() {
        this.states = {
            selectedIds: this.selectedIds,
            unSelectedIds: this.unSelectedIds,
            allSelected: this.allOptionsSelected,
            hasItemSelected: this.hasSelectedItem,
            currentGet: this.currentGet
        };
        this.gridCheckBoxRequest.next(this.states);
    }

    getGridCheckBoxForResquest() {
        return this.gridCheckBoxRequest;
    }

    clearStates() {
        this.selectedIds = [];
        this.unSelectedIds = [];
        this.ghostSelectionMode = false;
        this.allOptionsSelected = false;
        this.hasSelectedItem = false;
    }
}