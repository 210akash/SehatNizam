import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { AccountTypeService } from '../accounttype.service';
import { AddAccountTypeComponent } from '../add-accounttype/add-accounttype.component';
import { DeleteAccountTypeComponent } from '../delete-accounttype/delete-accounttype.component';
import { ViewAccountTypeComponent } from '../view-accounttype/view-accounttype.component';
import { AccountSubcategoryService } from '../../accountsubcategory/accountsubcategory.service';

@Component({
    selector: 'app-accountType-list',
    templateUrl: './accountType-list.component.html',
    styleUrls: ['./accountType-list.component.css'],
    standalone: false
})

export class AccountTypeListComponent {
  AccountTypeFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['category','subcategory','code','name', 'createdBy','company', 'actions'];
  dataSource: any;
  take = 5;
  totalRows = 0;
  subcategoryList :any;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private accountTypeService: AccountTypeService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: AccountSubcategoryService,
  ) { }

  async ngOnInit(): Promise<void> {
    this.AccountTypeFilterForm = this.formBuilder.group({
      name: [''],
      accountSubCategoryId: ['']
    });
    await this.bindData(); // Await bindData if it's async
    this.getcategoryList();
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
    const _AccountTypeFilterForm = {
      ...this.AccountTypeFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.accountTypeService.getAllAccounttypes(_AccountTypeFilterForm).subscribe({
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

  openAccountTypeDialog(element: any) {
    const dialogRef = this.dialog.open(AddAccountTypeComponent, {
      width: '30%',
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

  viewAccountTypeDialog(element: any): void {
    this.dialog.open(ViewAccountTypeComponent, {
      data: { element: element },
      width: '30%',
      disableClose: true
    });
  }

  deleteAccountTypeDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteAccountTypeComponent, {
      width: '30%',
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

  getcategoryList() {
    let _accountsubCategoryFilter: any = {};
    this.subcategoryService.getAllAccountSubcategorys(_accountsubCategoryFilter).subscribe((data: any) => {
     this.subcategoryList = data.item1;
    });
  }

  filterData() {
    this.bindData();
  }
}
