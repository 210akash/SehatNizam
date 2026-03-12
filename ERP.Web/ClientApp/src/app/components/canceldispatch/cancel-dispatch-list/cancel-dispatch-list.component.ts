import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConstantService } from '../../../Service/constant.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { CancelDispatchService } from '../canceldispatch.service';
import { ViewCancelDispatchComponent } from '../view-cancel-dispatch/view-cancel-dispatch.component';
import { ProcessCancelDispatchComponent } from '../process-cancel-dispatch/process-cancel-dispatch.component';
import { DeleteCancelDispatchComponent } from '../delete-cancel-dispatch/delete-cancel-dispatch.component';
import { RejectCancelDispatchComponent } from '../reject-cancel-dispatch/reject-cancel-dispatch.component';
import { CancelDispatchHistoryComponent } from '../cancel-dispatch-history/cancel-dispatch-history.component';

@Component({
  selector: 'app-cancel-dispatch-list',
  templateUrl: './cancel-dispatch-list.component.html',
  styleUrl: './cancel-dispatch-list.component.css',
  standalone: false,
})

export class CancelDispatchListComponent {
  @Output() getdispatchCount: EventEmitter<void> = new EventEmitter<void>();
  DispatchFilterForm!: FormGroup;
  isLoading = false;
  displayedColumns: string[] = [
    'code',
    'orderNo',
    'orderDate',
    'distributor',
    'createdDate',
    'createdBy',
    'actions',
  ];
  dataSource: any;

  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  pageSize = 5;
  totalRows = 0;

  subcategoryList: any;
  currentUser: any;
  roleList: string | undefined;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private cancelDispatchService: CancelDispatchService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.DispatchFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: [],
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role
      .toLowerCase()
      .split(',')
      .map((role: string) => role.trim().toLowerCase());
  }

  async bindData(dispatchFilterForm: any, isFromParent: boolean): Promise<void> {
    return new Promise<void>(async (resolve, reject) => {

      if (isFromParent == true) {
        this.currentPage = 0;
      }

      // Set loading indicator
      this.isLoading = true;
      this.DispatchFilterForm = dispatchFilterForm;

      const pagingData = {
        currentPage: this.currentPage,
        take: this.pageSize,
      };

      dispatchFilterForm['PagingData'] = pagingData;
      let fdate = new Date(dispatchFilterForm.fdate);
      let tdate = new Date(dispatchFilterForm.tdate);

      dispatchFilterForm['fdate'] = fdate.toLocaleDateString();
      dispatchFilterForm['tdate'] = tdate.toLocaleDateString();

      // Call the service method and subscribe with the observer

      (
        await this.cancelDispatchService.getAllCancelDispatches(
          dispatchFilterForm
        )
      ).subscribe({
        next: (data: any) => {
          // Update data source for MatTable
          this.dataSource = new MatTableDataSource(data.item1);
          // this.totalRows = data.item2; // Update totalRows

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
        },
      });
    });
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(this.DispatchFilterForm, false); // Re-fetch data on page change
  }

  viewDispatchDialog(element: any): void {
    this.dialog.open(ViewCancelDispatchComponent, {
      data: { element: element },
      width: '70%',
      maxHeight: '90vh',
      disableClose: true,
    });
  }

  deleteDispatchDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteCancelDispatchComponent, {
      width: '70%',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.bindData(this.DispatchFilterForm, false);
      this.getdispatchCount.emit();
    });
  }

  processDispatchDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessCancelDispatchComponent, {
      width: '70%',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.bindData(this.DispatchFilterForm, false);
      this.getdispatchCount.emit();
    });
  }

  getcategoryList() {
    this.subcategoryService.getSubcategoryByCompany().subscribe((data: any) => {
      this.subcategoryList = data;
    });
  }

  filterData() {
    this.bindData(this.DispatchFilterForm, false);
  }

  rejectDispatchDialog(element: any) {
    const dialogRef = this.dialog.open(RejectCancelDispatchComponent, {
      width: '70%',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.bindData(this.DispatchFilterForm, false);
      this.getdispatchCount.emit();
    });
  }

  cancelDispatchHistoryDialog(element: any) {
    const dialogRef = this.dialog.open(CancelDispatchHistoryComponent, {
      width: '70%',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.bindData(this.DispatchFilterForm, false);
      this.getdispatchCount.emit();
    });
  }


}