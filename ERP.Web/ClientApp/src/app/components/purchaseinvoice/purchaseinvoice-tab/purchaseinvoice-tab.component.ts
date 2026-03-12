import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { PurchaseInvoiceListComponent } from '../purchaseinvoice-list/purchaseinvoice-list.component';
import { GRNService } from '../../grn/grn.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { VendorService } from '../../vendor/vendor.service';

@Component({
  selector: 'app-purchaseinvoice-tab',
  templateUrl: './purchaseinvoice-tab.component.html',
  styleUrls: ['./purchaseinvoice-tab.component.css'],
  standalone: false
})

export class PurchaseInvoiceTabComponent implements OnInit {
  purchaseinvoiceFilterForm!: FormGroup;
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
  CountProcessedAudit: number = 0;
  CountProcessedFinance: number = 0;
  CountApproved: number = 0;

  constructor(private vendorService: VendorService, private grnService: GRNService, private notificationsService: NotificationsService, private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(PurchaseInvoiceListComponent) purchaseinvoiceListComponent!: PurchaseInvoiceListComponent;
  async ngOnInit() {
    this.purchaseinvoiceFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      vendorId: [null],
      vendorName: [''],
      grnCode: [''],
      statusId: [1]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.purchaseinvoiceFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.purchaseinvoiceFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.purchaseinvoiceFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.purchaseinvoiceFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

    this.fillGridCount(1);
  }

  tabs: any = [];
  selected: any = new FormControl(1);

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
    await this.purchaseinvoiceListComponent.bindData(this.lObjLeadsFilterForm, this.selected.value, true);
  }

  generateRoleWiseTab() {
    this.statusTab = null;

    // Split the roleList into an array and remove any empty elements or spaces
    const roles = this.roleList?.split(',').map(role => role.trim().toLowerCase());
    //accounts assistant
    // Check if the necessary roles exist in the roleList
    if (roles?.includes('accounts manager') || roles?.includes('accounts assistant')) {
      this.statusTab = [
        { label: 'Pending', id: 1, pCount: this.CountCreated, statusId: 1 },
        { label: 'In Process (Audit)', id: 2, pCount: this.CountProcessedAudit, statusId: 2 },
        { label: 'In Process (Finance)', id: 3, pCount: this.CountProcessedFinance, statusId: 6 },
        { label: 'Approved', id: 4, pCount: this.CountApproved, statusId: 3 },
      ];
    }
    else if (roles?.includes('audit')) {
      this.statusTab = [
        { label: 'Pending', id: 1, pCount: this.CountCreated, statusId: 1 },
        { label: 'In Process (Audit)', id: 2, pCount: this.CountProcessedAudit, statusId: 2 },
        { label: 'In Process (Finance)', id: 3, pCount: this.CountProcessedFinance, statusId: 6 },
        { label: 'Approved', id: 4, pCount: this.CountApproved, statusId: 3 },
      ];
    }
    else {
      this.statusTab = [];
    }
  }

  async fillGridCount(statusId: any) {
    this.isLoading = true;
    let _purchaseinvoiceFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.purchaseinvoiceFilterForm.get("statusId")?.patchValue(statusId);
    _purchaseinvoiceFilterForm = Object.assign(_purchaseinvoiceFilterForm, this.purchaseinvoiceFilterForm.value);
    this.lObjLeadsFilterForm = _purchaseinvoiceFilterForm;
    await this.getpurchaseinvoiceCount();
  }

  async getpurchaseinvoiceCount() {
    (this.grnService.getPurchaseInvoiceCount(this.lObjLeadsFilterForm)).subscribe(
      {
        next: async (data: any) => {
          this.CountCreated = data.item1;
          this.CountProcessedAudit = data.item2;
          this.CountProcessedFinance = data.item3;
          this.CountApproved = data.item4;
          this.addTab();
        }, error(error: any) {
          console.log(error);
        }
      });
  }

  resetForm() {
    this.purchaseinvoiceFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.purchaseinvoiceFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.purchaseinvoiceFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
    this.getpurchaseinvoiceCount();
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(0); // Change to the index of the tab you want to select
  }


  async getSupplierList(event: any) {
    // Clone the form value and add paging data
    const filter = event.currentTarget.value;
    this.vendorList = [];  // Empty the list before updating
    (await this.vendorService.getVendorByName(filter)).subscribe(
      (data: any) => {
        this.vendorList = data || []; // Ensure it's an array even if no data is returned
      },
      (error: any) => {
        console.error('Error fetching account list:', error);
        this.vendorList = [];  // Reset in case of an error
      }
    );
  }

  onOptionSupplierSelected(event: MatAutocompleteSelectedEvent): void {

    const selectedValue = event.option.value;
    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      return;
    }
    this.purchaseinvoiceFilterForm.get('vendorId')?.patchValue(selectedValue.id);
    this.purchaseinvoiceFilterForm.get('vendorName')?.patchValue(selectedValue.name);
    this.filterData();
  }

  onInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      this.purchaseinvoiceFilterForm.get('vendorId')?.patchValue(null);
      this.filterData();
    }
  }

  onInputClearedCode(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      this.purchaseinvoiceFilterForm.get('code')?.patchValue('');
      this.filterData();
    }
  }


}
