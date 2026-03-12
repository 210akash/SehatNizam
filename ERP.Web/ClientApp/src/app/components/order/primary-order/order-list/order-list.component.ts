import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { PrimaryOrderService } from '../order.service';
import { DeleteOrderComponent } from '../delete-order/delete-order.component';
import { ViewOrderComponent } from '../view-order/view-order.component';
import { CreateOrderComponent } from '../create-order/create-order.component';
import { OrderStatusChangeComponent } from '../order-status-change/order-status-change.component';
import { OrderHistoryComponent } from '../order-history/order-history.component';
import { AuthenticationService } from '../../../../Auth/authentication.service';
import { ConstantService, OrderStatusEnum } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { AreaService } from '../../area/area.service';
import { DealershipService } from '../../dealership/dealership.service';
import { RegionService } from '../../region/region.service';
import { TerritoryService } from '../../territory/territory.service';
import { ZoneService } from '../../zone/zone.service';
import { TemplateService } from '../../templates/template.service';
import { ReceiveDispatchComponent } from '../receive-dispatch/receive-dispatch.component';
import { PrintOrderReceiptComponent } from '../print-order-receipt/print-order-receipt.component';
import { CreateOrderWithPGComponent } from '../create-order-with-pg/create-order-with-pg.component';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { EditOrderComponent } from '../edit-order/edit-order.component';

@Component({
  selector: 'app-order-list',
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.css'], standalone: false
})

export class OrderListComponent implements OnInit {
  dataSource: any;
  orderListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['id', 'dealership', 'zone', 'territory', 'orderStatus', 'createdDate', 'quantity', 'actions'];
  isLoading = false;
  element: any;
  blob: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  receiveConfirmForm!: FormGroup;
  gElement: any;

  innerHtml: any;
  dialogRefPrint: any;

  regionList: any[] = [];
  zoneList: any[] = [];
  areaList: any[] = [];
  territoryList: any[] = [];
  dealershipList: any[] = [];
  orderStatusList: any[] = [];

  // currentUserRole: any;
  statusEnum: any;
  total: any;
  currentUser: any;
  roleList: string | undefined;
  constructor(private notificationsService: NotificationsService, private constantService: ConstantService, private areaService: AreaService, private templateService: TemplateService,
    private dialog: MatDialog, private orderService: PrimaryOrderService, private formBuilder: FormBuilder, private territoryService: TerritoryService, private regionService: RegionService,
    private dealershipService: DealershipService, private zoneService: ZoneService, private authService: AuthenticationService) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.statusEnum = OrderStatusEnum;
    this.pageSize = this.constantService.defaultItemPerPage;
    // this.currentUserRole = this.authService.currentUserRole;

    this.orderListFilerForm = this.formBuilder.group({
      orderId: [''],
      fdate: [new Date()],
      tdate: [new Date()],
      statusId: [0],
      regionId: [0],
      zoneId: [0],
      areaId: [0],
      territoryId: [0],
      dealershipName: [''],
      dealershipId: [0],
    });

    this.receiveConfirmForm = this.formBuilder.group({
      deliveryChallanCode: [''],
    })
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
    const today = new Date(); // today date
    const lastWeek = new Date();
    lastWeek.setDate(today.getDate() - 7);
    this.orderListFilerForm.get('fdate')?.patchValue(this.constantService.formatDate(lastWeek));
    this.orderListFilerForm.get('tdate')?.patchValue(this.constantService.formatDate(today));

    this.getAllStatus();
    this.bindData();
    this.getRegions();
  }

  openOrderDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateOrderComponent, {
      data: { element: element },
      width: '70%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openOrderDialogwithpg(element: any): void {
    const dialogRef = this.dialog.open(CreateOrderWithPGComponent, {
      data: { element: element },
      width: '70%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
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

  async bindData() {
    this.isLoading = true;
    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    }

    let _orderListFilerForm: any = {};
    _orderListFilerForm = Object.assign(_orderListFilerForm, this.orderListFilerForm.value);
    _orderListFilerForm["PagingData"] = pagingData;

    (await this.orderService.getAllOrder(_orderListFilerForm)).subscribe({
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

  pageChanged(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openDeleteDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteOrderComponent, {
      data: { element: element },
      width: '30%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openOrderStatusDialog(element: any, toStatusId: any) {

    let statement = "";
    if (toStatusId === OrderStatusEnum.InProcess) {
      statement = "Are you sure you want to process the order?";
    }
    else if (toStatusId === OrderStatusEnum.AccountReviewed) {
      statement = "Please confirm if you've thoroughly reviewed this order?";
    }
    else if (toStatusId === OrderStatusEnum.Confirm) {
      statement = "Are you sure you want to confirm the order?";
    }
    else if (toStatusId === OrderStatusEnum.Received) {
      statement = "Please enter comments!";
    }
    else if (toStatusId === OrderStatusEnum.Canceled) {
      statement = "Are you sure you want to cancel the order?";
    }
    const dialogRef = this.dialog.open(OrderStatusChangeComponent, {
      data: { element: element, toStatusId: toStatusId, statement: statement },
      width: '60%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  getRowClass(element: any) {
    if (element.orderStatusId === OrderStatusEnum.Canceled) {
      return 'cancel';
    }
    else {
      return '';
    }
  }

  openOrderHistoryDialog(element: any) {
    const dialogRef = this.dialog.open(OrderHistoryComponent, {
      data: { element: element },
      width: '50%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openReceiveDispatchDialog(element: any) {
    this.gElement = element;
    const dialogRef = this.dialog.open(ReceiveDispatchComponent, {
      data: { element: element },
      width: '45%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  async getOrderReceipt(element: any) {
    const dialogRef = this.dialog.open(PrintOrderReceiptComponent, {
      data: { element: element },
      width: '45%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
    // (await this.templateService.getPrintTemplate(orderId, templateId)).subscribe({
    //   next: async (data: { Data: SafeHtml; }) => {

    //     // const newTab = window.open('', '_blank');
    //     // var response = data.Data.replace(/\+/g, '%20');
    //     // // let printData = decodeURIComponent(response);
    //     // let user = JSON.parse(localStorage.getItem('currentUser'));
    //     // newTab.document.write(`
    //     //   <html>
    //     //     <head>
    //     //       <title>App Print</title>
    //     //       <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@3.3.7/dist/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">
    //     //     </head>
    //     //     <body>
    //     //     ${response}

    //     //     </body>
    //     //   </html>
    //     // `);
    //     // newTab.focus();

    //     this.innerHtml = data.Data as SafeHtml as string;
    //     this.dialogRefPrint = this.dialog.open(template, {
    //       width: '50%',
    //       minHeight: '500px',
    //       maxHeight: '90vh',
    //       disableClose: true,
    //     });

    //     this.dialogRefPrint.afterClosed().subscribe((result: any) => {
    //     });
    //   },
    //   error: (error: any) => {
    //     console.log(error);
    //   }
    // });
  }

  print(event: any) {
    const WindowPrt = window.open('', '', 'left=0,top=0,width=1100,height=1100,toolbar=0,scrollbars=0,status=0');
    WindowPrt!.document.write(this.innerHtml);
    setTimeout(() => {
      WindowPrt!.focus();
      WindowPrt!.print();
      WindowPrt!.close();
    }, 1000);
  }

  filterData() {
    this.isLoading = true;
    let _orderListFilerForm: any = {};
    _orderListFilerForm = Object.assign(_orderListFilerForm, this.orderListFilerForm.value);
    this.bindData();
    this.isLoading = false;

  }

  onReset() {
    const today = new Date(); // today date
    const lastWeek = new Date();
    lastWeek.setDate(today.getDate() - 7);
    this.orderListFilerForm.get('fdate')?.patchValue(this.constantService.formatDate(lastWeek));
    this.orderListFilerForm.get('tdate')?.patchValue(this.constantService.formatDate(today));

    this.orderListFilerForm.patchValue({
      statusId: 0,
      regionId: 0,
      zoneId: 0,
      areaId: 0,
      territoryId: 0,
      dealershipId: 0,
    });
    this.bindData();
  }

  // onZoneChange() {
  //   this.orderListFilerForm.get('territoryId')?.patchValue(0);
  //   let zoneId = this.orderListFilerForm.get('zoneId').value;

  //   this.getTerritoryByAreaId(zoneId);
  //   this.filterData();
  // }

  // async getTerritoryByAreaId(zoneId: any) {
  //   (await this.territoryService.getTerritoryByAreaId(zoneId)).subscribe({
  //     next: (data) => {
  //       if (data && Array.isArray(data)) {
  //         this.territoryList = data;
  //       } else {
  //         console.error('Expected array but got:', data);
  //         this.territoryList = [];
  //       }
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }

  // async getZones() {
  //   let _zoneFilterForm = {};
  //   (await this.zoneService.getAllZone(_zoneFilterForm)).subscribe({
  //     next: (data) => {
  //       if (data && Array.isArray(data.item1)) {
  //         this.zoneList = data.item1;
  //       } else {
  //         console.error('Expected array but got:', data.item1);
  //         this.zoneList = [];
  //       }
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }

  async getDealershipsByTerritoryId() {
    let territoryId = this.orderListFilerForm.get('territoryId')?.value;
    (await this.dealershipService.getDealershipByTerritoryId(territoryId)).subscribe({
      next: (data) => {
        if (data && Array.isArray(data)) {
          this.dealershipList = data;
        } else {
          console.error('Expected array but got:', data);
          this.dealershipList = [];
        }

        this.filterData();
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getAllStatus() {
    (await this.orderService.getAllOrderStatus()).subscribe({
      next: (data: any[]) => {

        this.orderStatusList = data;
        this.isLoading = false;
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getTerritoryByAreaId() {

    this.territoryList = [];
    this.dealershipList = [];

    this.orderListFilerForm.get('territoryId')?.patchValue(0);
    (await this.territoryService.getTerritoryByAreaId(this.orderListFilerForm.get('areaId')?.value)).subscribe(
      {
        next: (data) => {
          this.territoryList = data;
        },
        error: (error) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  async getZoneByRegionId() {

    this.zoneList = [];
    this.areaList = [];
    this.territoryList = [];
    this.dealershipList = [];

    (await this.zoneService.getZoneByRegionId(this.orderListFilerForm.get('regionId')?.value)).subscribe({
      next: (data) => {
        this.zoneList = data;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getRegions() {
    let _regionFilterForm = {};
    (await this.regionService.getAllRegion(_regionFilterForm)).subscribe({
      next: (data) => {
        this.regionList = data.item1;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getAreaByZoneId() {

    this.areaList = [];
    this.territoryList = [];
    this.dealershipList = [];

    (await this.areaService.getAreaByZoneId(this.orderListFilerForm.get('zoneId')?.value)).subscribe({
      next: (data) => {
        this.areaList = data;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
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

  getTotalQuantity(orderItems: any[]): number {
    if (!Array.isArray(orderItems)) return 0;
    return orderItems  .filter(item => item?.isActive).reduce((total, item) => total + (item.quantity || 0), 0);
  }

  editOrderDialog(element: any): void {
    const dialogRef = this.dialog.open(EditOrderComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

}