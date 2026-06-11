import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AddBloodGroupComponent } from '../add-blood-group/add-blood-group.component';
import { DeleteBloodGroupComponent } from '../delete-blood-group/delete-blood-group.component';
import { BloodGroupService } from '../blood-group.service';

@Component({
    selector: 'app-blood-group-list',
    templateUrl: './blood-group-list.component.html',
    styleUrls: ['./blood-group-list.component.css'],
    standalone: false
})
export class BloodGroupListComponent {
    filterForm!: FormGroup;
    isLoading = false;
    currentPage = 0;
    pageSizeOptions: number[] = [5, 10, 25, 100];
    displayedColumns: string[] = ['code', 'name', 'description', 'actions'];
    dataSource: any;
    take = 10;
    totalRows = 0;

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    constructor(
        private service: BloodGroupService,
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
        this.dialog.open(AddBloodGroupComponent, { panelClass: 'cstm_width_700', height: 'auto', data: { element }, disableClose: true }).afterClosed().subscribe(() => this.bindData());
    }
    viewDialog(element: any): void {
        this.dialog.open(AddBloodGroupComponent, { data: { element, isViewMode: true }, panelClass: 'cstm_width_700', height: 'auto', disableClose: true });
    }
    deleteDialog(element: any) {
        this.dialog.open(DeleteBloodGroupComponent, {
            panelClass: 'cstm_width_500',
            height: 'auto',
            data: { element },
            disableClose: true
        }).afterClosed().subscribe(() => this.bindData());
    }
}
