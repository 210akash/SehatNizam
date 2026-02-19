import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { CurrencyService } from '../currency.service';
import { AddCurrencyComponent } from '../add-currency/add-currency.component';
import { DeleteCurrencyComponent } from '../delete-currency/delete-currency.component';
import { ViewCurrencyComponent } from '../view-currency/view-currency.component';
import { CategoryService } from '../../category/category.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';

@Component({
    selector: 'app-currency-list',
    templateUrl: './currency-list.component.html',
    styleUrls: ['./currency-list.component.css'],
    standalone: false
})

export class CurrencyListComponent {
  CurrencyFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['name', 'symbol','createdBy','company', 'actions'];
  dataSource: any;
  take = 5;
  totalRows = 0;
  subcategoryList :any;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private currencyService: CurrencyService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
  ) { }

  async ngOnInit(): Promise<void> {
    this.CurrencyFilterForm = this.formBuilder.group({
      name: [''],
      subCategoryId: ['']
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
    const _CurrencyFilterForm = {
      ...this.CurrencyFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.currencyService.getAllCurrencys(_CurrencyFilterForm).subscribe({
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

  openCurrencyDialog(element: any) {
    const dialogRef = this.dialog.open(AddCurrencyComponent, {
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

  viewCurrencyDialog(element: any): void {
    this.dialog.open(ViewCurrencyComponent, {
      data: { element: element },
      width: '30%',
      disableClose: true
    });
  }

  deleteCurrencyDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteCurrencyComponent, {
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
    this.subcategoryService.getSubcategoryByCompany().subscribe((data: any) => {
     this.subcategoryList = data;
    });
  }

  filterData() {
    this.bindData();
  }
}
