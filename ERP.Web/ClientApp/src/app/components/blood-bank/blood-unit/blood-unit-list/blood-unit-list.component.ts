import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ComponentTypeService } from '../../component-type/component-type.service';
import { AddBloodUnitComponent } from '../add-blood-unit/add-blood-unit.component';
import { DeleteBloodUnitComponent } from '../delete-blood-unit/delete-blood-unit.component';
import { BloodUnitService } from '../blood-unit.service';

@Component({
    selector: 'app-blood-unit-list',
    templateUrl: './blood-unit-list.component.html',
    styleUrls: ['./blood-unit-list.component.css'],
    standalone: false
})
export class BloodUnitListComponent {
    filterForm!: FormGroup;
    isLoading = false;
    currentPage = 0;
    pageSizeOptions: number[] = [5, 10, 25, 100];
    displayedColumns: string[] = ['unitNo', 'componentType', 'bloodGroup', 'storage', 'status', 'expiry', 'actions'];
    dataSource: any;
    take = 10;
    totalRows = 0;
    componentTypeList: any[] = [];
    statusMap: { [key: number]: string } = {
        1: 'Available',
        2: 'Reserved',
        3: 'Issued',
        4: 'Discarded',
        5: 'Expired'
    };
    statusFilterOptions = [
        { value: 0, name: 'All' },
        { value: 1, name: 'Available' },
        { value: 2, name: 'Reserved' },
        { value: 3, name: 'Issued' },
        { value: 4, name: 'Discarded' },
        { value: 5, name: 'Expired' }
    ];
    storageFilterOptions = [
        { value: 0, name: 'All' },
        { value: 1, name: 'Assigned' },
        { value: 2, name: 'Not Assigned' }
    ];

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    constructor(
        private service: BloodUnitService,
        private componentTypeService: ComponentTypeService,
        private dialog: MatDialog,
        private formBuilder: FormBuilder
    ) { }

    ngOnInit(): void {
        this.filterForm = this.formBuilder.group({
            unitNo: [''],
            componentTypeName: [''],
            status: [0],
            storageAssigned: [0]
        });
        this.loadComponentTypes();
        this.bindData();
    }

    loadComponentTypes() {
        this.componentTypeService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.componentTypeList = data.item1 || [];
        });
    }

    bindData(): void {
        this.isLoading = true;
        const request = {
            ...this.filterForm.value,
            PagingData: { currentPage: this.currentPage, take: this.take }
        };

        this.service.getAll(request).subscribe({
            next: (data: any) => {
                this.dataSource = new MatTableDataSource(data.item1);
                this.totalRows = data.item2;
                this.dataSource.sort = this.sort;

                if (data.item1.length > 0) {
                    setTimeout(() => {
                        this.paginator.pageIndex = this.currentPage;
                        this.paginator.length = this.totalRows;
                    });
                }

                this.isLoading = false;
            },
            error: () => this.isLoading = false
        });
    }

    pageChanged(event: PageEvent): void {
        this.take = event.pageSize;
        this.currentPage = event.pageIndex;
        this.bindData();
    }

    filterData() {
        this.currentPage = 0;
        this.bindData();
    }

    openDialog(element: any) {
        this.dialog.open(AddBloodUnitComponent, {
            panelClass: 'cstm_width_700',
            height: 'auto',
            data: { element },
            disableClose: true
        }).afterClosed().subscribe(() => this.bindData());
    }

    viewDialog(element: any): void {
        this.dialog.open(AddBloodUnitComponent, {
            data: { element, isViewMode: true },
            panelClass: 'cstm_width_700',
            height: 'auto',
            disableClose: true
        });
    }

    getComponentTypeName(element: any): string {
        return element?.bloodComponentType?.name || '';
    }

    getBloodGroupName(element: any): string {
        return element?.bloodGroupMaster?.name || '';
    }

    getStatusText(status: number): string {
        return this.statusMap[status] || '';
    }

    isStorageAssigned(element: any): boolean {
        return !!(element?.bloodFridgeId && element?.bloodRackId);
    }

    getStorageText(element: any): string {
        if (!this.isStorageAssigned(element)) {
            return '';
        }

        const fridge = element?.bloodFridge?.name || element?.bloodFridge?.code || 'Fridge';
        const rack = element?.bloodRack?.name || element?.bloodRack?.code || 'Rack';
        const slot = element?.slotNo ? `Slot ${element.slotNo}` : '';
        return [fridge, rack, slot].filter(Boolean).join(' | ');
    }

    deleteDialog(element: any) {
        this.dialog.open(DeleteBloodUnitComponent, {
            panelClass: 'cstm_width_500',
            height: 'auto',
            data: { element },
            disableClose: true
        }).afterClosed().subscribe(() => this.bindData());
    }
}
