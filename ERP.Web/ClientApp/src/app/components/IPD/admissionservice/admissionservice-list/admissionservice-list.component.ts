
import { DeleteAdmissionServiceComponent } from '../delete-admissionservice/delete-admissionservice.component';
import { AddAdmissionServiceComponent } from '../add-admissionservice/add-admissionservice.component';
import { Component, Inject, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AdmissionServiceService } from '../admissionservice.service';
import { ServiceService } from '../../../opd/service/service.service';

@Component({
  selector: 'app-admissionservice-list',
  templateUrl: './admissionservice-list.component.html',
  styleUrl: './admissionservice-list.component.css',
    standalone: false
})

export class AdmissionServiceListComponent {
  ServiceFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['service','paymentMode','fee', 'discount', 'amount', 'paymentDate',  'actions'];
  dataSource: any;
  take = 50;
  totalRows = 0;

  constructor(private admissionServiceService : AdmissionServiceService,private serviceService: ServiceService, private dialog: MatDialog, private formBuilder: FormBuilder, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  async ngOnInit(): Promise<void> {
    this.ServiceFilterForm = this.formBuilder.group({
      appointmentId: [this.data.element.appointmentId]
    });
    await this.bindData();
  }

  async bindData(): Promise<void> {
    this.isLoading = true;

    const pagingData = {
      currentPage: this.currentPage,
      take: this.take
    };

    const _ServiceFilterForm = {
      ...this.ServiceFilterForm.value,
      PagingData: pagingData
    };

    this.admissionServiceService.getAllAdmissionServices(_ServiceFilterForm).subscribe({
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

        console.error('Error fetching data:', error);
        this.isLoading = false;
      }
    });
  }

  pageChanged(event: PageEvent): void {
    this.take = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openServiceDialog(element: any) {
    const dialogRef = this.dialog.open(AddAdmissionServiceComponent, {
      id: 'message-tracker-dialog',
      width: '40%',
      height: 'auto',
      data: { element: element, refreshList: this.bindData.bind(this) },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  deleteServiceDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteAdmissionServiceComponent, {
      id: 'message-delete-tracker',
      width: '40%',
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
