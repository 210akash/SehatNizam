import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { GSTService } from '../gst.service';
import { AddGSTComponent } from '../add-gst/add-gst.component';
import { DeleteGSTComponent } from '../delete-gst/delete-gst.component';
import { ViewGSTComponent } from '../view-gst/view-gst.component';

@Component({
    selector: 'app-gst-list',
    templateUrl: './gst-list.component.html',
    styleUrls: ['./gst-list.component.css'],
    standalone: false
})

export class GSTListComponent {
  GSTFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['fDate','tDate','gstPer', 'createdBy','company', 'actions'];
  dataSource: any;
  take = 5;
  totalRows = 0;
  subcategoryList :any;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private gstService: GSTService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
  ) { }

  async ngOnInit(): Promise<void> {
    this.GSTFilterForm = this.formBuilder.group({
      name: [''],
    });
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
    const _GSTFilterForm = {
      ...this.GSTFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.gstService.getAllGST(_GSTFilterForm).subscribe({
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

  openGSTDialog(element: any) {
    const dialogRef = this.dialog.open(AddGSTComponent, {
      panelClass: 'cstm_width_700',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  viewGSTDialog(element: any): void {
    this.dialog.open(ViewGSTComponent, {
      data: { element: element },
      panelClass: 'cstm_width_600',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteGSTDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteGSTComponent, {
      panelClass: 'cstm_width_600',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  filterData() {
    this.bindData();
  }
}
