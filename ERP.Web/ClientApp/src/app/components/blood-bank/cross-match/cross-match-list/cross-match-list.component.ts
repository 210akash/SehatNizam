import { Component, ViewChild } from '@angular/core';

import { FormBuilder, FormGroup } from '@angular/forms';

import { MatDialog } from '@angular/material/dialog';

import { MatPaginator, PageEvent } from '@angular/material/paginator';

import { MatSort } from '@angular/material/sort';

import { MatTableDataSource } from '@angular/material/table';

import { AddCrossMatchComponent } from '../add-cross-match/add-cross-match.component';

import { DeleteCrossMatchComponent } from '../delete-cross-match/delete-cross-match.component';

import { CrossMatchService } from '../cross-match.service';



@Component({

    selector: 'app-cross-match-list',

    templateUrl: './cross-match-list.component.html',

    styleUrls: ['./cross-match-list.component.css'],

    standalone: false

})

export class CrossMatchListComponent {

    filterForm!: FormGroup;

    isLoading = false;

    currentPage = 0;

    pageSizeOptions: number[] = [5, 10, 25, 100];

    displayedColumns: string[] = ['requestCode', 'patientName', 'patientCNIC', 'requestDetails', 'unit', 'crossMatchDate', 'result', 'actions'];

    dataSource: any;

    take = 10;

    totalRows = 0;

    readonly inProcessResult = 3;

    resultMap: { [key: number]: string } = {

        0: 'Not Assigned',

        1: 'Compatible',

        2: 'Incompatible',

        3: 'In Process'

    };



    @ViewChild(MatPaginator) paginator!: MatPaginator;

    @ViewChild(MatSort) sort!: MatSort;



    constructor(

        private service: CrossMatchService,

        private dialog: MatDialog,

        private formBuilder: FormBuilder

    ) { }



    ngOnInit(): void {

        this.filterForm = this.formBuilder.group({

            requestCode: ['']

        });

        this.bindData();

    }



    bindData(): void {

        this.isLoading = true;

        const request = {

            ...this.filterForm.value,

            PagingData: { currentPage: this.currentPage, take: this.take }

        };



        this.service.getWorklist(request).subscribe({

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



    assignUnitDialog(row: any) {

        this.dialog.open(AddCrossMatchComponent, {

            panelClass: 'cstm_width_700',

            height: 'auto',

            data: { worklistRow: row, mode: 'assign' },

            disableClose: true

        }).afterClosed().subscribe(() => this.bindData());

    }



    updateResultDialog(row: any) {

        this.dialog.open(AddCrossMatchComponent, {

            panelClass: 'cstm_width_700',

            height: 'auto',

            data: { worklistRow: row, mode: 'updateResult' },

            disableClose: true

        }).afterClosed().subscribe(() => this.bindData());

    }



    getRequestCode(element: any): string {

        return element?.bloodRequest?.code || '';

    }



    getPatientName(element: any): string {

        return element?.bloodRequest?.patientName || '';

    }



    getPatientCNIC(element: any): string {

        return element?.bloodRequest?.patientCNIC || '';

    }

    getRequestDetails(element: any): string {
        const req = element?.bloodRequest;
        if (!req) return '—';
        const bloodGroup = req.bloodGroupMaster?.name || req.bloodGroupMaster?.code || '';
        const component = req.bloodComponentType?.name || req.bloodComponentType?.code || '';
        const quantity = req.quantity != null ? `Qty ${req.quantity}` : '';
        return [bloodGroup, component, quantity].filter(Boolean).join(' | ') || '—';
    }

    getUnitNo(element: any): string {

        return element?.bloodUnit?.unitNo || '—';

    }



    getResultText(result: number): string {

        return this.resultMap[result] || 'Not Assigned';

    }



    isNotAssigned(element: any): boolean {

        return !element?.crossMatchId || element?.result === 0;

    }



    isInProcess(element: any): boolean {

        return element?.result === this.inProcessResult;

    }



    deleteDialog(element: any) {

        this.dialog.open(DeleteCrossMatchComponent, {

            panelClass: 'cstm_width_500',

            height: 'auto',

            data: { element },

            disableClose: true

        }).afterClosed().subscribe(() => this.bindData());

    }

}


