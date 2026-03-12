import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { AccountSubcategoryService } from '../accountsubcategory.service';
import { AddAccountSubcategoryComponent } from '../add-accountsubcategory/add-accountsubcategory.component';
import { DeleteAccountSubcategoryComponent } from '../delete-accountsubcategory/delete-accountsubcategory.component';
import { ViewAccountSubcategoryComponent } from '../view-accountsubcategory/view-accountsubcategory.component';
import { AccountCategoryService } from '../../accountcategory/accountcategory.service';

@Component({
    selector: 'app-accountsubcategory-list',
    templateUrl: './accountsubcategory-list.component.html',
    styleUrls: ['./accountsubcategory-list.component.css'],
    standalone: false
})

export class AccountSubcategoryListComponent {
  AccountSubcategoryFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['category','code','name', 'createdBy','company', 'actions'];
  dataSource: any;
  take = 5;
  totalRows = 0;
  categoryList :any;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private accountsubcategoryService: AccountSubcategoryService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private categoryService: AccountCategoryService,
  ) { }

  async ngOnInit(): Promise<void> {
    this.AccountSubcategoryFilterForm = this.formBuilder.group({
      name: [''],
      accountCategoryId: ['']
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
    const _AccountSubcategoryFilterForm = {
      ...this.AccountSubcategoryFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.accountsubcategoryService.getAllAccountSubcategorys(_AccountSubcategoryFilterForm).subscribe({
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

  openAccountSubcategoryDialog(element: any) {
    const dialogRef = this.dialog.open(AddAccountSubcategoryComponent, {
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

  viewAccountSubcategoryDialog(element: any): void {
    this.dialog.open(ViewAccountSubcategoryComponent, {
      data: { element: element },
      panelClass: 'cstm_width_500',
      height: 'auto',
      disableClose: true
    });
  }

  deleteAccountSubcategoryDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteAccountSubcategoryComponent, {
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

  getcategoryList() {
    let _CategoryFilter: any = {};
    this.categoryService.getAllAccountCategorys(_CategoryFilter).subscribe((data: any) => {
     this.categoryList = data.item1;
    });
  }

  filterData() {
    this.bindData();
  }
}
