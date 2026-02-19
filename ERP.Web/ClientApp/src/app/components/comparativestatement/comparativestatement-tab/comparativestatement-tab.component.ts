import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { ComparativeStatementListComponent } from '../comparativestatement-list/comparativestatement-list.component';
import { ComparativeStatementService } from '../comparativestatement.service';
import { AddComparativeStatementComponent } from '../add-comparativestatement/add-comparativestatement.component';
// import { AddCommentsComponent } from '../lead-history/add-comments/add-comments.component';
// import { ConfirmLeadsComponent } from '../confirm-comparativestatement/confirm-comparativestatement.component';

@Component({
  selector: 'app-comparativestatement-tab',
  templateUrl: './comparativestatement-tab.component.html',
  styleUrls: ['./comparativestatement-tab.component.css'],
  standalone : false
})

export class ComparativeStatementTabComponent implements OnInit {
  comparativestatementFilterForm!: FormGroup;
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

  constructor(private comparativestatementService : ComparativeStatementService,private notificationsService: NotificationsService,private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(ComparativeStatementListComponent) comparativestatementListComponent!: ComparativeStatementListComponent;
  async ngOnInit() {
    this.comparativestatementFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      statusId: [0]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.comparativestatementFilterForm.get('fdate').patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.comparativestatementFilterForm.get('tdate').patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.comparativestatementFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.comparativestatementFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

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
   await this.comparativestatementListComponent.bindData(this.lObjLeadsFilterForm);
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
    let _comparativestatementFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.comparativestatementFilterForm.get("statusId")?.patchValue(statusId);
    _comparativestatementFilterForm = Object.assign(_comparativestatementFilterForm, this.comparativestatementFilterForm.value);
    this.lObjLeadsFilterForm = _comparativestatementFilterForm;
    await this.getcomparativestatementCount();
  }

  async getcomparativestatementCount() {
    (this.comparativestatementService.getIndentRequestCount(this.lObjLeadsFilterForm)).subscribe(
      {
        next: async (data:any) => {
          this.CountCreated = data.item1;
          this.CountProcessed = data.item2;
          this.CountApproved = data.item3;
          this.addTab();
          await this.comparativestatementListComponent.bindData(this.lObjLeadsFilterForm);

        }, error(error:any) {
          console.log(error);
        }
      });
  }

  resetForm() {
    this.comparativestatementFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.comparativestatementFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.comparativestatementFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(1); // Change to the index of the tab you want to select
  }

  async opencomparativestatementDialog(element: any) {
    const dialogRef = this.dialog.open(AddComparativeStatementComponent, {
      data: { element: element},
      panelClass: 'cstm_width_950',
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