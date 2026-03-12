import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { DeleteCustomerComponent } from '../delete-customer/delete-customer.component';
import { ViewCustomerComponent } from '../view-customer/view-customer.component';
import { CreateCustomerComponent } from '../create-customer/create-customer.component';
import { DealershipService } from '../../order/dealership/dealership.service';
import { ConstantService } from '../../../Service/constant.service';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.css'],
  standalone: false,
})
export class CustomerListComponent implements OnInit {
  dataSource: any;
  customerListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = [
    'code',
    'name',
    'description',
    'createdDate',
    'isActive',
    'actions',
  ];
  isLoading = false;
  element: any;
  blob: any;

  isActive: boolean = false;
  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  regionList: any[] = [];
  zoneList: any[] = [];
  areaList: any[] = [];
  territoryList: any[] = [];

  constructor(
    private constantService: ConstantService,
    private dialog: MatDialog,
    private customerService: DealershipService,
    private formBuilder: FormBuilder  ) {}
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.customerListFilerForm = this.formBuilder.group({
      regionId: [0],
      zoneId: [0],
      areaId: [0],
      territoryId: [0],
      name: [''],
      isActive: [null],
    });

    this.bindData();
  }

  filterData1(event: any): void {
    // The `event.checked` will be `true` if the toggle is on, otherwise `false`
    this.isActive = event.checked;

    // Call any function or perform the action you need, passing the `isActive` value
    this.bindData();
  }

  openCustomerDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateCustomerComponent, {
      data: { element: element },
      width: '60%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openViewCustomerDialog(element: any): void {
    const dialogRef = this.dialog.open(ViewCustomerComponent, {
      data: { element: element },
      width: '60%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  async bindData() {
    this.isLoading = true;
    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize,
    };

    let _customerListFilerForm: any = {};
    _customerListFilerForm = Object.assign(
      _customerListFilerForm,
      this.customerListFilerForm.value
    );

    _customerListFilerForm['PagingData'] = pagingData;
    _customerListFilerForm['dealershipTypeId'] = 2;

    (
      await this.customerService.getAllDealershipList(_customerListFilerForm)
    ).subscribe({
      next: (data) => {
        this.dataSource = new MatTableDataSource(data.item1);
        if (data.item1.length > 0) {
          setTimeout(() => {
            this.paginator.pageIndex = this.currentPage;
            this.paginator.length = data.item2;
          });
        }
        console.log(this.dataSource);
        this.isLoading = false;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      },
    });
  }

  pageChanged(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openDeleteDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteCustomerComponent, {
      data: { element: element },
      width: '60%',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  onReset() {
    this.customerListFilerForm.patchValue({
      regionId: 0,
      zoneId: 0,
      areaId: 0,
      territoryId: 0,
      isActive: null,
    });
    this.bindData();
  }

  filterData() {
    this.bindData();
  }
}
