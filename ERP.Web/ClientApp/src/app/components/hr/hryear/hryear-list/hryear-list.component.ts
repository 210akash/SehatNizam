import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewHRYearComponent } from '../view-hryear/view-hryear.component';
import { HRYearService } from '../hryear.service';
import { AddHRYearComponent } from '../add-hryear/add-hryear.component';
import { DeleteHRYearComponent } from '../delete-hryear/delete-hryear.component';

@Component({
    selector: 'app-hryear-list',
    templateUrl: './hryear-list.component.html',
    styleUrls: ['./hryear-list.component.css'],
    standalone: false
})

export class HRYearListComponent {
  HRYearFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSize = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['name', 'startDate','endDate', 'createdBy', 'actions'];
  dataSource: any;
  take = 50;
  totalRows = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private hryearService: HRYearService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit(): Promise<void> {
    this.HRYearFilterForm = this.formBuilder.group({});
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
    const _HRYearFilterForm = {
      ...this.HRYearFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.hryearService.getAllHryear(_HRYearFilterForm).subscribe({
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

  openHRYearDialog(element: any) {
    const dialogRef = this.dialog.open(AddHRYearComponent, {
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

  viewHRYearDialog(element: any): void {
    this.dialog.open(ViewHRYearComponent, {
      data: { element: element },
     panelClass: 'cstm_width_500',
     height: 'auto',
      disableClose: true
    });
  }

  deleteHRYearDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteHRYearComponent, {
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
