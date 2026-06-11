import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AddBloodRequestComponent } from '../add-blood-request/add-blood-request.component';
import { DeleteBloodRequestComponent } from '../delete-blood-request/delete-blood-request.component';
import { BloodRequestLogComponent } from '../blood-request-log/blood-request-log.component';
import { BloodRequestService } from '../blood-request.service';

@Component({
    selector: 'app-blood-request-list',
    templateUrl: './blood-request-list.component.html',
    styleUrls: ['./blood-request-list.component.css'],
    standalone: false
})
export class BloodRequestListComponent {
    filterForm!: FormGroup;
    isLoading = false;
    currentPage = 0;
    pageSizeOptions: number[] = [5, 10, 25, 100];
    displayedColumns: string[] = ['code', 'patientName', 'patientCNIC', 'bloodGroup', 'componentType', 'quantity', 'requestDate', 'status', 'actions'];
    dataSource: any;
    take = 10;
    totalRows = 0;
    statusMap: { [key: number]: string } = {
        1: 'Pending',
        2: 'Cross Matched',
        3: 'Issued',
        4: 'Cancelled'
    };
    statusFilterOptions = [
        { value: 0, name: 'All' },
        { value: 1, name: 'Pending' },
        { value: 2, name: 'Cross Matched' },
        { value: 3, name: 'Issued' },
        { value: 4, name: 'Cancelled' }
    ];

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    constructor(
        private service: BloodRequestService,
        private dialog: MatDialog,
        private formBuilder: FormBuilder
    ) { }

    ngOnInit(): void {
        this.filterForm = this.formBuilder.group({
            patientCNIC: [''],
            status: [0]
        });
        this.bindData();
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
        this.dialog.open(AddBloodRequestComponent, {
            panelClass: 'cstm_width_700',
            height: 'auto',
            data: { element },
            disableClose: true
        }).afterClosed().subscribe(() => this.bindData());
    }

    viewDialog(element: any): void {
        this.dialog.open(AddBloodRequestComponent, {
            data: { element, isViewMode: true },
            panelClass: 'cstm_width_700',
            height: 'auto',
            disableClose: true
        });
    }

    viewLogDialog(element: any): void {
        this.dialog.open(BloodRequestLogComponent, {
            data: { element },
            panelClass: 'cstm_width_800',
            height: 'auto',
            maxHeight: '90vh',
            disableClose: true
        });
    }

    deleteDialog(element: any) {
        this.dialog.open(DeleteBloodRequestComponent, {
            panelClass: 'cstm_width_500',
            height: 'auto',
            data: { element },
            disableClose: true
        }).afterClosed().subscribe(() => this.bindData());
    }

    getBloodGroupName(element: any): string {
        return element?.bloodGroupMaster?.name || '';
    }

    getComponentTypeName(element: any): string {
        return element?.bloodComponentType?.name || '';
    }

    getStatusText(status: number): string {
        return this.statusMap[status] || '';
    }
}
