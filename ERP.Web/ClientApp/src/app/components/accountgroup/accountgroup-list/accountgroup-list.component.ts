import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { AccountGroupService } from '../accountgroup.service';
import { AddAccountGroupComponent } from '../add-accountgroup/add-accountgroup.component';
import { DeleteAccountGroupComponent } from '../delete-accountgroup/delete-accountgroup.component';
import { ViewAccountGroupComponent } from '../view-accountgroup/view-accountgroup.component';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { AccountService } from '../../account/account.service';
import { AuthenticationService } from '../../../Auth/authentication.service';
import { environment } from '../../../../environments/environment';

@Component({
    selector: 'app-accountgroup-list',
    templateUrl: './accountgroup-list.component.html',
    styleUrls: ['./accountgroup-list.component.css'],
    standalone: false
})

export class AccountGroupListComponent {
  AccountGroupFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['account','code','name', 'createdBy','company', 'actions'];
  dataSource: any;
  take = 5;
  totalRows = 0;
  accountList :any;
  currentUser: any;
  reportsUrl: any;
  roleList: string | undefined;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private accountgroupService: AccountGroupService,
    private accountService: AccountService,
    private authenticationService :  AuthenticationService ,
  ) { this.reportsUrl = environment.reports_uri; }

  async ngOnInit(): Promise<void> {
    this.currentUser = this.authenticationService.currentUserValue;
    this.AccountGroupFilterForm = this.formBuilder.group({
      name: [''],
      accountId: ['']
    });

        this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
    await this.bindData(); // Await bindData if it's async
    this.getaccountList();
  }

  async bindData(): Promise<void> {
    // Set loading indicator
    this.isLoading = true;

    // Prepare paging data
    const pagingData = {
      currentPage: this.currentPage,
      take: this.take
    };

    // Clone the form value and add paging data
    const _AccountGroupFilterForm = {
      ...this.AccountGroupFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.accountgroupService.getAllAccountGroups(_AccountGroupFilterForm).subscribe({
      next: (data: any) => {
        // Update data source for MatTable
        this.dataSource = new MatTableDataSource(data.item1);
        this.totalRows = data.item2; // Update totalRows

        // Set up sorting
        this.dataSource.sort = this.sort;

        // If there is data, adjust paginator settings after a short delay
        if (data.item1.length > 0) {
          setTimeout(() => {
            this.paginator.pageIndex = this.currentPage;
            this.paginator.length = this.totalRows;
          });
        }

        // Reset loading indicator
        this.isLoading = false;
      },
      error: (error: any) => {
        // Handle errors
        console.error('Error fetching data:', error);
        this.isLoading = false;
      }
    });
  }

  pageChanged(event: PageEvent): void {
    this.take = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(); // Re-fetch data on page change
  }

  openAccountGroupDialog(element: any) {
    const dialogRef = this.dialog.open(AddAccountGroupComponent, {
      panelClass: 'cstm_width_800',
      height: 'auto',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  viewAccountGroupDialog(element: any): void {
    this.dialog.open(ViewAccountGroupComponent, {
      data: { element: element },
      panelClass: 'cstm_width_800',
      height: 'auto',
      disableClose: true
    });
  }

  deleteAccountGroupDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteAccountGroupComponent, {
      panelClass: 'cstm_width_800',
      height: 'auto',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  getaccountList() {
    let accountgrouptypeFilter  = {};
    this.accountService.getGroupAccount(accountgrouptypeFilter).subscribe((data: any) => {
     this.accountList = data;
    });
  }

  filterData() {
    this.bindData();
  }

  redirectToaccountledger(accountId : any) {
    const url = ''+this.reportsUrl+'ReportServer/Pages/ReportViewer.aspx?%2FERPReports%2FAccountLedger&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId='+ this.currentUser.department.companyId + '&Account=' + accountId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

}
