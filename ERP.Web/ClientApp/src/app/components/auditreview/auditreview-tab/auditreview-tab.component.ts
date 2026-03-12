import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService, OrderStatusEnum } from '../../../Service/constant.service';
import { AuditReviewService } from '../auditreview.service';
import { AuditReviewListComponent } from '../auditreview-list/auditreview-list.component';
import { DealershipService } from '../../order/dealership/dealership.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'app-auditreview-tab',
  templateUrl: './auditreview-tab.component.html',
  styleUrls: ['./auditreview-tab.component.css'],
  standalone: false
})

export class AuditReviewTabComponent implements OnInit {
  auditreviewFilterForm!: FormGroup;
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
  CountManagerApproved: number = 0;
  CountApproved: number = 0;
  statusEnum: any;
  dealershipList: any[] = [];
  constructor(private auditreviewService: AuditReviewService, private notificationsService: NotificationsService, private dialog: MatDialog,
    private dealershipService: DealershipService,
    private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(AuditReviewListComponent) auditreviewListComponent!: AuditReviewListComponent;
  async ngOnInit() {
    this.statusEnum = OrderStatusEnum;
    this.auditreviewFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      dealershipName: [''],
      dealershipId: [0],
      statusId: [this.statusEnum.InProcess]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.auditreviewFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.auditreviewFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.auditreviewFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.auditreviewFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

    this.fillGridCount(this.statusEnum.InProcess);
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
    this.currentPage = 0;
    await this.fillGridCount(this.selected.value);
  }

  async changeTab(event: MatTabChangeEvent) {
    this.lObjLeadsFilterForm['statusId'] = this.statusTab[event.index].statusId;
    await this.auditreviewListComponent.bindData(this.lObjLeadsFilterForm, this.selected.value, true);
  }

  generateRoleWiseTab() {
    this.statusTab = null;

    // Split the roleList into an array and remove any empty elements or spaces
    const roles = this.roleList?.split(',').map(role => role.trim().toLowerCase());

    // Check if the necessary roles exist in the roleList
    if (roles?.includes('accounts manager')) {
      this.statusTab = [
        { label: 'In Process', id: 1, pCount: this.CountCreated, statusId: this.statusEnum.InProcess },
        { label: 'Account Reviewed (Accounts)', id: 2, pCount: this.CountProcessed, statusId: this.statusEnum.AccountReviewed },
        { label: 'Manager Approved (Audit)', id: 3, pCount: this.CountManagerApproved, statusId: this.statusEnum.ManagerApproved },
        { label: 'Order Confirmed', id: 4, pCount: this.CountApproved, statusId: this.statusEnum.Confirm },
      ];
    } else if (roles?.includes('accounts assistant')) {
      this.statusTab = [
        { label: 'In Process', id: 1, pCount: this.CountCreated, statusId: this.statusEnum.InProcess },
        { label: 'Account Reviewed (Accounts)', id: 2, pCount: this.CountProcessed, statusId: this.statusEnum.AccountReviewed },
        { label: 'Manager Approved (Audit)', id: 3, pCount: this.CountManagerApproved, statusId: this.statusEnum.ManagerApproved },
        { label: 'Order Confirmed', id: 4, pCount: this.CountApproved, statusId: this.statusEnum.Confirm },
      ];
    }
    else if (roles?.includes('audit')) {
      this.statusTab = [
        { label: 'In Process', id: 1, pCount: this.CountCreated, statusId: this.statusEnum.InProcess },
        { label: 'Account Reviewed', id: 2, pCount: this.CountProcessed, statusId: this.statusEnum.AccountReviewed },
        { label: 'Manager Approved', id: 3, pCount: this.CountManagerApproved, statusId: this.statusEnum.ManagerApproved },
        { label: 'Order Confirmed', id: 4, pCount: this.CountApproved, statusId: this.statusEnum.Confirm },
      ];
    }
    else {
      this.statusTab = [];
    }
  }



  async fillGridCount(statusId: any) {
    this.isLoading = true;
    let _auditreviewFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.auditreviewFilterForm.get("statusId")?.patchValue(statusId);
    _auditreviewFilterForm = Object.assign(_auditreviewFilterForm, this.auditreviewFilterForm.value);
    this.lObjLeadsFilterForm = _auditreviewFilterForm;
    await this.getauditreviewCount();
  }

  async getauditreviewCount() {
    (this.auditreviewService.getAuditReviewCount(this.lObjLeadsFilterForm)).subscribe(
      {
        next: async (data: any) => {
          this.CountCreated = data.item1;
          this.CountProcessed = data.item2;
          this.CountManagerApproved = data.item3;
          this.CountApproved = data.item4;
          this.addTab();
        }, error(error: any) {
          console.log(error);
        }
      });
  }

  resetForm() {
    this.auditreviewFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.auditreviewFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.auditreviewFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(0); // Change to the index of the tab you want to select
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
      this.auditreviewFilterForm.get('dealershipId')?.patchValue(0);
      this.filterData();
    }
  }

  onOptionDealershipSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      return;
    }
    this.auditreviewFilterForm.get('dealershipId')?.patchValue(selectedValue.id);
    this.auditreviewFilterForm.get('dealershipName')?.patchValue(selectedValue.name + ' | ' + selectedValue.address);
    this.filterData();
  }


}