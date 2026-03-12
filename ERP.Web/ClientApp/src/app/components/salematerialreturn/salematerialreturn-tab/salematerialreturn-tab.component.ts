import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { AddSaleMaterialReturnComponent } from '../add-salematerialreturn/add-salematerialreturn.component';
import { SaleMaterialReturnService } from '../salematerialreturn.service';
import { SaleMaterialReturnListComponent } from '../salematerialreturn-list/salematerialreturn-list.component';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { DealershipService } from '../../order/dealership/dealership.service';

@Component({
  selector: 'app-salematerialreturn-tab',
  templateUrl: './salematerialreturn-tab.component.html',
  styleUrls: ['./salematerialreturn-tab.component.css'],
  standalone: false
})

export class SaleMaterialReturnTabComponent implements OnInit {
  saleMaterialReturnFilterForm!: FormGroup;
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
  distributorList: any;
  CountCreated: number = 0;
  CountProcessed: number = 0;
  CountApproved: number = 0;

  constructor(private dealershipService: DealershipService, private saleMaterialReturnService: SaleMaterialReturnService, private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(SaleMaterialReturnListComponent) saleMaterialReturnListComponent!: SaleMaterialReturnListComponent;
  async ngOnInit() {
    this.saleMaterialReturnFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      dealershipId: [0],
      dealershipName: [''],
      statusId: [0]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.saleMaterialReturnFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.saleMaterialReturnFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.saleMaterialReturnFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.saleMaterialReturnFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

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
    await this.saleMaterialReturnListComponent.bindData(this.lObjLeadsFilterForm, this.selected.value, true);
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
    let _saleMaterialReturnFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.saleMaterialReturnFilterForm.get("statusId")?.patchValue(statusId);
    _saleMaterialReturnFilterForm = Object.assign(_saleMaterialReturnFilterForm, this.saleMaterialReturnFilterForm.value);
    this.lObjLeadsFilterForm = _saleMaterialReturnFilterForm;
    await this.getsaleMaterialReturnCount();
  }

  async getsaleMaterialReturnCount() {
    (this.saleMaterialReturnService.getSaleMaterialReturnCount(this.lObjLeadsFilterForm)).subscribe(
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
    this.saleMaterialReturnFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
      dealershipId: 0,
      dealershipName: ""
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.saleMaterialReturnFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.saleMaterialReturnFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
    this.getsaleMaterialReturnCount();
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(0); // Change to the index of the tab you want to select
  }

  async opensaleMaterialReturnDialog(element: any) {
    const dialogRef = this.dialog.open(AddSaleMaterialReturnComponent, {
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

  async getCustomersList(event: any) {
    const filter = event.currentTarget.value;
    this.distributorList = []; // Empty the list before updating
    (await this.dealershipService.getCustomerByName(filter)).subscribe(
      (data: any) => {

        this.distributorList = data || []; // Ensure it's an array even if no data is returned
      },
      (error: any) => {
        console.error('Error fetching distributor list:', error);
        this.distributorList = []; // Reset in case of an error
      }
    );
  }

  onOptionDealershipSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      return;
    }
    this.saleMaterialReturnFilterForm.get('dealershipId')?.patchValue(selectedValue.id);
    this.saleMaterialReturnFilterForm.get('dealershipName')?.patchValue(selectedValue.name + ' | ' + selectedValue.dealershipType?.name + ' | ' + selectedValue.territory?.name + ' | ' + selectedValue.address);
    this.filterData();
  }

  onInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      this.saleMaterialReturnFilterForm.get('dealershipId')?.patchValue(0);
      this.saleMaterialReturnFilterForm.get('dealershipName')?.patchValue('');
      this.filterData();
    }
  }

  onInputClearedCode(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      this.saleMaterialReturnFilterForm.get('code')?.patchValue('');
      this.filterData();
    }
  }


}
