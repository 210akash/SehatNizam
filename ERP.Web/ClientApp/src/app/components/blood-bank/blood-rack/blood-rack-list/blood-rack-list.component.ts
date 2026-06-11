import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AddBloodRackComponent } from '../add-blood-rack/add-blood-rack.component';
import { DeleteBloodRackComponent } from '../delete-blood-rack/delete-blood-rack.component';
import { BloodRackService } from '../blood-rack.service';

@Component({
    selector: 'app-blood-rack-list',
    templateUrl: './blood-rack-list.component.html',
    styleUrls: ['./blood-rack-list.component.css'],
    standalone: false
})
export class BloodRackListComponent {
    filterForm!: FormGroup;
    isLoading = false;
    currentPage = 0;
    pageSizeOptions: number[] = [5, 10, 25, 100];
    displayedColumns: string[] = ['code', 'name', 'bloodFridge', 'actions'];
    dataSource: any;
    take = 10;
    totalRows = 0;

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    constructor(
        private service: BloodRackService,
        private dialog: MatDialog,
        private formBuilder: FormBuilder
    ) { }

    ngOnInit(): void {
        this.filterForm = this.formBuilder.group({ name: [''] });
        this.bindData();
    }

    bindData(): void {
        this.isLoading = true;
        const request = { ...this.filterForm.value, PagingData: { currentPage: this.currentPage, take: this.take } };
        this.service.getAll(request).subscribe({
            next: (data: any) => {
                this.dataSource = new MatTableDataSource(data.item1);
                this.totalRows = data.item2;
                this.dataSource.sort = this.sort;
                if (data.item1.length > 0) setTimeout(() => { this.paginator.pageIndex = this.currentPage; this.paginator.length = this.totalRows; });
                this.isLoading = false;
            },
            error: () => this.isLoading = false
        });
    }

    pageChanged(event: PageEvent): void { this.take = event.pageSize; this.currentPage = event.pageIndex; this.bindData(); }
    filterData() { this.currentPage = 0; this.bindData(); }
    openDialog(element: any) {
        this.dialog.open(AddBloodRackComponent, { panelClass: 'cstm_width_700', height: 'auto', data: { element }, disableClose: true }).afterClosed().subscribe(() => this.bindData());
    }
    viewDialog(element: any): void {
        this.dialog.open(AddBloodRackComponent, { data: { element, isViewMode: true }, panelClass: 'cstm_width_700', height: 'auto', disableClose: true });
    }
    deleteDialog(element: any) {
        this.dialog.open(DeleteBloodRackComponent, {
            panelClass: 'cstm_width_500',
            height: 'auto',
            data: { element },
            disableClose: true
        }).afterClosed().subscribe(() => this.bindData());
    }

    getFridgeName(element: any): string {
        return element?.bloodFridge?.name || '';
    }
}
