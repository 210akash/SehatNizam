import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewEmployeeWorkSiteTypeComponent } from '../view-employee-worksitetype/view-employee-worksitetype.component';
import { EmployeeWorkSiteTypeService } from '../employee-worksitetype.service';
import { AddEmployeeWorkSiteTypeComponent } from '../add-employee-worksitetype/add-employee-worksitetype.component';
import { DeleteEmployeeWorkSiteTypeComponent } from '../delete-employee-worksitetype/delete-employee-worksitetype.component';

@Component({
    selector: 'app-employee-worksitetype-list',
    templateUrl: './employee-worksitetype-list.component.html',
    styleUrls: ['./employee-worksitetype-list.component.css'],
    standalone: false
})

export class EmployeeWorkSiteTypeListComponent {
  EmployeeWorkSiteTypeFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSize = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['name', 'createdBy', 'actions'];
  dataSource: any;
  take = 50;
  totalRows = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private employeeWorkSiteTypeService: EmployeeWorkSiteTypeService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit(): Promise<void> {
    this.EmployeeWorkSiteTypeFilterForm = this.formBuilder.group({});
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
    const _EmployeeWorkSiteTypeFilterForm = {
      ...this.EmployeeWorkSiteTypeFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.employeeWorkSiteTypeService.getAllEmployeeWorkSiteTypes(_EmployeeWorkSiteTypeFilterForm).subscribe({
      next: (data: any) => {
        // Update data source for MatTable
        this.dataSource = new MatTableDataSource(data.item1);
        //this.totalRows = data.item2; // Update totalRows

        // Set up sorting
        this.dataSource.sort = this.sort;

        // If there is data, adjust paginator settings after a short delay
        if (data.item1.length > 0) {
          setTimeout(() => {
            this.paginator.pageIndex = this.currentPage;
            this.paginator.length = data.item2;
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
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(); // Re-fetch data on page change
  }

  openEmployeeWorkSiteTypeDialog(element: any) {
    const dialogRef = this.dialog.open(AddEmployeeWorkSiteTypeComponent, {
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

  viewEmployeeWorkSiteTypeDialog(element: any): void {
    this.dialog.open(ViewEmployeeWorkSiteTypeComponent, {
      data: { element: element },
     panelClass: 'cstm_width_500',
     height: 'auto',
      disableClose: true
    });
  }

  deleteEmployeeWorkSiteTypeDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteEmployeeWorkSiteTypeComponent, {
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


}
