import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewPriorityComponent } from '../view-priority/view-priority.component';
import { PriorityService } from '../priority.service';
import { AddPriorityComponent } from '../add-priority/add-priority.component';
import { DeletePriorityComponent } from '../delete-priority/delete-priority.component';

@Component({
    selector: 'app-priority-list',
    templateUrl: './priority-list.component.html',
    styleUrls: ['./priority-list.component.css'],
    standalone: false
})

export class PriorityListComponent {
  PriorityFilterForm!: FormGroup;
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
    private priorityService: PriorityService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit(): Promise<void> {
    this.PriorityFilterForm = this.formBuilder.group({});
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
    const _PriorityFilterForm = {
      ...this.PriorityFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.priorityService.getAllPrioritys(_PriorityFilterForm).subscribe({
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

  openPriorityDialog(element: any) {
    const dialogRef = this.dialog.open(AddPriorityComponent, {
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

  viewPriorityDialog(element: any): void {
    this.dialog.open(ViewPriorityComponent, {
      data: { element: element },
      width: '30%',
      disableClose: true
    });
  }

  deletePriorityDialog(element: any) {
    const dialogRef = this.dialog.open(DeletePriorityComponent, {
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
