import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewWardComponent } from '../view-ward/view-ward.component';
import { WardService } from '../ward.service';
import { AddWardComponent } from '../add-ward/add-ward.component';
import { DeleteWardComponent } from '../delete-ward/delete-ward.component';
import { DepartmentService } from '../../../department/department.service';

@Component({
    selector: 'app-ward-list',
    templateUrl: './ward-list.component.html',
    styleUrls: ['./ward-list.component.css'],
    standalone: false
})

export class WardListComponent {
  WardFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['code','name', 'createdBy','project', 'actions'];
  dataSource: any;
  take = 5;
  totalRows = 0;
  wardList :any;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private wardService: WardService,
    private departmentService: DepartmentService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit(): Promise<void> {
    this.WardFilterForm = this.formBuilder.group({
      name: ['']
    });
    await this.bindData(); // Await bindData if it's async
    this.getdepartmentList();
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
    const _WardFilterForm = {
      ...this.WardFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.wardService.getAllWards(_WardFilterForm).subscribe({
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

  openWardDialog(element: any) {
    const dialogRef = this.dialog.open(AddWardComponent, {
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

  viewWardDialog(element: any): void {
    this.dialog.open(ViewWardComponent, {
      data: { element: element },
      panelClass: 'cstm_width_500',
      height: 'auto',
      disableClose: true
    });
  }

  deleteWardDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteWardComponent, {
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

  getdepartmentList() {
    this.departmentService.getAllDepartments({}).subscribe((data: any) => {
     this.wardList = data.item1;
    });
  }

  filterData() {
    this.bindData();
  }
}
