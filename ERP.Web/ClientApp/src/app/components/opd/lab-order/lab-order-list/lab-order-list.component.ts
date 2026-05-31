import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ConstantService } from '../../../../Service/constant.service';
import { LabOrderService } from '../lab-order.service';
import { ViewLabOrderComponent } from '../view-lab-order/view-lab-order.component';
import { DeleteLabOrderComponent } from '../delete-lab-order/delete-lab-order.component';
import { Router } from '@angular/router';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { LabOrderTypeService } from '../../lab-order-type/lab-order-type.service';
import { PrimaryOrderService } from '../../../order/primary-order/order.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { SaveLabResultComponent } from '../save-lab-result/save-lab-result.component';
import { PrintResultComponent } from '../print-result/print-result.component';

@Component({
  selector: 'app-lab-order-list',
  templateUrl: './lab-order-list.component.html',
  styleUrls: ['./lab-order-list.component.css'],
  standalone: false
})
export class LabOrderListComponent implements OnInit {
  dataSource = new MatTableDataSource<any>([]);
  form!: FormGroup;
  displayedColumns: string[] = ['tokenNumber', 'patientName', 'testName', 'clinicalNotes', 'status', 'appointmentDate', 'actions'];
  isLoading = false;
  currentPage = 0;
  pageSize = 10;
  totalRecords = 0;
  
  labOrderTypes: any[] = [];
  statusList: any[] = [];
  
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private dialog: MatDialog,
    private fb: FormBuilder,
    private constantService: ConstantService,
    private service: LabOrderService,
    private labOrderTypeService: LabOrderTypeService,
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

  buildForm(): void {
    this.form = this.fb.group({
      tokenNumber: [''],
      patientName: [''],
      labOrderTypeId: [null],
      statusId: [null],
      fromDate: [null],
      toDate: [null]
    });
  }

  setupFilters(): void {
    // Debounce search filters
    this.form.get('tokenNumber')?.valueChanges.pipe(
      debounceTime(500),
      distinctUntilChanged()
    ).subscribe(() => {
      this.currentPage = 0;
      this.filterData();
    });

    this.form.get('patientName')?.valueChanges.pipe(
      debounceTime(500),
      distinctUntilChanged()
    ).subscribe(() => {
      this.currentPage = 0;
      this.filterData();
    });
  }

  loadLookups(): void {
    // Load lab order types
    this.labOrderTypeService.getAllLabOrderTypes({}).subscribe({
      next: (res: any) => {
        this.labOrderTypes = res?.item1 ?? [];
      },
      error: () => this.labOrderTypes = []
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
      tokenNumber: '',
      patientName: '',
      labOrderTypeId: null,
      statusId: null,
      fromDate: null,
      toDate: null
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
    this.service.getAllLabOrders(filter).subscribe({
      next: (data: any) => {
        this.dataSource.data = data?.item1 ?? [];
        this.totalRecords = data?.item2 ?? data?.item1?.length ?? 0;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notifications.showNotification('Error loading lab orders', 'snack-bar-danger');
      }
    });
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openAdd(element: any = {}): void {
    this.router.navigate(['/newlaborder'], { state: { element } });
  }

  openView(element: any): void {
    this.dialog.open(ViewLabOrderComponent, {
      data: { element },
      width: '600px',
      disableClose: true
    });
  }
  
 saveResult(element: any): void {
  this.dialog.open(SaveLabResultComponent, {
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
    this.dialog.open(DeleteLabOrderComponent, {
      data: {
    labOrderId: element.id,
    variables: element.labOrderType.variables
  },
      width: '400px',
      disableClose: true
    }).afterClosed().subscribe(() => {
      this.bindData();
    });
  }

   openprintResult(element: any): void {
    this.dialog.open(PrintResultComponent, {
      data: {element: element},
    panelClass: 'cstm_width_1300',
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