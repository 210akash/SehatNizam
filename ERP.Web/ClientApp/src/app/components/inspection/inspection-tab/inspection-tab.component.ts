import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { AddInspectionComponent } from '../add-inspection/add-inspection.component';
import { InspectionService } from '../inspection.service';
import { InspectionListComponent } from '../inspection-list/inspection-list.component';

@Component({
  selector: 'app-inspection-tab',
  templateUrl: './inspection-tab.component.html',
  styleUrls: ['./inspection-tab.component.css'],
  standalone: false
})

export class InspectionTabComponent implements OnInit {
  inspectionFilterForm!: FormGroup;
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

  constructor(private inspectionService: InspectionService, private notificationsService: NotificationsService, private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(InspectionListComponent) inspectionListComponent!: InspectionListComponent;
  async ngOnInit() {
    this.inspectionFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      statusId: [0]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.inspectionFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.inspectionFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.inspectionFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.inspectionFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

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
    await this.inspectionListComponent.bindData(this.lObjLeadsFilterForm, this.selected.value, true);
  }

  generateRoleWiseTab() {
    this.statusTab = null;

    // Split the roleList into an array and remove any empty elements or spaces
    const roles = this.roleList?.split(',').map(role => role.trim().toLowerCase());

    // Check if the necessary roles exist in the roleList

    this.statusTab = [
      { label: 'New', id: 1, pCount: this.CountCreated, statusId: 1 },
      { label: 'Post', id: 3, pCount: this.CountApproved, statusId: 3 },
    ];

  }

  async fillGridCount(statusId: any) {
    this.isLoading = true;
    let _inspectionFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.inspectionFilterForm.get("statusId")?.patchValue(statusId);
    _inspectionFilterForm = Object.assign(_inspectionFilterForm, this.inspectionFilterForm.value);
    this.lObjLeadsFilterForm = _inspectionFilterForm;
    await this.getinspectionCount();
  }

  async getinspectionCount() {
    (this.inspectionService.getInspectionCount(this.lObjLeadsFilterForm)).subscribe(
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
    this.inspectionFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.inspectionFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.inspectionFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
    this.getinspectionCount();
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(0); // Change to the index of the tab you want to select
  }

  async openinspectionDialog(element: any) {
    const dialogRef = this.dialog.open(AddInspectionComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1200',
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