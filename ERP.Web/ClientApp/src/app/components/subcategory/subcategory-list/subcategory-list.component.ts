import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { SubcategoryService } from '../subcategory.service';
import { AddSubcategoryComponent } from '../add-subcategory/add-subcategory.component';
import { DeleteSubcategoryComponent } from '../delete-subcategory/delete-subcategory.component';
import { ViewSubcategoryComponent } from '../view-subcategory/view-subcategory.component';
import { CategoryService } from '../../category/category.service';

@Component({
    selector: 'app-subcategory-list',
    templateUrl: './subcategory-list.component.html',
    styleUrls: ['./subcategory-list.component.css'],
    standalone: false
})

export class SubcategoryListComponent {
  SubcategoryFilterForm!: FormGroup;
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
    private subcategoryService: SubcategoryService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private categoryService: CategoryService,
  ) { }

  async ngOnInit(): Promise<void> {
    this.SubcategoryFilterForm = this.formBuilder.group({
      name: [''],
      categoryId: ['']
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
    const _SubcategoryFilterForm = {
      ...this.SubcategoryFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.subcategoryService.getAllSubcategorys(_SubcategoryFilterForm).subscribe({
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

  openSubcategoryDialog(element: any) {
    const dialogRef = this.dialog.open(AddSubcategoryComponent, {
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

  viewSubcategoryDialog(element: any): void {
    this.dialog.open(ViewSubcategoryComponent, {
      data: { element: element },
      panelClass: 'cstm_width_500',
      height: 'auto',
      disableClose: true
    });
  }

  deleteSubcategoryDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteSubcategoryComponent, {
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
    this.categoryService.getAllCategorys(_CategoryFilter).subscribe((data: any) => {
     this.categoryList = data.item1;
    });
  }

  filterData() {
    this.bindData();
  }
}
