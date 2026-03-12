import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewRejectReasonComponent } from '../view-rejectreason/view-rejectreason.component';
import { RejectReasonService } from '../rejectreason.service';
import { AddRejectReasonComponent } from '../add-rejectreason/add-rejectreason.component';
import { DeleteRejectReasonComponent } from '../delete-rejectreason/delete-rejectreason.component';

@Component({
    selector: 'app-rejectreason-list',
    templateUrl: './rejectreason-list.component.html',
    styleUrls: ['./rejectreason-list.component.css'],
    standalone: false
})

export class RejectReasonListComponent {
  RejectReasonFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSize = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['name', 'createdBy','company', 'actions'];
  dataSource: any;
  take = 50;
  totalRows = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private rejectreasonService: RejectReasonService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit(): Promise<void> {
    this.RejectReasonFilterForm = this.formBuilder.group({});
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
    const _RejectReasonFilterForm = {
      ...this.RejectReasonFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.rejectreasonService.getAllRejectReasons(_RejectReasonFilterForm).subscribe({
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

  openRejectReasonDialog(element: any) {
    const dialogRef = this.dialog.open(AddRejectReasonComponent, {
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

  viewRejectReasonDialog(element: any): void {
    this.dialog.open(ViewRejectReasonComponent, {
      data: { element: element },
      width: '30%',
      disableClose: true
    });
  }

  deleteRejectReasonDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteRejectReasonComponent, {
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
