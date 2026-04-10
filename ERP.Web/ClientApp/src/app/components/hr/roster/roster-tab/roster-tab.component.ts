import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { RosterListComponent } from '../roster-list/roster-list.component';
import { AddRosterComponent } from '../add-roster/add-roster.component';
import { RosterService } from '../roster.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { DepartmentService } from '../../../department/department.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-roster-tab',
  templateUrl: './roster-tab.component.html',
  styleUrls: ['./roster-tab.component.css'],
  standalone: false
})

export class RosterTabComponent implements OnInit {
  RosterFilterForm!: FormGroup;
  dataSource: any;
  currentUser: any;
  roleList: string | undefined;
  isLoading = false;
  totalRows = 0;
  pageSize = 0;
  currentPage = 0;
  dialogRef: any;
  statusTab: any = [];
  productList: any;
  propertyTypeList: any;
  partyTypeList: any;
  projectsList: any;
  lObjLeadsFilterForm: any;
  gIsAll: boolean = false;
  departmentList: any;
  CountCreated: number = 0;
  CountProcessed: number = 0;
  CountApproved: number = 0;
  CountIssued: number = 0;
  years: number[] = [];
  months = [
    { value: 1, label: 'January' },
    { value: 2, label: 'February' },
    { value: 3, label: 'March' },
    { value: 4, label: 'April' },
    { value: 5, label: 'May' },
    { value: 6, label: 'June' },
    { value: 7, label: 'July' },
    { value: 8, label: 'August' },
    { value: 9, label: 'September' },
    { value: 10, label: 'October' },
    { value: 11, label: 'November' },
    { value: 12, label: 'December' },
  ];
  constructor(private router: Router, private departmentService: DepartmentService, private rosterService: RosterService, private notificationsService: NotificationsService, private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(RosterListComponent) rosterListComponent!: RosterListComponent;
  async ngOnInit() {
    this.RosterFilterForm = this.formBuilder.group({
      year: [2026],
      month: [5],
      departmentId: [26],
      statusId: [1]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();
    this.getDepartmentList();
    this.buildYears();
    this.fillGridCount(0);
  }

  tabs: any = [];
  selected: any = new FormControl(1);

  buildYears(): void {
    const current = new Date().getFullYear();
    for (let y = current; y <= current + 1; y++) {
      this.years.push(y);
    }
  }



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
    await this.rosterListComponent.bindData(this.lObjLeadsFilterForm, this.selected.value, true);
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
    let _RosterFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.RosterFilterForm.get("statusId")?.patchValue(statusId);
    _RosterFilterForm = Object.assign(_RosterFilterForm, this.RosterFilterForm.value);
    this.lObjLeadsFilterForm = _RosterFilterForm;
    await this.getRosterCount();
  }

  async getRosterCount() {
    (this.rosterService.getRosterCount(this.lObjLeadsFilterForm)).subscribe(
      {
        next: async (data: any) => {
          this.CountCreated = data.item1;
          this.CountProcessed = data.item2;
          this.CountApproved = data.item3;
          this.CountIssued = data.item4;
          this.addTab();
        }, error(error: any) {
          console.log(error);
        }
      });
  }

  resetForm() {
    this.RosterFilterForm.reset({
      // code: "",
      // fdate: new Date(),
      // tdate: new Date(),
    });

    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
    this.getRosterCount();
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(0); // Change to the index of the tab you want to select
  }

  openRosterDialog(element: any) {
    // Open the appointment form as a full page instead of a dialog.
    const navigationExtras = element ? { state: { element } } : undefined;
    this.router.navigate(['/addroster'], navigationExtras);
  }

  getDepartmentList(): void {
    let _departmentsForm: any = {};
    this.departmentService.getAllDepartments(_departmentsForm).subscribe(data => {
      this.departmentList = data.item1;
    });
  }
}