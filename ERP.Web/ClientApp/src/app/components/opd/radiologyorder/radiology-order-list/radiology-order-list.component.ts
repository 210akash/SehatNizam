import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ConstantService } from '../../../../Service/constant.service';
import { ViewRadiologyOrderComponent } from '../view-radiology-order/view-radiology-order.component';
import { Router } from '@angular/router';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { PrimaryOrderService } from '../../../order/primary-order/order.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { SaveRadiologyResultComponent } from '../save-radiology-result/save-radiology-result.component';
import { RadiologyOrderService } from '../radiologyorder.service';
import { RadiologyTypeService } from '../../radiologytype/radiologytype.service';
import { PrintRadiologyOrderResultComponent } from '../print-radiology-order-result/print-radiology-order-result.component';
import { ConfirmRadiologyOrderComponent } from '../confirm-radiology-order/confirm-radiology-order.component';
import { DeleteRadiologyOrderComponent } from '../delete-radiology-order/delete-radiology-order.component';

@Component({
  selector: 'app-radiology-order-list',
  templateUrl: './radiology-order-list.component.html',
  styleUrls: ['./radiology-order-list.component.css'],
  standalone: false
})
export class RadiologyOrderListComponent implements OnInit, AfterViewInit {
  dataSource = new MatTableDataSource<any>([]);
  form!: FormGroup;
  displayedColumns: string[] = ['appointmentDate','tokenNumber', 'patientName', 'testName', 'clinicalNotes','reference', 'status',  'actions'];
  isLoading = false;
  currentPage = 0;
  pageSize = 10;
  totalRecords = 0;
  
  radiologyOrderTypes: any[] = [];
  statusList: any[] = [];
  
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private dialog: MatDialog,
    private fb: FormBuilder,
    private constantService: ConstantService,
    private service: RadiologyOrderService,
    private radiologyTypeService: RadiologyTypeService,
    private orderStatusService: PrimaryOrderService,
    private router: Router,
    private notifications: NotificationsService
  ) { }

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.buildForm();
    this.setupFilters();
    this.loadLookups();
    this.bindData();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
  }

  buildForm(): void {
    this.form = this.fb.group({
      tokenNo: [''],
      mRN: [''],
      name: [''],
      radiologyOrderTypeId: [null],
      statusId: [null],
      fDate: [new Date()],
      tDate: [new Date()]
    });
    console.log(this.form.value);


    //  const currentYear = new Date().getFullYear();
    // const startDate = new Date(currentYear, 0, 1);

    // // const endDate = new Date(currentYear, 11, 31);
    // const currentDate = new Date();
    // const endDate = new Date(currentDate);
    // endDate.setDate(currentDate.getDate() + 2); // Add 2 days

    // this.TransactionFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    // this.TransactionFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

  }

  setupFilters(): void {
    // Debounce search filters
    this.form.get('tokenNo')?.valueChanges.pipe(
      debounceTime(500),
      distinctUntilChanged()
    ).subscribe(() => {
      this.currentPage = 0;
      this.filterData();
    });

    this.form.get('name')?.valueChanges.pipe(
      debounceTime(500),
      distinctUntilChanged()
    ).subscribe(() => {
      this.currentPage = 0;
      this.filterData();
    });
  }

  loadLookups(): void {
    // Load radiology order types
    this.radiologyTypeService.getAllRadiologyTypes({}).subscribe({
      next: (res: any) => {
        this.radiologyOrderTypes = res?.item1 ?? [];
      },
      error: () => this.radiologyOrderTypes = []
    });

    // Load status list
    this.orderStatusService.getAllOrderStatus().then(obs => {
      obs.subscribe((d: any) => {
        this.statusList = d ?? [];
      });
    });
  }

  filterData(): void {
    this.currentPage = 0;
    this.bindData();
  }

  clearFilters(): void {
    this.form.reset({
      tokenNo: '',
      name: '',
      radiologyOrderTypeId: null,
      statusId: null,
      fDate: null,
      tDate: null
    });
    this.currentPage = 0;
    this.bindData();
  }

  async bindData(): Promise<void> {
    const filter: any = {
      ...this.form.value,
      pagingData: {
        currentPage: this.currentPage,
        take: this.pageSize
      }
    };

    this.isLoading = true;
    this.service.getAllRadiologyOrders(filter).subscribe({
      next: (data: any) => {
        this.dataSource.data = data?.item1 ?? [];
        this.totalRecords = data?.item2 ?? data?.item1?.length ?? 0;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notifications.showNotification('Error loading radiology orders', 'snack-bar-danger');
      }
    });
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openAdd(element: any = {}): void {
    this.router.navigate(['/newradiologyorder'], { state: { element } });
  }

  openView(element: any): void {
    this.dialog.open(ViewRadiologyOrderComponent, {
      data: { element },
      panelClass: 'cstm_width_900',
      disableClose: true
    });
  }
  
 saveResult(element: any): void {
  this.dialog.open(SaveRadiologyResultComponent, {
    data: {
      order: element
    },
    panelClass: 'cstm_width_1300',
    maxHeight: '90vh',
    disableClose: true
  }).afterClosed().subscribe(() => {
    this.bindData();
  });
}

  openDelete(element: any): void {
    this.dialog.open(DeleteRadiologyOrderComponent, {
      data: {
    radiologyOrderId: element.id,
    variables: element.radiologyOrderType.variables
  },
      width: '400px',
      disableClose: true
    }).afterClosed().subscribe(() => {
      this.bindData();
    });
  }

   openprintResult(element: any): void {
    this.dialog.open(PrintRadiologyOrderResultComponent, {
      data: {element: element},
    panelClass: 'cstm_width_1300',
    maxHeight: '90vh',
      disableClose: true
    }).afterClosed().subscribe(() => {
      this.bindData();
    });
  }

  confirmRadiologyOrder(element: any): void {
    this.dialog.open(ConfirmRadiologyOrderComponent, {
      data: {element: element},
    panelClass: 'cstm_width_800',
    maxHeight: '90vh',
      disableClose: true
    }).afterClosed().subscribe(() => {
      this.bindData();
    });
  }


  

  printReport(element: any): void {
    // Implement print functionality
    console.log('Print report for:', element);
    this.notifications.showNotification('Print feature coming soon', 'snack-bar-info');
  }

  getStatusColor(status: string): string {
    const colors: { [key: string]: string } = {
      'Created': '#4caf50',
      'In Progress': '#ff9800',
      'Completed': '#2196f3',
      'Cancelled': '#f44336',
      'Pending': '#9e9e9e'
    };
    return colors[status] || '#9e9e9e';
  }
}