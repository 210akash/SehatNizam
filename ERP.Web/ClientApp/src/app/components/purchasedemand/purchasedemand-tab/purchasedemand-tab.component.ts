import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { PurchaseDemandListComponent } from '../purchasedemand-list/purchasedemand-list.component';
import { PurchaseDemandService } from '../purchasedemand.service';
import { AddPurchaseDemandComponent } from '../add-purchasedemand/add-purchasedemand.component';
// import { AddCommentsComponent } from '../lead-history/add-comments/add-comments.component';
// import { ConfirmLeadsComponent } from '../confirm-purchasedemand/confirm-purchasedemand.component';

@Component({
  selector: 'app-purchasedemand-tab',
  templateUrl: './purchasedemand-tab.component.html',
  styleUrls: ['./purchasedemand-tab.component.css'],
  standalone : false
})

export class PurchaseDemandTabComponent implements OnInit {
  purchasedemandFilterForm!: FormGroup;
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

  constructor(private purchasedemandService : PurchaseDemandService,private notificationsService: NotificationsService,private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(PurchaseDemandListComponent) purchasedemandListComponent!: PurchaseDemandListComponent;
  async ngOnInit() {
    this.purchasedemandFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      statusId: [0]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.purchasedemandFilterForm.get('fdate').patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.purchasedemandFilterForm.get('tdate').patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.purchasedemandFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.purchasedemandFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

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
   await this.purchasedemandListComponent.bindData(this.lObjLeadsFilterForm);
  }

  generateRoleWiseTab() {
    this.statusTab = null;
  
    // Split the roleList into an array and remove any empty elements or spaces
    const roles = this.roleList?.split(',').map(role => role.trim().toLowerCase());
  
    // Check if the necessary roles exist in the roleList
    if (roles?.includes('manager')) {
      this.statusTab = [
        { label: 'New', id: 1, pCount: this.CountCreated, statusId: 1 },
        { label: 'Processed', id: 2, pCount: this.CountProcessed, statusId: 2 },
        { label: 'Approved', id: 3, pCount: this.CountApproved, statusId: 3 },
        { label: 'Issued', id: 4, pCount: this.CountIssued, statusId: 5 },
      ];
    } else if (roles?.includes('assistant')) {
      this.statusTab = [
        { label: 'New', id: 1, pCount: this.CountCreated, statusId: 1 },
        { label: 'Processed', id: 2, pCount: this.CountProcessed, statusId: 2 },
        { label: 'Approved', id: 3, pCount: this.CountApproved, statusId: 3 },
        { label: 'Issued', id: 4, pCount: this.CountIssued, statusId: 5 },
      ];
    } else {
      this.statusTab = [];
    }
  }

  async fillGridCount(statusId: any) {
    this.isLoading = true;
    let _purchasedemandFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.purchasedemandFilterForm.get("statusId")?.patchValue(statusId);
    _purchasedemandFilterForm = Object.assign(_purchasedemandFilterForm, this.purchasedemandFilterForm.value);
    this.lObjLeadsFilterForm = _purchasedemandFilterForm;
    await this.getpurchasedemandCount();
  }

  async getpurchasedemandCount() {
    (this.purchasedemandService.getIndentRequestCount(this.lObjLeadsFilterForm)).subscribe(
      {
        next: async (data:any) => {
          this.CountCreated = data.item1;
          this.CountProcessed = data.item2;
          this.CountApproved = data.item3;
          this.CountIssued = data.item4;
          this.addTab();
          await this.purchasedemandListComponent.bindData(this.lObjLeadsFilterForm);

        }, error(error:any) {
          console.log(error);
        }
      });
  }

  resetForm() {
    this.purchasedemandFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.purchasedemandFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.purchasedemandFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(0); // Change to the index of the tab you want to select
  }

  async openpurchasedemandDialog(element: any) {
    const dialogRef = this.dialog.open(AddPurchaseDemandComponent, {
      data: { element: element},
      panelClass: 'cstm_width_1300',
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