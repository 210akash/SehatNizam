import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { AddWarehouseTransferComponent } from '../add-warehousetransfer/add-warehousetransfer.component';
import { WarehouseTransferService } from '../warehousetransfer.service';
import { WarehouseTransferListComponent } from '../warehousetransfer-list/warehousetransfer-list.component';

@Component({
  selector: 'app-warehousetransfer-tab',
  templateUrl: './warehousetransfer-tab.component.html',
  styleUrls: ['./warehousetransfer-tab.component.css'],
  standalone: false
})

export class WarehouseTransferTabComponent implements OnInit {
  warehousetransferFilterForm!: FormGroup;
  dataSource: any;
  currentUser: any;
  roleList: string | undefined;
  isLoading = false;
  totalRows = 0;
  pageSize = 0;
  currentPage = 0;
  dialogRef: any;
  statusTab: any = [];

  propertyTypeList: any;
  partyTypeList: any;
  projectsList: any;
  lObjLeadsFilterForm: any;
  gIsAll: boolean = false;

  CountCreated: number = 0;
  CountProcessed: number = 0;
  CountApproved: number = 0;
  CountIssued: number = 0;

  constructor(private warehousetransferService: WarehouseTransferService, private notificationsService: NotificationsService, private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(WarehouseTransferListComponent) warehousetransferListComponent!: WarehouseTransferListComponent;
  async ngOnInit() {
    this.warehousetransferFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      statusId: [0]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.CostsheetFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.CostsheetFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.warehousetransferFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.warehousetransferFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

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
    await this.warehousetransferListComponent.bindData(this.lObjLeadsFilterForm, this.selected.value, false);
  }
  generateRoleWiseTab() {
    this.statusTab = null;

    this.statusTab = [
      { label: 'New', id: 1, pCount: this.CountCreated, statusId: 1 },
      { label: 'Processed', id: 2, pCount: this.CountProcessed, statusId: 2 },
      { label: 'Received', id: 3, pCount: this.CountApproved, statusId: 3 },
    ];
  }

  async fillGridCount(statusId: any) {
    this.isLoading = true;
    let _warehousetransferFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.warehousetransferFilterForm.get("statusId")?.patchValue(statusId);
    _warehousetransferFilterForm = Object.assign(_warehousetransferFilterForm, this.warehousetransferFilterForm.value);
    this.lObjLeadsFilterForm = _warehousetransferFilterForm;
    await this.getWarehouseTransferCount();
  }

  async getWarehouseTransferCount() {
    (this.warehousetransferService.getWarehouseTransferCount(this.lObjLeadsFilterForm)).subscribe(
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
    this.warehousetransferFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.warehousetransferFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.warehousetransferFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
    this.getWarehouseTransferCount();
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(0); // Change to the index of the tab you want to select
  }

  async openWarehouseTransferDialog(element: any) {
    const dialogRef = this.dialog.open(AddWarehouseTransferComponent, {
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
