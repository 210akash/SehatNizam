import { Component, EventEmitter, ViewChild, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { CategoryService } from '../../category/category.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService } from '../../../Service/constant.service';
import { SafeHtml } from '@angular/platform-browser';
import { PrintWarehouseTransferComponent } from '../print-warehousetransfer/print-warehousetransfer.component';
import { WarehouseTransferService } from '../warehousetransfer.service';
import { AddWarehouseTransferComponent } from '../add-warehousetransfer/add-warehousetransfer.component';
import { DeleteWarehouseTransferComponent } from '../delete-warehousetransfer/delete-warehousetransfer.component';
import { ViewWarehouseTransferComponent } from '../view-warehousetransfer/view-warehousetransfer.component';
import { ProcessWarehouseTransferComponent } from '../process-warehousetransfer/process-warehousetransfer.component';
import { ApproveWarehouseTransferComponent } from '../approve-warehousetransfer/approve-warehousetransfer.component';
@Component({
  selector: 'app-warehousetransfer-list',
  templateUrl: './warehousetransfer-list.component.html',
  styleUrls: ['./warehousetransfer-list.component.css'],
  standalone: false
})

export class WarehouseTransferListComponent {
  [x: string]: any;
  @Output() getWarehouseTransferCount: EventEmitter<void> = new EventEmitter<void>();
  WarehouseTransferFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = [];
  dataSource: any;
  take = 50;
  pageSize = 0;
  totalRows = 0;
  subcategoryList: any;
  currentUser: any;
  currenttab: any;
  History: any;
  roleList: string | undefined;
  dialogRef: any;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private warehousetransferService: WarehouseTransferService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.WarehouseTransferFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }

  async bindData(warehousetransferFilterForm: any, currenttab: number, isFromParent: boolean): Promise<void> {

    if (isFromParent == true) {
      this.currentPage = 0;
    }
    
    this.currenttab = currenttab;
    if (currenttab == 0) {
      this.displayedColumns = ['code', 'transferto', 'createdDate', 'createdBy', 'status', 'actions'];
    }
    else if (currenttab == 1) {
      this.displayedColumns = ['code', 'transferto', 'processedDate', 'processedBy', 'status', 'actions'];
    }
    else if (currenttab == 2) {
      this.displayedColumns = ['code', 'transferto', 'approvedDate', 'approvedBy', 'status', 'actions'];
    }

    return new Promise<void>(async (resolve, reject) => {
      // Set loading indicator
      this.isLoading = true;
      this.WarehouseTransferFilterForm = warehousetransferFilterForm;

      const pagingData = {
        currentPage: this.currentPage,
        take: this.pageSize
      };

      warehousetransferFilterForm["PagingData"] = pagingData;
      let fdate = new Date(warehousetransferFilterForm.fdate);
      let tdate = new Date(warehousetransferFilterForm.tdate);

      warehousetransferFilterForm['fdate'] = fdate.toLocaleDateString();
      warehousetransferFilterForm['tdate'] = tdate.toLocaleDateString();

      // Call the service method and subscribe with the observer

      (await this.warehousetransferService.getAllWarehouseTransfers(warehousetransferFilterForm)).subscribe({
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
    });
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(this.WarehouseTransferFilterForm, this.currenttab, false); // Re-fetch data on page change
  }

  openWarehouseTransferDialog(element: any) {
    const dialogRef = this.dialog.open(AddWarehouseTransferComponent, {
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.WarehouseTransferFilterForm, this.currenttab, false);
      this.getWarehouseTransferCount.emit();
    });
  }

  viewWarehouseTransferDialog(element: any): void {
    this.dialog.open(ViewWarehouseTransferComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteWarehouseTransferDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteWarehouseTransferComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.WarehouseTransferFilterForm, this.currenttab, false);
      this.getWarehouseTransferCount.emit();
    });
  }

  processWarehouseTransferDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessWarehouseTransferComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.WarehouseTransferFilterForm, this.currenttab, false);
      this.getWarehouseTransferCount.emit();
    });
  }

  approveWarehouseTransferDialog(element: any) {
    const dialogRef = this.dialog.open(ApproveWarehouseTransferComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.WarehouseTransferFilterForm, this.currenttab, false);
      this.getWarehouseTransferCount.emit();
    });
  }

  printWarehouseTransferDialog(element: any) {
    const dialogRef = this.dialog.open(PrintWarehouseTransferComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });
  }

  getcategoryList() {
    this.subcategoryService.getSubcategoryByCompany().subscribe((data: any) => {
      this.subcategoryList = data;
    });
  }

  filterData() {
    this.bindData(this.WarehouseTransferFilterForm, this.currenttab, false);
  }


}
