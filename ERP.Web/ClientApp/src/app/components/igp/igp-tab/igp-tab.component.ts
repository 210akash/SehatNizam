import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { AddIGPComponent } from '../add-igp/add-igp.component';
import { IGPService } from '../igp.service';
import { IGPListComponent } from '../igp-list/igp-list.component';
import { VendorService } from '../../vendor/vendor.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'app-igp-tab',
  templateUrl: './igp-tab.component.html',
  styleUrls: ['./igp-tab.component.css'],
  standalone: false
})

export class IGPTabComponent implements OnInit {
  iGPFilterForm!: FormGroup;
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

  constructor(private vendorService: VendorService, private iGPService: IGPService, private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(IGPListComponent) iGPListComponent!: IGPListComponent;
  async ngOnInit() {
    this.iGPFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      vendorId: [null],
      vendorName: [''],
      statusId: [0]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.iGPFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.iGPFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.iGPFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.iGPFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

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
    await this.iGPListComponent.bindData(this.lObjLeadsFilterForm, this.selected.value, true);
  }

  generateRoleWiseTab() {
    this.statusTab = null;
    // Split the roleList into an array and remove any empty elements or spaces
    const roles = this.roleList?.split(',').map(role => role.trim().toLowerCase());
    // Check if the necessary roles exist in the roleList
    this.statusTab = [
      { label: 'New', id: 1, pCount: this.CountCreated, statusId: 1 },
      { label: 'Posted', id: 3, pCount: this.CountApproved, statusId: 3 },
    ];
  }

  async fillGridCount(statusId: any) {
    this.isLoading = true;
    let _iGPFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.iGPFilterForm.get("statusId")?.patchValue(statusId);
    _iGPFilterForm = Object.assign(_iGPFilterForm, this.iGPFilterForm.value);
    this.lObjLeadsFilterForm = _iGPFilterForm;
    await this.getiGPCount();
  }

  async getiGPCount() {
    (this.iGPService.getIndentRequestCount(this.lObjLeadsFilterForm)).subscribe(
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
    this.iGPFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.iGPFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.iGPFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
    this.getiGPCount();
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(0); // Change to the index of the tab you want to select
  }

  async openiGPDialog(element: any) {
    const dialogRef = this.dialog.open(AddIGPComponent, {
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
    this.iGPFilterForm.get('vendorId')?.patchValue(selectedValue.id);
    this.iGPFilterForm.get('vendorName')?.patchValue(selectedValue.name);
    this.filterData();
  }

  onInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      this.iGPFilterForm.get('vendorId')?.patchValue(null);
      this.filterData();
    }
  }

  onInputClearedCode(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      this.iGPFilterForm.get('code')?.patchValue('');
      this.filterData();
    }
  }


}
