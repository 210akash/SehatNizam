import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { AccountService } from '../account.service';
import { AddAccountComponent } from '../add-account/add-account.component';
import { DeleteAccountComponent } from '../delete-account/delete-account.component';
import { ViewAccountComponent } from '../view-account/view-account.component';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { AccountTypeService } from '../../accounttype/accounttype.service';

@Component({
    selector: 'app-account-list',
    templateUrl: './account-list.component.html',
    styleUrls: ['./account-list.component.css'],
    standalone: false
})

export class AccountListComponent {
  AccountFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['accountcategory','accountsubcategory','accountType','code','name', 'createdBy','company', 'actions'];
  dataSource: any;
  take = 5;
  totalRows = 0;
  subcategoryList :any;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private accountService: AccountService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private accountTypeService: AccountTypeService,
  ) { }

  async ngOnInit(): Promise<void> {
    this.AccountFilterForm = this.formBuilder.group({
      name: [''],
      accountTypeId: ['']
    });
    await this.bindData(); // Await bindData if it's async
    this.getaccountTypeList();
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
    const _AccountFilterForm = {
      ...this.AccountFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.accountService.getAllAccounts(_AccountFilterForm).subscribe({
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

  openAccountDialog(element: any) {
    const dialogRef = this.dialog.open(AddAccountComponent, {
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

  viewAccountDialog(element: any): void {
    this.dialog.open(ViewAccountComponent, {
      data: { element: element },
      panelClass: 'cstm_width_800',
      height: 'auto',
      disableClose: true
    });
  }

  deleteAccountDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteAccountComponent, {
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

  getaccountTypeList() {
    let accounttypeFilter  = {};
    this.accountTypeService.getAllAccounttypes(accounttypeFilter).subscribe((data: any) => {
     this.subcategoryList = data.item1;
    });
  }

  filterData() {
    this.bindData();
  }
}
