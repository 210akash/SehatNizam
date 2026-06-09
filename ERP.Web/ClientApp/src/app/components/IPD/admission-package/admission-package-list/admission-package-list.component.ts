import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import Swal from 'sweetalert2';
import { NotificationsService } from '../../../../Service/notification.service';
import { AddAdmissionPackageComponent } from '../add-admission-package/add-admission-package.component';
import { AdmissionPackageService } from '../admission-package.service';

@Component({
    selector: 'app-admission-package-list',
    templateUrl: './admission-package-list.component.html',
    styleUrls: ['./admission-package-list.component.css'],
    standalone: false
})
export class AdmissionPackageListComponent {
    filterForm!: FormGroup;
    isLoading = false;
    currentPage = 0;
    pageSizeOptions: number[] = [5, 10, 25, 100];
    displayedColumns: string[] = ['name', 'description', 'servicesCount', 'actions'];
    dataSource: any;
    take = 10;
    totalRows = 0;

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    constructor(
        private admissionPackageService: AdmissionPackageService,
        private dialog: MatDialog,
        private formBuilder: FormBuilder,
        private notificationsService: NotificationsService
    ) { }

    ngOnInit(): void {
        this.filterForm = this.formBuilder.group({
            name: ['']
        });
        this.bindData();
    }

    bindData(): void {
        this.isLoading = true;

        const request = {
            ...this.filterForm.value,
            PagingData: {
                currentPage: this.currentPage,
                take: this.take
            }
        };

        this.admissionPackageService.getAllAdmissionPackages(request).subscribe({
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
            error: (error: any) => {
                console.error('Error fetching admission packages:', error);
                this.isLoading = false;
            }
        });
    }

    getServicesCount(element: any): number {
        return element?.admissionPackageDetail?.length || 0;
    }

    getServicesTotal(element: any): number {
        return (element?.admissionPackageDetail || []).reduce((total: number, detail: any) => {
            return total + Number(detail?.service?.basePrice ?? 0);
        }, 0);
    }

    pageChanged(event: PageEvent): void {
        this.take = event.pageSize;
        this.currentPage = event.pageIndex;
        this.bindData();
    }

    openDialog(element: any) {
        const dialogRef = this.dialog.open(AddAdmissionPackageComponent, {
            panelClass: 'cstm_width_700',
            height: 'auto',
            data: { element },
            disableClose: true
        });

        dialogRef.afterClosed().subscribe(() => this.bindData());
    }

    viewDialog(element: any): void {
        this.dialog.open(AddAdmissionPackageComponent, {
            data: { element, isViewMode: true },
            panelClass: 'cstm_width_700',
            height: 'auto',
            disableClose: true
        });
    }

    deleteDialog(element: any) {
        Swal.fire({
            title: 'Confirmation',
            text: `Are you sure you want to delete "${element.name}"?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (!result.isConfirmed) {
                return;
            }

            this.admissionPackageService.deleteAdmissionPackage(element.id).subscribe({
                next: (data) => {
                    if (data === true) {
                        this.notificationsService.showNotification('Successfully Deleted!', 'snack-bar-success');
                        this.bindData();
                    }
                },
                error: (error) => {
                    this.notificationsService.showNotification(error, 'snack-bar-danger');
                }
            });
        });
    }

    filterData() {
        this.currentPage = 0;
        this.bindData();
    }
}
