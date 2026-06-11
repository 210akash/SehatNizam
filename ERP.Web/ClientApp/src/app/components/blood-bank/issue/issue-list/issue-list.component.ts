import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AddIssueComponent } from '../add-issue/add-issue.component';
import { DeleteIssueComponent } from '../delete-issue/delete-issue.component';
import { ReturnToCrossMatchComponent } from '../return-to-cross-match/return-to-cross-match.component';
import { IssueService } from '../issue.service';

@Component({
    selector: 'app-issue-list',
    templateUrl: './issue-list.component.html',
    styleUrls: ['./issue-list.component.css'],
    standalone: false
})
export class IssueListComponent {
    filterForm!: FormGroup;
    historyFilterForm!: FormGroup;
    isLoading = false;
    isHistoryLoading = false;
    selectedTab = 0;
    currentPage = 0;
    historyPage = 0;
    pageSizeOptions: number[] = [5, 10, 25, 100];
    pendingColumns: string[] = ['requestCode', 'patientName', 'patientCNIC', 'requestDetails', 'unit', 'crossMatchDate', 'actions'];
    historyColumns: string[] = ['requestCode', 'patientName', 'patientCNIC', 'requestDetails', 'unit', 'crossMatchDate', 'issueDate', 'issuedTo', 'actions'];
    pendingDataSource: any;
    historyDataSource: any;
    take = 10;
    historyTake = 10;
    totalRows = 0;
    historyTotalRows = 0;

    @ViewChild('pendingPaginator') pendingPaginator!: MatPaginator;
    @ViewChild('historyPaginator') historyPaginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    constructor(
        private service: IssueService,
        private dialog: MatDialog,
        private formBuilder: FormBuilder
    ) { }

    ngOnInit(): void {
        this.filterForm = this.formBuilder.group({ requestCode: [''] });
        this.historyFilterForm = this.formBuilder.group({ requestCode: [''], issuedTo: [''] });
        this.bindPendingData();
        this.bindHistoryData();
    }

    onTabChange(index: number): void {
        this.selectedTab = index;
        if (index === 0) {
            this.bindPendingData();
        } else {
            this.bindHistoryData();
        }
    }

    bindPendingData(): void {
        this.isLoading = true;
        const request = {
            ...this.filterForm.value,
            PagingData: { currentPage: this.currentPage, take: this.take }
        };

        this.service.getWorklist(request).subscribe({
            next: (data: any) => {
                const items = data.item1 || data.Item1 || [];
                this.pendingDataSource = new MatTableDataSource(items);
                this.totalRows = data.item2 ?? data.Item2 ?? items.length;
                this.isLoading = false;
            },
            error: () => this.isLoading = false
        });
    }

    bindHistoryData(): void {
        this.isHistoryLoading = true;
        const request = {
            ...this.historyFilterForm.value,
            PagingData: { currentPage: this.historyPage, take: this.historyTake }
        };

        this.service.getAll(request).subscribe({
            next: (data: any) => {
                const items = data.item1 || data.Item1 || [];
                this.historyDataSource = new MatTableDataSource(items);
                this.historyTotalRows = data.item2 ?? data.Item2 ?? items.length;
                this.isHistoryLoading = false;
            },
            error: () => this.isHistoryLoading = false
        });
    }

    pendingPageChanged(event: PageEvent): void {
        this.take = event.pageSize;
        this.currentPage = event.pageIndex;
        this.bindPendingData();
    }

    historyPageChanged(event: PageEvent): void {
        this.historyTake = event.pageSize;
        this.historyPage = event.pageIndex;
        this.bindHistoryData();
    }

    filterPendingData(): void {
        this.currentPage = 0;
        this.bindPendingData();
    }

    filterHistoryData(): void {
        this.historyPage = 0;
        this.bindHistoryData();
    }

    issueDialog(row: any): void {
        this.dialog.open(AddIssueComponent, {
            panelClass: 'cstm_width_700',
            height: 'auto',
            data: { worklistRow: row, mode: 'issue' },
            disableClose: true
        }).afterClosed().subscribe(() => {
            this.bindPendingData();
            this.bindHistoryData();
        });
    }

    returnToCrossMatchDialog(row: any): void {
        this.dialog.open(ReturnToCrossMatchComponent, {
            panelClass: 'cstm_width_500',
            height: 'auto',
            data: { element: row },
            disableClose: true
        }).afterClosed().subscribe(() => this.bindPendingData());
    }

    editDialog(element: any): void {
        this.dialog.open(AddIssueComponent, {
            panelClass: 'cstm_width_700',
            height: 'auto',
            data: { element, mode: 'edit' },
            disableClose: true
        }).afterClosed().subscribe(() => this.bindHistoryData());
    }

    viewDialog(element: any): void {
        this.dialog.open(AddIssueComponent, {
            data: { element, mode: 'view' },
            panelClass: 'cstm_width_700',
            height: 'auto',
            disableClose: true
        });
    }

    deleteDialog(element: any): void {
        this.dialog.open(DeleteIssueComponent, {
            panelClass: 'cstm_width_500',
            height: 'auto',
            data: { element },
            disableClose: true
        }).afterClosed().subscribe(() => {
            this.bindPendingData();
            this.bindHistoryData();
        });
    }

    getRequestCode(row: any): string {
        return row?.bloodRequest?.code || '';
    }

    getPatientName(row: any): string {
        return row?.bloodRequest?.patientName || '';
    }

    getPatientCNIC(row: any): string {
        return row?.bloodRequest?.patientCNIC || '';
    }

    getRequestDetails(row: any): string {
        const req = row?.bloodRequest;
        if (!req) return '—';
        const bloodGroup = req.bloodGroupMaster?.name || req.bloodGroupMaster?.code || '';
        const component = req.bloodComponentType?.name || req.bloodComponentType?.code || '';
        const quantity = req.quantity != null ? `Qty ${req.quantity}` : '';
        return [bloodGroup, component, quantity].filter(Boolean).join(' | ') || '—';
    }

    getUnitNo(row: any): string {
        return row?.bloodUnit?.unitNo || '—';
    }

    getCrossMatchDate(row: any): Date | null {
        if (row?.crossMatchDate) return new Date(row.crossMatchDate);
        if (row?.bloodCrossMatch?.crossMatchDate) return new Date(row.bloodCrossMatch.crossMatchDate);
        return null;
    }
}
