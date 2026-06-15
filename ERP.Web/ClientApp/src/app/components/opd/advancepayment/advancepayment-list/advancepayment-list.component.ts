
import { DeleteAdvancePaymentComponent } from '../delete-advancepayment/delete-advancepayment.component';
import { AddAdvancePaymentComponent } from '../add-advancepayment/add-advancepayment.component';
import { Component, Inject, Optional, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AdvancePaymentService } from '../advancepayment.service';
import { ServiceService } from '../../service/service.service';
import { ConfirmAdvancePaymentComponent } from '../confirm-advancepayment/confirm-advancepayment.component';
import { ViewAdvancePaymentComponent } from '../view-advancepayment/view-advancepayment.component';

@Component({
  selector: 'app-advancepayment-list',
  templateUrl: './advancepayment-list.component.html',
  styleUrl: './advancepayment-list.component.css',
    standalone: false
})

export class AdvancePaymentListComponent {
  ServiceFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['appoinmentno','mrnno','patientname','paymentMode','amount', 'paymentDate', 'status',  'actions'];
  dataSource: any;
  take = 50;
  totalRows = 0;

  constructor(private admissionServiceService : AdvancePaymentService,private serviceService: ServiceService, private dialog: MatDialog, private formBuilder: FormBuilder, @Optional() @Inject(MAT_DIALOG_DATA) public data: { element: any } | null) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  async ngOnInit(): Promise<void> {
    this.ServiceFilterForm = this.formBuilder.group({
      appointmentno: [''],
      mRN: [''],
      patientName: [''],
      fDate: [new Date()],
      tDate: [new Date()],
      statusId : [null]
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

    this.admissionServiceService.getAllAdvancePayments(_ServiceFilterForm).subscribe({
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

  filterData() {
    this.bindData();
  }


  openServiceDialog(element: any) {
    const dialogRef = this.dialog.open(AddAdvancePaymentComponent, {
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
    const dialogRef = this.dialog.open(DeleteAdvancePaymentComponent, {
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

    confirmServiceDialog(element: any) {
    const dialogRef = this.dialog.open(ConfirmAdvancePaymentComponent, {
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

  
    viewServiceDialog(element: any) {
    const dialogRef = this.dialog.open(ViewAdvancePaymentComponent, {
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
