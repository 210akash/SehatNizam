import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { RoomService } from '../room.service';
import { AddRoomComponent } from '../add-room/add-room.component';
import { DeleteRoomComponent } from '../delete-room/delete-room.component';
import { ViewRoomComponent } from '../view-room/view-room.component';
import { WardService } from '../../ward/ward.service';

@Component({
    selector: 'app-room-list',
    templateUrl: './room-list.component.html',
    styleUrls: ['./room-list.component.css'],
    standalone: false
})

export class RoomListComponent {
  RoomFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['ward','code','name', 'createdBy','company', 'actions'];
  dataSource: any;
  take = 5;
  totalRows = 0;
  wardList :any;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private roomService: RoomService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private wardService: WardService,
  ) { }

  async ngOnInit(): Promise<void> {
    this.RoomFilterForm = this.formBuilder.group({
      name: [''],
      accountCategoryId: ['']
    });
    await this.bindData(); // Await bindData if it's async
    this.getwardList();
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
    const _RoomFilterForm = {
      ...this.RoomFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.roomService.getAllRooms(_RoomFilterForm).subscribe({
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

  openRoomDialog(element: any) {
    const dialogRef = this.dialog.open(AddRoomComponent, {
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

  viewRoomDialog(element: any): void {
    this.dialog.open(ViewRoomComponent, {
      data: { element: element },
      panelClass: 'cstm_width_500',
      height: 'auto',
      disableClose: true
    });
  }

  deleteRoomDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteRoomComponent, {
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

  getwardList() {
    let _CategoryFilter: any = {};
    this.wardService.getAllWards(_CategoryFilter).subscribe((data: any) => {
     this.wardList = data.item1;
    });
  }

  filterData() {
    this.bindData();
  }
}
