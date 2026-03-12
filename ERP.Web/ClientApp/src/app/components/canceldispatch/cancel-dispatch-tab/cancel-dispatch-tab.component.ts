import { Component, ViewChild } from '@angular/core';
import { DispatchService } from '../../dispatch/dispatch.service';
import { NotificationsService } from '../../../Service/notification.service';
import { MatDialog } from '@angular/material/dialog';
import {
  ConstantService,
  OrderStatusEnum,
} from '../../../Service/constant.service';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { CancelDispatchListComponent } from '../cancel-dispatch-list/cancel-dispatch-list.component';
import { PendingDispatchOrderListComponent } from '../../dispatch/pending-dispatch-order-list/pending-dispatch-order-list.component';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { AddCancelDispatchComponent } from '../add-cancel-dispatch/add-cancel-dispatch.component';
import { CancelDispatchService } from '../canceldispatch.service';

@Component({
  selector: 'app-cancel-dispatch-tab',
  templateUrl: './cancel-dispatch-tab.component.html',
  styleUrl: './cancel-dispatch-tab.component.css',
  standalone: false,
})
export class CancelDispatchTabComponent {
  dispatchFilterForm!: FormGroup;
  dataSource: any;
  currentUser: any;
  roleList: string | undefined;
  isLoading = false;
  totalRows = 0;
  pageSize = 0;
  currentPage = 0;
  displayedColumns: string[] = [
    'code',
    'name',
    'phone',
    'project',
    'assignedUserName',
    'lastContact',
    'lastComments',
    'statusId',
    'actions',
  ];
  dialogRef: any;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  statusTab: any = [];

  propertyTypeList: any;
  partyTypeList: any;
  projectsList: any;
  lObjLeadsFilterForm: any;
  gIsAll: boolean = false;

  CountCreated: number = 0;
  CountForward: number = 0;
  CountSale: number = 0;
  CountAccount: number = 0;
  CountConfirm: number = 0;

  constructor(
    private cancelDispatchService: CancelDispatchService,
    private notificationsService: NotificationsService,
    private dialog: MatDialog,
    private constantService: ConstantService,
    private formBuilder: FormBuilder
  ) { }
  @ViewChild(CancelDispatchListComponent)
  cancelDispatchListComponent!: CancelDispatchListComponent;
  async ngOnInit() {
    this.dispatchFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      statusId: [110],
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.dispatchFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.dispatchFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.dispatchFilterForm
      .get('fdate')
      ?.patchValue(this.constantService.formatDate(startDate));
    this.dispatchFilterForm
      .get('tdate')
      ?.patchValue(this.constantService.formatDate(endDate));

    this.fillGridCount(110);
  }

  tabs: any = [];
  selected: any = new FormControl(0);

  addTab() {
    this.generateRoleWiseTab();
    this.tabs = [];

    for (let i = 0; i < this.statusTab.length; i++) {
      const item = this.statusTab[i];
      this.tabs.push({
        tabId: item.id,
        posessionStatusValues: item.label,
        isSelected: false,
        pCount: item.pCount,
        isAll: item.isAll,
      });
    }
  }

  async filterData() {
    await this.fillGridCount(this.selected.value);
  }

  async changeTab(event: MatTabChangeEvent) {
    this.lObjLeadsFilterForm['statusId'] = this.statusTab[event.index].statusId;
    await this.cancelDispatchListComponent.bindData(this.lObjLeadsFilterForm, true);
  }
  generateRoleWiseTab() {
    // Initialize statusTab as empty array
    this.statusTab = [];

    // Normalize roles into an array or use empty array if undefined
    const roles: string[] = this.roleList
      ?.split(',')
      .map((role) => role.trim().toLowerCase()) || [];

    // Utility function to check if user has any of the target roles
    const hasAnyRole = (targetRoles: string[]) =>
      targetRoles.some((role) => roles.includes(role.toLowerCase()));

    // Utility function to check if user has all of the target roles
    const hasAllRoles = (targetRoles: string[]) =>
      targetRoles.every((role) => roles.includes(role.toLowerCase()));

    // Append tabs based on role matches
    if (hasAnyRole(['store issuer', 'store manager'])) {
      this.statusTab.push({
        label: 'Created',
        id: 1,
        pCount: this.CountCreated,
        statusId: OrderStatusEnum.CancelDispatchCreated,
      });
    }

    if (hasAnyRole(['store issuer', 'store manager', 'sales'])) {
      this.statusTab.push({
        label: 'In Process',
        id: 2,
        pCount: this.CountForward,
        statusId: OrderStatusEnum.CancelDispatchForward,
      });
    }

    if (hasAnyRole(['store issuer', 'store manager', 'sales', 'accounts assistant', 'store manager'])) {
      this.statusTab.push({
        label: 'Sales Reviewed',
        id: 3,
        pCount: this.CountSale,
        statusId: OrderStatusEnum.CancelDispatchSalesReviewed,
      });
    }

    if (hasAnyRole(['store issuer', 'store manager', 'sales', 'accounts assistant', 'store manager', 'accounts manager'])) {
      this.statusTab.push(
        {
          label: 'Account Reviewed',
          id: 4,
          pCount: this.CountAccount,
          statusId: OrderStatusEnum.CancelDispatchAccountReviewed,
        },
        {
          label: 'Account Confirmed',
          id: 5,
          pCount: this.CountConfirm,
          statusId: OrderStatusEnum.CancelDispatchConfirm,
        }
      );
    }
    console.log(this.statusTab);

    // Optional: If nothing matched, ensure statusTab is still defined as an array
    // (already initialized as [] at the start)
  }

  async fillGridCount(statusId: any) {
    this.isLoading = true;
    let _dispatchFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.dispatchFilterForm.get('statusId')?.patchValue(statusId);
    _dispatchFilterForm = Object.assign(
      _dispatchFilterForm,
      this.dispatchFilterForm.value
    );
    this.lObjLeadsFilterForm = _dispatchFilterForm;
    await this.getdispatchCount();
  }

  async getdispatchCount() {
    this.cancelDispatchService
      .getCancelDispatchCount(this.lObjLeadsFilterForm)
      .subscribe({
        next: async (data: any) => {
          this.CountCreated = data.item1;
          this.CountForward = data.item2;
          this.CountSale = data.item3;
          this.CountAccount = data.item4;
          this.CountConfirm = data.item5;
          this.addTab();
        },
        error(error: any) {
          console.log(error);
        },
      });
  }

  resetForm() {
    this.dispatchFilterForm.reset({
      code: '',
      fdate: new Date(),
      tdate: new Date(),
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.dispatchFilterForm
      .get('fdate')
      ?.patchValue(this.constantService.formatDate(startDate));
    this.dispatchFilterForm
      .get('tdate')
      ?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
    this.getdispatchCount();
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(1); // Change to the index of the tab you want to select
  }

  async openAddCancelDispatchDialog(element: any) {
    const dialogRef = this.dialog.open(AddCancelDispatchComponent, {
      data: { element: element },
      maxHeight: '90vh',
      width: '70%',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.changeTabs(0); // Change to the index of the tab you want to select
      console.log(`Dialog result: ${result}`);
    });
  }

  openPendingOrderListDialog() {
    this.dialog.open(PendingDispatchOrderListComponent, {
      maxHeight: '90vh',
      width: '60%',
      disableClose: true,
    });
  }


}
