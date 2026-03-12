import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { AddIssuanceComponent } from '../add-issuance/add-issuance.component';
import { IssuanceService } from '../issuance.service';
import { IssuanceListComponent } from '../issuance-list/issuance-list.component';

@Component({
  selector: 'app-issuance-tab',
  templateUrl: './issuance-tab.component.html',
  styleUrls: ['./issuance-tab.component.css'],
  standalone: false
})

export class IssuanceTabComponent implements OnInit {
  issuanceFilterForm!: FormGroup;
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

  constructor(private issuanceService: IssuanceService, private notificationsService: NotificationsService, private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(IssuanceListComponent) issuanceListComponent!: IssuanceListComponent;
  async ngOnInit() {
    this.issuanceFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      statusId: [0]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.issuanceFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.issuanceFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.issuanceFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.issuanceFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

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
    await this.issuanceListComponent.bindData(this.lObjLeadsFilterForm, true);
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
    let _issuanceFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.issuanceFilterForm.get("statusId")?.patchValue(statusId);
    _issuanceFilterForm = Object.assign(_issuanceFilterForm, this.issuanceFilterForm.value);
    this.lObjLeadsFilterForm = _issuanceFilterForm;
    await this.getissuanceCount();
  }

  async getissuanceCount() {
    (this.issuanceService.getIndentRequestCount(this.lObjLeadsFilterForm)).subscribe(
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
    this.issuanceFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.issuanceFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.issuanceFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
    this.getissuanceCount();
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(0); // Change to the index of the tab you want to select
  }

  async openissuanceDialog(element: any) {
    const dialogRef = this.dialog.open(AddIssuanceComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.changeTabs(0); // Change to the index of the tab you want to select
      console.log(`Dialog result: ${result}`);
    });
  }


}