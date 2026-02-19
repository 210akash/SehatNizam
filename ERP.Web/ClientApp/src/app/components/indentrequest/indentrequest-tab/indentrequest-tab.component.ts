import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { IndentrequestListComponent } from '../indentrequest-list/indentrequest-list.component';
import { IndentrequestService } from '../indentrequest.service';
import { AddIndentrequestComponent } from '../add-indentrequest/add-indentrequest.component';
// import { AddCommentsComponent } from '../lead-history/add-comments/add-comments.component';
// import { ConfirmLeadsComponent } from '../confirm-indentrequest/confirm-indentrequest.component';

@Component({
  selector: 'app-indentrequest-tab',
  templateUrl: './indentrequest-tab.component.html',
  styleUrls: ['./indentrequest-tab.component.css'],
  standalone : false
})

export class IndentrequestTabComponent implements OnInit {
  IndentrequestFilterForm!: FormGroup;
  dataSource: any;
  currentUser: any;
  roleList: string | undefined;
  isLoading = false;
  totalRows = 0;
  pageSize = 0;
  currentPage = 0;
  dialogRef: any;
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

  constructor(private indentrequestService : IndentrequestService,private notificationsService: NotificationsService,private dialog: MatDialog, private constantService: ConstantService, private formBuilder: FormBuilder) { }
  @ViewChild(IndentrequestListComponent) indentrequestListComponent!: IndentrequestListComponent;
  async ngOnInit() {
    this.IndentrequestFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      code: [''],
      statusId: [0]
    });

    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase();

    // this.IndentrequestFilterForm.get('fdate').patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    // this.IndentrequestFilterForm.get('tdate').patchValue(this.constantService.formatDate(new Date()));

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.IndentrequestFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.IndentrequestFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));

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
   await this.indentrequestListComponent.bindData(this.lObjLeadsFilterForm,this.selected.value);
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
    let _IndentrequestFilterForm: any = {};
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.IndentrequestFilterForm.get("statusId")?.patchValue(statusId);
    _IndentrequestFilterForm = Object.assign(_IndentrequestFilterForm, this.IndentrequestFilterForm.value);
    this.lObjLeadsFilterForm = _IndentrequestFilterForm;
    await this.getIndentrequestCount();
  }

  async getIndentrequestCount() {
    (this.indentrequestService.getIndentRequestCount(this.lObjLeadsFilterForm)).subscribe(
      {
        next: async (data:any) => {
          this.CountCreated = data.item1;
          this.CountProcessed = data.item2;
          this.CountApproved = data.item3;
          this.CountIssued = data.item4;
          this.addTab();
          await this.indentrequestListComponent.bindData(this.lObjLeadsFilterForm,this.selected.value);

        }, error(error:any) {
          console.log(error);
        }
      });
  }

  resetForm() {
    this.IndentrequestFilterForm.reset({
      code: "",
      fdate: new Date(),
      tdate: new Date(),
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.IndentrequestFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.IndentrequestFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.filterData();
  }

  changeTabs(index: number) {
    this.selected.value = index;
  }

  onPopupClose() {
    // Change tab index after popup is closed
    this.changeTabs(0); // Change to the index of the tab you want to select
  }

  async openIndentrequestDialog(element: any) {
    const dialogRef = this.dialog.open(AddIndentrequestComponent, {
      data: { element: element},
      panelClass: 'cstm_width_1100',
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