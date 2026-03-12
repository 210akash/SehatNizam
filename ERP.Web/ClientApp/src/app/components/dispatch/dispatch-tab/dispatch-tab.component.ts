import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { DispatchListComponent } from '../dispatch-list/dispatch-list.component';
import { DispatchService } from '../dispatch.service';
import { AddDispatchComponent } from '../add-dispatch/add-dispatch.component';
import { PendingDispatchOrderListComponent } from '../pending-dispatch-order-list/pending-dispatch-order-list.component';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { DealershipService } from '../../order/dealership/dealership.service';
// import { AddCommentsComponent } from '../lead-history/add-comments/add-comments.component';
// import { ConfirmLeadsComponent } from '../confirm-dispatch/confirm-dispatch.component';

@Component({
  selector: 'app-dispatch-tab',
  templateUrl: './dispatch-tab.component.html',
  styleUrls: ['./dispatch-tab.component.css'],
  standalone: false
})

export class DispatchTabComponent implements OnInit {
  dispatchFilterForm!: FormGroup;
  dataSource: any;
  currentUser: any;
  roleList: string | undefined;
  isLoading = false;
  totalRows = 0;
  pageSize = 0;
  currentPage = 0;
  displayedColumns: string[] = ['code', 'name', 'phone', 'project', 'assignedUserName', 'lastContact', 'lastComments', 'statusId', 'actions'];
  dialogRef: any;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  statusTab: any = [];

  propertyTypeList: any;
  partyTypeList: any;
  projectsList: any;
  lObjLeadsFilterForm: any;
  gIsAll: boolean = false;

  CountCreated: number = 0;
  CountProcessed: number = 0;
  CountApproved: number = 0;
  dealershipList: any[] = [];

  constructor(private dispatchService: DispatchService, private notificationsService: NotificationsService,
    private dealershipService: DealershipService,
    private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(DispatchListComponent) dispatchListComponent!: DispatchListComponent;
  async ngOnInit() {
    this.dispatchFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      dealershipName: [''],
      dealershipId: [0],
      orderId: [''],
      statusId: [0]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.dispatchFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.dispatchFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(2024, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.dispatchFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.dispatchFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

    this.fillGridCount(1);
    // this.openPendingOrderListDialog();
  }

  tabs: any = [];
  selected: any = new FormControl(0);

  addTab() {
    this.generateRoleWiseTab();
    this.tabs = [];

    for (let i = 0; i < this.statusTab.length; i++) {
      const item = this.statusTab[i];
      this.tabs.push({ tabId: item.id, posessionStatusValues: item.label, isSelected: false, pCount: item.pCount, isAll: item.isAll });
    }
  }

  async filterData() {
    await this.fillGridCount(this.selected.value);
  }

  async changeTab(event: MatTabChangeEvent) {
    this.lObjLeadsFilterForm['statusId'] = this.statusTab[event.index].statusId;
    await this.dispatchListComponent.bindData(this.lObjLeadsFilterForm, this.selected.value, true);
  }

  generateRoleWiseTab() {
    this.statusTab = null;

    this.statusTab = [
      { label: 'New', id: 1, pCount: this.CountCreated, statusId: 1 },
      { label: 'Processed', id: 2, pCount: this.CountProcessed, statusId: 2 },
      { label: 'Approved', id: 3, pCount: this.CountApproved, statusId: 3 },
    ];
  }

  async fillGridCount(statusId: any) {
    this.isLoading = true;
    let _dispatchFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.dispatchFilterForm.get("statusId")?.patchValue(statusId);
    _dispatchFilterForm = Object.assign(_dispatchFilterForm, this.dispatchFilterForm.value);
    this.lObjLeadsFilterForm = _dispatchFilterForm;
    await this.getdispatchCount();
  }

  async getdispatchCount() {
    (this.dispatchService.getIndentRequestCount(this.lObjLeadsFilterForm)).subscribe(
      {
        next: async (data: any) => {
          this.CountCreated = data.item1;
          this.CountProcessed = data.item2;
          this.CountApproved = data.item3;
          this.addTab();
        }, error(error: any) {
          console.log(error);
        }
      });
  }

  resetForm() {
    this.dispatchFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
      dealershipName: "",
      dealershipId: 0,
      orderId: ''
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.dispatchFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.dispatchFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
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

  async opendispatchDialog(element: any) {
    const dialogRef = this.dialog.open(AddDispatchComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1400',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.changeTabs(0); // Change to the index of the tab you want to select
      console.log(`Dialog result: ${result}`);
    });
  }

  openPendingOrderListDialog() {
    this.dialog.open(PendingDispatchOrderListComponent, {
      panelClass: 'cstm_width_1400',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
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
      this.dispatchFilterForm.get('dealershipId')?.patchValue(0);
      this.filterData();
    }
  }

  onOptionDealershipSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      return;
    }
    this.dispatchFilterForm.get('dealershipId')?.patchValue(selectedValue.id);
    this.dispatchFilterForm.get('dealershipName')?.patchValue(selectedValue.name + ' | ' + selectedValue.address);
    this.filterData();
  }


}