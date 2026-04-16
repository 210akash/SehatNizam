import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewInterviewComponent } from '../view-interview/view-interview.component';
import { InterviewService } from '../interview.service';
import { AddInterviewComponent } from '../add-interview/add-interview.component';
import { DeleteInterviewComponent } from '../delete-interview/delete-interview.component';
import { AddCommentsComponent } from '../add-comments/add-comments.component';

@Component({
  selector: 'app-conduct-interview-list',
  templateUrl: './conduct-interview-list.component.html',
  styleUrls: ['./conduct-interview-list.component.css'],
  standalone: false
})

export class ConductInterviewListComponent {
  InterviewFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSize = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['code', 'name', 'status', 'createdBy', 'createdDate', 'actions'];
  dataSource: any;
  take = 50;
  totalRows = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private interviewService: InterviewService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit(): Promise<void> {
    this.InterviewFilterForm = this.formBuilder.group({});
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
    const _InterviewFilterForm = {
      ...this.InterviewFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.interviewService.getAllInterviews(_InterviewFilterForm).subscribe({
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

  openInterviewDialog(element: any) {
    const dialogRef = this.dialog.open(AddInterviewComponent, {
      panelClass: 'cstm_width_1000',
      height: 'auto',
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

  viewInterviewDialog(element: any): void {
    this.dialog.open(ViewInterviewComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1300',
      height: 'auto',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteInterviewDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteInterviewComponent, {
      panelClass: 'cstm_width_1300',
      height: 'auto',
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

  addCommentsDialog(element: any) {
    const dialogRef = this.dialog.open(AddCommentsComponent, {
      panelClass: 'cstm_width_1300',
      height: 'auto',
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


}