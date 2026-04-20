import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NotificationService } from '../notification.service';
import { AddNotificationComponent } from '../add-notification/add-notification.component';
import { DeleteNotificationComponent } from '../delete-notification/delete-notification.component';
import { ViewNotificationComponent } from '../view-notification/view-notification.component';
import { DepartmentService } from '../../../department/department.service';

@Component({
    selector: 'app-notification-list',
    templateUrl: './notification-list.component.html',
    styleUrls: ['./notification-list.component.css'],
    standalone: false
})

export class NotificationListComponent {
    notificationFilterForm!: FormGroup;
    isLoading = false;
    currentPage = 0;
    pageSize = 0;
    pageSizeOptions: number[] = [5, 10, 25, 100];
    displayedColumns: string[] = ['title', 'departmentName', 'expireDate', 'isExpired', 'createdBy', 'actions'];
    dataSource: any;
    take = 50;
    totalRows = 0;
    departmentList: any[] = [];

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    constructor(
        private notificationService: NotificationService,
        private dialog: MatDialog,
        private departmentService: DepartmentService,
        private formBuilder: FormBuilder
    ) { }

    async ngOnInit(): Promise<void> {
        this.notificationFilterForm = this.formBuilder.group({
            departmentId: ['']
        });
        this.getDepartmentList();
        await this.bindData();
    }

  getDepartmentList(): void {
    this.departmentService.getDepartmentByCompany('2').subscribe(data => {
      this.departmentList = data;
    });
  }

    async bindData(): Promise<void> {
        this.isLoading = true;

        const pagingData = {
            currentPage: this.currentPage,
            take: this.take
        };

        const _notificationFilterForm = {
            ...this.notificationFilterForm.value,
            pagingData: pagingData
        };

        (await this.notificationService.getAllNotifications(_notificationFilterForm)).subscribe({
            next: (data: any) => {
                this.dataSource = new MatTableDataSource(data.item1);
                this.dataSource.sort = this.sort;

                if (data.item1 && data.item1.length > 0) {
                    setTimeout(() => {
                        this.paginator.pageIndex = this.currentPage;
                        this.paginator.length = data.item2;
                    });
                }

                this.isLoading = false;
            },
            error: (error: any) => {
                console.error('Error fetching data:', error);
                this.isLoading = false;
            }
        });
    }

    pageChanged(event: PageEvent): void {
        this.pageSize = event.pageSize;
        this.currentPage = event.pageIndex;
        this.bindData();
    }

    openAddNotificationDialog(element: any = null) {
        const dialogRef = this.dialog.open(AddNotificationComponent, {
            panelClass: 'cstm_width_700',
            height: 'auto',
            data: {
                element: element,
            },
            disableClose: true
        });

        dialogRef.afterClosed().subscribe(result => {
            this.bindData();
        });
    }

    viewNotificationDialog(element: any): void {
        this.dialog.open(ViewNotificationComponent, {
            data: { element: element },
            panelClass: 'cstm_width_700',
            height: 'auto',
            disableClose: true
        });
    }

    deleteNotificationDialog(element: any) {
        const dialogRef = this.dialog.open(DeleteNotificationComponent, {
            panelClass: 'cstm_width_500',
            height: 'auto',
            data: {
                element: element,
            },
            disableClose: true
        });

        dialogRef.afterClosed().subscribe(result => {
            this.bindData();
        });
    }
}
