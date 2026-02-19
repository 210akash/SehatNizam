import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewIndentTypeComponent } from '../view-indenttype/view-indenttype.component';
import { IndentTypeService } from '../indenttype.service';
import { AddIndentTypeComponent } from '../add-indenttype/add-indenttype.component';
import { DeleteIndentTypeComponent } from '../delete-indenttype/delete-indenttype.component';

@Component({
    selector: 'app-indenttype-list',
    templateUrl: './indenttype-list.component.html',
    styleUrls: ['./indenttype-list.component.css'],
    standalone: false
})

export class IndentTypeListComponent {
  IndentTypeFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['name', 'createdBy','company', 'actions'];
  dataSource: any;
  take = 50;
  totalRows = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private indenttypeService: IndentTypeService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit(): Promise<void> {
    this.IndentTypeFilterForm = this.formBuilder.group({});
    await this.bindData(); // Await bindData if it's async
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
    const _IndentTypeFilterForm = {
      ...this.IndentTypeFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.indenttypeService.getAllIndentTypes(_IndentTypeFilterForm).subscribe({
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

  openIndentTypeDialog(element: any) {
    const dialogRef = this.dialog.open(AddIndentTypeComponent, {
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

  viewIndentTypeDialog(element: any): void {
    this.dialog.open(ViewIndentTypeComponent, {
      data: { element: element },
      width: '30%',
      disableClose: true
    });
  }

  deleteIndentTypeDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteIndentTypeComponent, {
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

  
}
