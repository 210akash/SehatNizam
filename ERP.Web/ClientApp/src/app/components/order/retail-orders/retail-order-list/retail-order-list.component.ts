import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { SafeHtml } from '@angular/platform-browser';
import { AuthenticationService } from '../../../../Auth/authentication.service';
import { ConstantService, OrderStatusEnum } from '../../../../Service/constant.service';
import { AreaService } from '../../area/area.service';
import { PrimaryOrderService } from '../../primary-order/order.service';
import { RegionService } from '../../region/region.service';
import { TerritoryService } from '../../territory/territory.service';
import { ZoneService } from '../../zone/zone.service';
import { TemplateService } from '../../templates/template.service';
import { PrintRetailOrderReceiptComponent } from '../print-retail-order-receipt/print-retail-order-receipt.component';
import { RetailOrderService } from '../retail-order.service';
import { DeleteRetailOrderComponent } from '../delete-retail-order/delete-retail-order.component';
import { RetailOrderStatusChangeComponent } from '../retail-order-status-change/retail-order-status-change.component';
import { ConfirmRetailOrderQuantityComponent } from '../confirm-retail-order-quantity/confirm-retail-order-quantity.component';
import { CreateRetailOrderComponent } from '../create-retail-order/create-retail-order.component';
import { RetailOrderHistoryComponent } from '../retail-order-history/retail-order-history.component';
import { ViewRetailOrderComponent } from '../view-retail-orders/view-retail-order.component';

@Component({
  selector: 'app-retail-order-list',
  templateUrl: './retail-order-list.component.html',
  styleUrls: ['./retail-order-list.component.css'], standalone: false
})

export class RetailOrderListComponent implements OnInit {
  dataSource: any;
  shopOrderListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['id', 'shop', 'orderStatus', 'createdDate', 'actions'];
  isLoading = false;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  receiveConfirmForm!: FormGroup;
  gElement: any;

  innerHtml: any;
  dialogRefPrint: any;
  currentUserRole: any;
  orderStatusList: any;

  regionList: any[] = [];
  zoneList: any[] = [];
  areaList: any[] = [];
  territoryList: any[] = [];
  shopList: any[] = [];

  statusEnum: any;

  constructor(private authService: AuthenticationService, private constantService: ConstantService, private dialog: MatDialog, private areaService: AreaService,
    private retailOrderService: RetailOrderService, private orderService: PrimaryOrderService, private formBuilder: FormBuilder,
    private territoryService: TerritoryService, private zoneService: ZoneService, private regionService: RegionService,
    private templateService: TemplateService) { }

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.statusEnum = OrderStatusEnum;
    // this.currentUserRole = this.authService.currentUserRole;
    this.pageSize = this.constantService.defaultItemPerPage;

    this.shopOrderListFilerForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      statusId: [0],
      retailOrderId: [null],
    });

    this.receiveConfirmForm = this.formBuilder.group({
      deliveryChallanCode: [''],
    })

    const today = new Date(); // today date
    const lastWeek = new Date();
    lastWeek.setDate(today.getDate() - 7);
    this.shopOrderListFilerForm.get('fdate')?.patchValue(this.constantService.formatDate(lastWeek));
    this.shopOrderListFilerForm.get('tdate')?.patchValue(this.constantService.formatDate(today));
    this.getAllStatus();
    this.bindData();
    this.getRegions();
  }

  filterData() {
    this.isLoading = true;
    let _shopOrderListFilterForm: any = {};
    _shopOrderListFilterForm = Object.assign(_shopOrderListFilterForm, this.shopOrderListFilerForm.value);
    this.bindData();
    this.isLoading = false;

  }

  openCreateRetailOrderDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateRetailOrderComponent, {
      data: { element: element },
      width: '80%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openViewRetailOrderDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewRetailOrderComponent, {
      data: { element: element },
      width: '80%',
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

    let _shopOrderListFilerForm: any = {};
    _shopOrderListFilerForm = Object.assign(_shopOrderListFilerForm, this.shopOrderListFilerForm.value);
    _shopOrderListFilerForm["PagingData"] = pagingData;

    (await this.retailOrderService.getAllRetailOrder(_shopOrderListFilerForm)).subscribe({
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
      }
    });
  }

  pageChanged(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openDeleteDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteRetailOrderComponent, {
      data: { element: element },
      width: '80%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openRetailOrderStatusDialog(element: any, toStatusId: any) {
    let statement = "";
    if (toStatusId === OrderStatusEnum.Confirm) {
      statement = "Are you sure you want to confirm the order?";
    }
    else if (toStatusId === OrderStatusEnum.Received) {
      statement = "Please enter comments!";
    }
    else if (toStatusId === OrderStatusEnum.Canceled) {
      statement = "Are you sure you want to cancel the order?";
    }
    const dialogRef = this.dialog.open(RetailOrderStatusChangeComponent, {
      data: { element: element, toStatusId: toStatusId, statement: statement },
      width: '40%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openConfirmQuantityDialog(element: any): void {
    const dialogRef = this.dialog.open(ConfirmRetailOrderQuantityComponent, {
      data: { element: element },
      width: '80%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openOrderCancelDialog(element: any, template: any): void {
    const dialogRef = this.dialog.open(template, {
      data: { element: element },
      width: '80%',
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
    if (element.retailOrderStatusId === OrderStatusEnum.Canceled) {
      return 'cancel';
    }
    else {
      return '';
    }
  }

  openRetailOrderHistoryDialog(element: any) {
    const dialogRef = this.dialog.open(RetailOrderHistoryComponent, {
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

  getOrderReceipt(element: any) {
    const dialogRef = this.dialog.open(PrintRetailOrderReceiptComponent, {
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

  openReceiveConfirmatinoDialog(element: any, template: any) {
    this.gElement = element;
    const dialogRef = this.dialog.open(template, {
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

  receiveStockConfirm(element: any, toStatusId: any) {

    this.openRetailOrderStatusDialog(element, toStatusId);

    // ------------------ BY PASSED DELIVERY CHALLAN CODE ----------------
    // if (this.receiveConfirmForm.get('deliveryChallanCode').value == this.gElement.dispatchOrderDetails[0].deliveryChallanCode) {
    //   this.openShopOrderStatusDialog(this.gElement, toStatusId);
    // }
    // else {
    //   this.notificationsService.showNotification('Please Enter Correct Delivery Challan Code!', 'snack-bar-danger');
    //   return;
    // }
  }

  // async getOrderReceipt(orderId: any, templateId: any, template: any) {
  //   (await this.templateService.getPrintTemplate(orderId, templateId)).subscribe({
  //     next: async (data) => {

  //       // const newTab = window.open('', '_blank');
  //       // var response = data.Data.replace(/\+/g, '%20');
  //       // // let printData = decodeURIComponent(response);
  //       // let user = JSON.parse(localStorage.getItem('currentUser'));
  //       // newTab.document.write(`
  //       //   <html>
  //       //     <head>
  //       //       <title>App Print</title>
  //       //       <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@3.3.7/dist/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">
  //       //     </head>
  //       //     <body>
  //       //     ${response}

  //       //     </body>
  //       //   </html>
  //       // `);
  //       // newTab.focus();

  //       this.innerHtml = data.Data as SafeHtml as string;
  //       this.dialogRefPrint = this.dialog.open(template, {
  //         width: '50%',
  //         minHeight: '500px',
  //         maxHeight: '90vh',
  //         disableClose: true,
  //       });

  //       this.dialogRefPrint.afterClosed().subscribe(() => {
  //       });
  //     },
  //     error: (error) => {
  //       console.log(error);
  //     }
  //   });
  // }

  // async print() {
  //   const WindowPrt = window.open('', '', 'left=0,top=0,width=1100,height=1100,toolbar=0,scrollbars=0,status=0');
  //   WindowPrt!.document.write(this.innerHtml);
  //   setTimeout(() => {
  //     WindowPrt!.focus();
  //     WindowPrt!.print();
  //     WindowPrt!.close();
  //   }, 1000);
  // }

  async getAllStatus() {
    (await this.orderService.getAllOrderStatus()).subscribe({
      next: (data) => {
        this.orderStatusList = data;
        this.isLoading = false;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  onReset() {
    const today = new Date(); // today date
    const lastWeek = new Date();
    lastWeek.setDate(today.getDate() - 7);
    this.shopOrderListFilerForm.get('fdate')?.patchValue(this.constantService.formatDate(lastWeek));
    this.shopOrderListFilerForm.get('tdate')?.patchValue(this.constantService.formatDate(today));

    this.shopOrderListFilerForm.patchValue({
      statusId: 0,
      regionId: 0,
      zoneId: 0,
      areaId: 0,
      territoryId: 0,
      shopId: 0,
    });
    this.bindData();
  }

  async getShopLedger(shopId: any, templateId: any, template: any) {

    (await this.templateService.getPrintTemplateByShopId(shopId, templateId)).subscribe({
      next: async (data) => {

        this.innerHtml = data.Data as SafeHtml as string;
        this.dialogRefPrint = this.dialog.open(template, {
          width: '50%',
          minHeight: '500px',
          maxHeight: '90vh',
          disableClose: true,
        });

        this.dialogRefPrint.afterClosed().subscribe(() => {
        });
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  async getTerritoryByAreaId() {

    this.territoryList = [];
    this.shopList = [];

    this.shopOrderListFilerForm.get('territoryId')?.patchValue(0);
    (await this.territoryService.getTerritoryByAreaId(this.shopOrderListFilerForm.get('areaId')?.value)).subscribe(
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
    this.shopList = [];

    (await this.zoneService.getZoneByRegionId(this.shopOrderListFilerForm.get('regionId')?.value)).subscribe({
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
    this.shopList = [];

    (await this.areaService.getAreaByZoneId(this.shopOrderListFilerForm.get('zoneId')?.value)).subscribe({
      next: (data) => {
        this.areaList = data;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}
