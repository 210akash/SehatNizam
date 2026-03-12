import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewAccountCategoryComponent } from '../view-accountcategory/view-accountcategory.component';
import { AccountCategoryService } from '../accountcategory.service';
import { AddAccountCategoryComponent } from '../add-accountcategory/add-accountcategory.component';
import { DeleteAccountCategoryComponent } from '../delete-accountcategory/delete-accountcategory.component';

@Component({
    selector: 'app-accountcategory-list',
    templateUrl: './accountcategory-list.component.html',
    styleUrls: ['./accountcategory-list.component.css'],
    standalone: false
})

export class AccountCategoryListComponent {
  AccountCategoryFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['accountHead','code','name', 'createdBy','company', 'actions'];
  dataSource: any;
  take = 5;
  totalRows = 0;
  accountcategoryList :any;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private accountcategoryService: AccountCategoryService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit(): Promise<void> {
    this.AccountCategoryFilterForm = this.formBuilder.group({
      name: ['']
    });
    await this.bindData(); // Await bindData if it's async
    this.getaccountcategoryList();
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
    const _AccountCategoryFilterForm = {
      ...this.AccountCategoryFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.accountcategoryService.getAllAccountCategorys(_AccountCategoryFilterForm).subscribe({
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

  openAccountCategoryDialog(element: any) {
    const dialogRef = this.dialog.open(AddAccountCategoryComponent, {
      panelClass: 'cstm_width_500',
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

  viewAccountCategoryDialog(element: any): void {
    this.dialog.open(ViewAccountCategoryComponent, {
      data: { element: element },
      panelClass: 'cstm_width_500',
      height: 'auto',
      disableClose: true
    });
  }

  deleteAccountCategoryDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteAccountCategoryComponent, {
      panelClass: 'cstm_width_500',
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
  getaccountcategoryList() {
    let _AccountCategoryFilter: any = {};
    this.accountcategoryService.getAllAccountCategorys(_AccountCategoryFilter).subscribe((data: any) => {
     this.accountcategoryList = data.item1;
    });
  }
  filterData() {
    this.bindData();
  }
  
}
