import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { BedService } from '../bed.service';
import { AddBedComponent } from '../add-bed/add-bed.component';
import { DeleteBedComponent } from '../delete-bed/delete-bed.component';
import { ViewBedComponent } from '../view-bed/view-bed.component';
import { RoomService } from '../../room/room.service';

@Component({
    selector: 'app-bed-list',
    templateUrl: './bed-list.component.html',
    styleUrls: ['./bed-list.component.css'],
    standalone: false
})

export class BedListComponent {
  BedFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['ward','room','code','name', 'createdBy','actions'];
  dataSource: any;
  take = 5;
  totalRows = 0;
  roomList :any;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private bedService: BedService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private roomService: RoomService,
  ) { }

  async ngOnInit(): Promise<void> {
    this.BedFilterForm = this.formBuilder.group({
      name: [''],
      roomId: ['']
    });
    await this.bindData(); // Await bindData if it's async
    this.getroomList();
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
    const _BedFilterForm = {
      ...this.BedFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.bedService.getAllBeds(_BedFilterForm).subscribe({
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

  openBedDialog(element: any) {
    const dialogRef = this.dialog.open(AddBedComponent, {
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

  viewBedDialog(element: any): void {
    this.dialog.open(ViewBedComponent, {
      data: { element: element },
      width: '30%',
      disableClose: true
    });
  }

  deleteBedDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteBedComponent, {
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

  getroomList() {
    let _accountsubWardFilter: any = {};
    this.roomService.getAllRooms(_accountsubWardFilter).subscribe((data: any) => {
     this.roomList = data.item1;
    });
  }

  filterData() {
    this.bindData();
  }
}
