import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { AddShopOrderReturnComponent } from '../add-shoporderreturn/add-shoporderreturn.component';
import { ShopOrderReturnService } from '../shoporderreturn.service';
import { ShopOrderReturnListComponent } from '../shoporderreturn-list/shoporderreturn-list.component';
import { VendorService } from '../../vendor/vendor.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { DealershipService } from '../../order/dealership/dealership.service';

@Component({
  selector: 'app-shoporderreturn-tab',
  templateUrl: './shoporderreturn-tab.component.html',
  styleUrls: ['./shoporderreturn-tab.component.css'],
  standalone: false
})

export class ShopOrderReturnTabComponent implements OnInit {
  shopOrderReturnFilterForm!: FormGroup;
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
  dealershipList: any;
  CountCreated: number = 0;
  CountProcessed: number = 0;
  CountApproved: number = 0;

  constructor(private dealershipService: DealershipService, private shopOrderReturnService: ShopOrderReturnService, private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(ShopOrderReturnListComponent) shopOrderReturnListComponent!: ShopOrderReturnListComponent;
  async ngOnInit() {
    this.shopOrderReturnFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      orderId: [''],
      statusId: [0]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.shopOrderReturnFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.shopOrderReturnFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.shopOrderReturnFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.shopOrderReturnFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

    this.fillGridCount(0);
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
    await this.shopOrderReturnListComponent.bindData(this.lObjLeadsFilterForm, this.selected.value, true);
  }

  generateRoleWiseTab() {
    this.statusTab = null;

    this.statusTab = [
      { label: 'New', id: 1, pCount: this.CountCreated, statusId: 1 },
      { label: 'Posted', id: 2, pCount: this.CountProcessed, statusId: 3 },
    ];
  }

  async fillGridCount(statusId: any) {
    this.isLoading = true;
    let _shopOrderReturnFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.shopOrderReturnFilterForm.get("statusId")?.patchValue(statusId);
    _shopOrderReturnFilterForm = Object.assign(_shopOrderReturnFilterForm, this.shopOrderReturnFilterForm.value);
    this.lObjLeadsFilterForm = _shopOrderReturnFilterForm;
    await this.getshopOrderReturnCount();
  }

  async getshopOrderReturnCount() {
    (this.shopOrderReturnService.getShopOrderReturnCount(this.lObjLeadsFilterForm)).subscribe(
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
    this.shopOrderReturnFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
      dealershipId: 0,
      dealershipName: ""
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.shopOrderReturnFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.shopOrderReturnFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
    this.getshopOrderReturnCount();
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(0); // Change to the index of the tab you want to select
  }

  async openshopOrderReturnDialog(element: any) {
    const dialogRef = this.dialog.open(AddShopOrderReturnComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1000',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.changeTabs(0); // Change to the index of the tab you want to select
      console.log(`Dialog result: ${result}`);
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

  onOptionDealershipSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      return;
    }
    this.shopOrderReturnFilterForm.get('dealershipId')?.patchValue(selectedValue.id);
    this.shopOrderReturnFilterForm.get('dealershipName')?.patchValue(selectedValue.name + ' | ' + selectedValue.dealershipType?.name + ' | ' + selectedValue.territory?.name + ' | ' + selectedValue.address);
    this.filterData();
  }

  onInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      this.shopOrderReturnFilterForm.get('dealershipId')?.patchValue(0);
      this.shopOrderReturnFilterForm.get('dealershipName')?.patchValue('');
      this.filterData();
    }
  }

  onInputClearedCode(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      this.shopOrderReturnFilterForm.get('code')?.patchValue('');
      this.filterData();
    }
  }


}