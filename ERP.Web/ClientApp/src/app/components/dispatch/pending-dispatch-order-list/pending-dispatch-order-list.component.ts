import { Component, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort'; // Import MatSort and Sort
import { ConstantService } from '../../../Service/constant.service';
import { DispatchService } from '../dispatch.service';
import { ViewOrderComponent } from '../../order/primary-order/view-order/view-order.component';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { DealershipService } from '../../order/dealership/dealership.service';

@Component({
  selector: 'app-pending-dispatch-order-list',
  templateUrl: './pending-dispatch-order-list.component.html',
  styleUrls: ['./pending-dispatch-order-list.component.css'],
  standalone: false
})

export class PendingDispatchOrderListComponent {
  isLoading = false;
  displayedColumns: string[] = ['id', 'dealership', 'zone', 'territory', 'orderStatus', 'createdDate', 'actions'];
  dataSource: any;
  currentUser: any;
  roleList: string | undefined;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  orderListFilerForm!: FormGroup;
  dealershipList: any[] = [];
  constructor(private formBuilder: FormBuilder, private dialog: MatDialog, private dispatchService: DispatchService,
    private dealershipService: DealershipService,
    private constantService: ConstantService) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());

    this.orderListFilerForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      dealershipName: [''],
      dealershipId: [0],
      code: ['']
    });

    const startDate = new Date(2024, 0, 1);
    const endDate = new Date();
    this.orderListFilerForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.orderListFilerForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.bindData();
  }

  async bindData() {
    this.isLoading = true;
    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    };
    let _orderListFilerForm: any = {};
    _orderListFilerForm = Object.assign(_orderListFilerForm, this.orderListFilerForm.value);
    _orderListFilerForm["PagingData"] = pagingData;

    (await this.dispatchService.getOrdersToDispatch(_orderListFilerForm)).subscribe({
      next: (data: any) => {
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
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


  filterData() {
    this.isLoading = true;
    let _orderListFilerForm: any = {};
    _orderListFilerForm = Object.assign(_orderListFilerForm, this.orderListFilerForm.value);
    this.bindData();
    this.isLoading = false;

  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openViewOrderDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewOrderComponent, {
      data: { element: element },
      width: '70%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true
    }),
    {
      enterAnimationDuration,
      exitAnimationDuration,
    };
  }


  async getDealershipsList(event: any) {
    const filter = event.currentTarget.value;
    this.dealershipList = [];
    (await this.dealershipService.getAllActiveByName(filter)).subscribe(
      (data: any) => {
        this.dealershipList = data || [];
      },
      (error: any) => {
        console.error('Error fetching distributor list:', error);
        this.dealershipList = [];
      }
    );
  }

  onInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      this.orderListFilerForm.get('dealershipId')?.patchValue(0);
      this.filterData();
    }
  }

  onOptionDealershipSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      return;
    }
    this.orderListFilerForm.get('dealershipId')?.patchValue(selectedValue.id);
    this.orderListFilerForm.get('dealershipName')?.patchValue(selectedValue.name + ' | ' + selectedValue.address);
    this.filterData();
  }
}