import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { PurchaseOrderListComponent } from '../purchaseorder-list/purchaseorder-list.component';
import { PurchaseOrderService } from '../purchaseorder.service';
import { AddPurchaseOrderComponent } from '../add-purchaseorder/add-purchaseorder.component';
// import { AddCommentsComponent } from '../lead-history/add-comments/add-comments.component';
// import { ConfirmLeadsComponent } from '../confirm-purchaseorder/confirm-purchaseorder.component';

@Component({
  selector: 'app-purchaseorder-tab',
  templateUrl: './purchaseorder-tab.component.html',
  styleUrls: ['./purchaseorder-tab.component.css'],
  standalone : false
})

export class PurchaseOrderTabComponent implements OnInit {
  purchaseorderFilterForm!: FormGroup;
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
  statusTab : any = [];

  propertyTypeList: any;
  partyTypeList: any;
  projectsList: any;
  lObjLeadsFilterForm: any;
  gIsAll: boolean = false;

  CountCreated: number = 0;
  CountProcessed: number = 0;
  CountApproved: number = 0;
  CountIssued: number = 0;

  constructor(private purchaseorderService : PurchaseOrderService,private notificationsService: NotificationsService,private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(PurchaseOrderListComponent) purchaseorderListComponent!: PurchaseOrderListComponent;
  async ngOnInit() {
    this.purchaseorderFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      statusId: [1]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.purchaseorderFilterForm.get('fdate').patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.purchaseorderFilterForm.get('tdate').patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.purchaseorderFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.purchaseorderFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

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
   await this.purchaseorderListComponent.bindData(this.lObjLeadsFilterForm);
  }

  generateRoleWiseTab() {
    this.statusTab = null;
  
    // Split the roleList into an array and remove any empty elements or spaces
    const roles = this.roleList?.split(',').map(role => role.trim().toLowerCase());
  
    // Check if the necessary roles exist in the roleList
    if (roles?.includes('purchase manager')) {
      this.statusTab = [
        { label: 'New', id: 1, pCount: this.CountCreated, statusId: 1 },
        { label: 'Processed', id: 2, pCount: this.CountProcessed, statusId: 2 },
        { label: 'Approved', id: 3, pCount: this.CountApproved, statusId: 3 },
      ];
    } else if (roles?.includes('purchaser')) {
      this.statusTab = [
        { label: 'New', id: 1, pCount: this.CountCreated, statusId: 1 },
        { label: 'Processed', id: 2, pCount: this.CountProcessed, statusId: 2 },
        { label: 'Approved', id: 3, pCount: this.CountApproved, statusId: 3 },
      ];
    } else {
      this.statusTab = [];
    }
  }

  async fillGridCount(statusId: any) {
    this.isLoading = true;
    let _purchaseorderFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.purchaseorderFilterForm.get("statusId")?.patchValue(statusId);
    _purchaseorderFilterForm = Object.assign(_purchaseorderFilterForm, this.purchaseorderFilterForm.value);
    this.lObjLeadsFilterForm = _purchaseorderFilterForm;
    await this.getpurchaseorderCount();
  }

  async getpurchaseorderCount() {
    (this.purchaseorderService.getIndentRequestCount(this.lObjLeadsFilterForm)).subscribe(
      {
        next: async (data:any) => {
          this.CountCreated = data.item1;
          this.CountProcessed = data.item2;
          this.CountApproved = data.item3;
          this.CountIssued = data.item4;
          this.addTab();
          await this.purchaseorderListComponent.bindData(this.lObjLeadsFilterForm);

        }, error(error:any) {
          console.log(error);
        }
      });
  }

  resetForm() {
    this.purchaseorderFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.purchaseorderFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.purchaseorderFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(0); // Change to the index of the tab you want to select
  }

  async openpurchaseorderDialog(element: any) {
    const dialogRef = this.dialog.open(AddPurchaseOrderComponent, {
      data: { element: element},
      panelClass: 'cstm_width_1000',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.changeTabs(1); // Change to the index of the tab you want to select
      console.log(`Dialog result: ${result}`);
    });
  }
}