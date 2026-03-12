import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { AddGRNComponent } from '../add-grn/add-grn.component';
import { GRNService } from '../grn.service';
import { GRNListComponent } from '../grn-list/grn-list.component';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { VendorService } from '../../vendor/vendor.service';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-grn-tab',
  templateUrl: './grn-tab.component.html',
  styleUrls: ['./grn-tab.component.css'],
  standalone: false
})

export class GRNTabComponent implements OnInit {
  grnFilterForm!: FormGroup;
  dataSource = new MatTableDataSource<any>();
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

  constructor(private vendorService: VendorService, private grnService: GRNService, private notificationsService: NotificationsService, private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(GRNListComponent) grnListComponent!: GRNListComponent;
  async ngOnInit() {
    this.grnFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      vendorId: [null],
      vendorName: [''],
      statusId: [0]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.grnFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.grnFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.grnFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.grnFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

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
    await this.grnListComponent.bindData(this.lObjLeadsFilterForm, this.selected.value, true);
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
    let _grnFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.grnFilterForm.get("statusId")?.patchValue(statusId);
    _grnFilterForm = Object.assign(_grnFilterForm, this.grnFilterForm.value);
    this.lObjLeadsFilterForm = _grnFilterForm;
    await this.getgrnCount();
  }

  async getgrnCount() {
    (this.grnService.getIndentRequestCount(this.lObjLeadsFilterForm)).subscribe(
      {
        next: async (data: any) => {
          this.CountCreated = data.item1;
          this.CountProcessed = data.item2;
          this.CountApproved = data.item3;
          this.addTab(); // ensure this doesn’t re-render prematurely
        }, error(error: any) {
          console.log(error);
        }
      });
  }

  resetForm() {
    this.grnFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.grnFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.grnFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(0); // Change to the index of the tab you want to select
  }

  async opengrnDialog(element: any) {
    const dialogRef = this.dialog.open(AddGRNComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.changeTabs(1); // Change to the index of the tab you want to select
      console.log(`Dialog result: ${result}`);
    });
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
    this.grnFilterForm.get('vendorId')?.patchValue(selectedValue.id);
    this.grnFilterForm.get('vendorName')?.patchValue(selectedValue.name);
    this.filterData();
  }

  onInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      this.grnFilterForm.get('vendorId')?.patchValue(null);
      this.filterData();
    }
  }

  onInputClearedCode(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      this.grnFilterForm.get('code')?.patchValue('');
      this.filterData();
    }
  }


}