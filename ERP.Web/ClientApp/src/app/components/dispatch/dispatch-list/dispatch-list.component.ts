import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { DispatchService } from '../dispatch.service';
import { AddDispatchComponent } from '../add-dispatch/add-dispatch.component';
import { DeleteDispatchComponent } from '../delete-dispatch/delete-dispatch.component';
import { ViewDispatchComponent } from '../view-dispatch/view-dispatch.component';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService } from '../../../Service/constant.service';
import { ProcessDispatchComponent } from '../process-dispatch/process-dispatch.component';
import { ApproveDispatchComponent } from '../approve-dispatch/approve-dispatch.component';
import { PrintDispatchOrdersPopupComponent } from '../print-dispatch-orders-popup/print-dispatch-orders-popup.component';

@Component({
  selector: 'app-dispatch-list',
  templateUrl: './dispatch-list.component.html',
  styleUrls: ['./dispatch-list.component.css'],
  standalone: false
})

export class DispatchListComponent {
  @Output() getdispatchCount: EventEmitter<void> = new EventEmitter<void>();
  DispatchFilterForm!: FormGroup;
  isLoading = false;
  displayedColumns: string[] = ['code', 'createdDate', 'createdBy', 'actions'];
  dataSource: any;

  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  pageSize = 5;
  totalRows = 0;
  currenttab: any;
  subcategoryList: any;
  currentUser: any;
  roleList: string | undefined;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private dispatchService: DispatchService,
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
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }

  async bindData(dispatchFilterForm: any, currenttab: number, isFromParent: boolean): Promise<void> {

    if (isFromParent == true) {
      this.currentPage = 0;
    }

    this.currenttab = currenttab;
    if (currenttab == 0) {
      this.displayedColumns = ['code', 'createdDate', 'createdBy', 'actions'];
    }
    else if (currenttab == 1) {
      this.displayedColumns = ['code', 'processedDate', 'processedBy', 'actions'];
    }
    else if (currenttab == 2) {
      this.displayedColumns = ['code', 'approvedDate', 'approvedBy', 'actions'];
    }
    return new Promise<void>(async (resolve, reject) => {
      // Set loading indicator
      this.isLoading = true;
      this.DispatchFilterForm = dispatchFilterForm;

      const pagingData = {
        currentPage: this.currentPage,
        take: this.pageSize
      };

      dispatchFilterForm["PagingData"] = pagingData;
      let fdate = new Date(dispatchFilterForm.fdate);
      let tdate = new Date(dispatchFilterForm.tdate);

      dispatchFilterForm['fdate'] = fdate.toLocaleDateString();
      dispatchFilterForm['tdate'] = tdate.toLocaleDateString();

      // Call the service method and subscribe with the observer

      (await this.dispatchService.getAllDispatchs(dispatchFilterForm)).subscribe({
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
        }
      });
    });
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(this.DispatchFilterForm, this.currenttab, false); // Re-fetch data on page change
  }

  openDispatchDialog(element: any) {
    const dialogRef = this.dialog.open(AddDispatchComponent, {
      width: '70%',
      maxHeight: '90vh',
      disableClose: true,
      data: {
        element: element,
      },
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.DispatchFilterForm, this.currenttab, false);
      this.getdispatchCount.emit();
    });
  }

  viewDispatchDialog(element: any): void {
    this.dialog.open(ViewDispatchComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1400',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    });
  }

  deleteDispatchDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteDispatchComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1400',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.DispatchFilterForm, this.currenttab, false);
      this.getdispatchCount.emit();
    });
  }

  processDispatchDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessDispatchComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1400',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.DispatchFilterForm, this.currenttab, false);
      this.getdispatchCount.emit();
    });
  }

  approveDispatchDialog(element: any) {
    const dialogRef = this.dialog.open(ApproveDispatchComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1400',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.DispatchFilterForm, this.currenttab, false);
      this.getdispatchCount.emit();
    });
  }

  printDispatchDialog(element: any) {
    const dialogRef = this.dialog.open(PrintDispatchOrdersPopupComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1400',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.DispatchFilterForm, this.currenttab, false);
      this.getdispatchCount.emit();
    });
  }

  getcategoryList() {
    this.subcategoryService.getSubcategoryByCompany().subscribe((data: any) => {
      this.subcategoryList = data;
    });
  }

  filterData() {
    this.bindData(this.DispatchFilterForm, this.currenttab, false);
  }


}