import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { AddPurchaseReturnComponent } from '../add-purchasereturn/add-purchasereturn.component';
import { PurchaseReturnService } from '../purchasereturn.service';
import { PurchaseReturnListComponent } from '../purchasereturn-list/purchasereturn-list.component';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { VendorService } from '../../vendor/vendor.service';

@Component({
  selector: 'app-purchasereturn-tab',
  templateUrl: './purchasereturn-tab.component.html',
  styleUrls: ['./purchasereturn-tab.component.css'],
  standalone: false
})

export class PurchaseReturnTabComponent implements OnInit {
  purchaseReturnFilterForm!: FormGroup;
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
  vendorList: any;
  CountCreated: number = 0;
  CountProcessed: number = 0;
  CountApproved: number = 0;

  constructor(private vendorService: VendorService, private purchaseReturnService: PurchaseReturnService, private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(PurchaseReturnListComponent) purchaseReturnListComponent!: PurchaseReturnListComponent;
  async ngOnInit() {
    this.purchaseReturnFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      vendorId: [0],
      vendorName: [''],
      statusId: [0]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.purchaseReturnFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.purchaseReturnFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.purchaseReturnFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.purchaseReturnFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

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
    await this.purchaseReturnListComponent.bindData(this.lObjLeadsFilterForm, this.selected.value, true);
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
    let _purchaseReturnFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.purchaseReturnFilterForm.get("statusId")?.patchValue(statusId);
    _purchaseReturnFilterForm = Object.assign(_purchaseReturnFilterForm, this.purchaseReturnFilterForm.value);
    this.lObjLeadsFilterForm = _purchaseReturnFilterForm;
    await this.getpurchaseReturnCount();
  }

  async getpurchaseReturnCount() {
    (this.purchaseReturnService.getPurchaseReturnCount(this.lObjLeadsFilterForm)).subscribe(
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
    this.purchaseReturnFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
      vendorId: 0,
      vendorName: ""
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.purchaseReturnFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.purchaseReturnFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
    this.getpurchaseReturnCount();
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(0); // Change to the index of the tab you want to select
  }

  async openpurchaseReturnDialog(element: any) {
    const dialogRef = this.dialog.open(AddPurchaseReturnComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.changeTabs(0); // Change to the index of the tab you want to select
      console.log(`Dialog result: ${result}`);
    });
  }

  async getVendorsList(event: any) {
    const filter = event.currentTarget.value;
    this.vendorList = [];
    (await this.vendorService.getVendorByName(filter)).subscribe(
      (data: any) => {
        this.vendorList = data || [];
      },
      (error: any) => {
        console.error('Error fetching distributor list:', error);
        this.vendorList = [];
      }
    );
  }

  onOptionVendorSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      return;
    }
    this.purchaseReturnFilterForm.get('vendorId')?.patchValue(selectedValue.id);
    this.purchaseReturnFilterForm.get('vendorName')?.patchValue(selectedValue.name + ' | ' + selectedValue.vendorType?.name + ' | ' + selectedValue.territory?.name + ' | ' + selectedValue.address);
    this.filterData();
  }

  onInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      this.purchaseReturnFilterForm.get('vendorId')?.patchValue(0);
      this.purchaseReturnFilterForm.get('vendorName')?.patchValue('');
      this.filterData();
    }
  }

  onInputClearedCode(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      this.purchaseReturnFilterForm.get('code')?.patchValue('');
      this.filterData();
    }
  }


}