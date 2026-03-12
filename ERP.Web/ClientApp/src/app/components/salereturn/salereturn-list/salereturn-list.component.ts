import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService } from '../../../Service/constant.service';
import { AddSaleReturnComponent } from '../add-salereturn/add-salereturn.component';
import { DeleteSaleReturnComponent } from '../delete-salereturn/delete-salereturn.component';
import { SaleReturnService } from '../salereturn.service';
import { PrintSaleReturnComponent } from '../print-salereturn/print-salereturn.component';
import { ProcessSaleReturnComponent } from '../process-salereturn/process-salereturn.component';
import { ViewSaleReturnComponent } from '../view-salereturn/view-salereturn.component';
import { ApproveSaleReturnComponent } from '../approve-salereturn/approve-salereturn.component';

@Component({
  selector: 'app-salereturn-list',
  templateUrl: './salereturn-list.component.html',
  styleUrls: ['./salereturn-list.component.css'], standalone: false
})

export class SaleReturnListComponent {
  @Output() getSaleReturnCount: EventEmitter<void> = new EventEmitter<void>();
  SaleReturnFilterForm!: FormGroup;
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
  roleList: string | undefined;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private saleReturnService: SaleReturnService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.SaleReturnFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }

  async bindData(iGPFilterForm: any, currenttab: number, isFromParent: boolean): Promise<void> {

    if (isFromParent == true) {
      this.currentPage = 0;
    }

    this.currenttab = currenttab;
    if (currenttab == 0) {
      this.displayedColumns = ['date', 'code', 'dcCode', 'distributor', 'createdBy', 'actions'];
    }
    else if (currenttab == 1) {
      this.displayedColumns = ['date', 'code', 'dcCode', 'distributor', 'createdBy', 'processedDate', 'processedBy', 'actions'];
    }
    else if (currenttab == 2) {
      this.displayedColumns = ['date', 'code', 'dcCode', 'distributor', 'createdBy', 'approvedDate', 'approvedBy', 'actions'];
    }

    return new Promise<void>(async (resolve, reject) => {
      // Set loading indicator
      this.isLoading = true;
      this.SaleReturnFilterForm = iGPFilterForm;

      const pagingData = {
        currentPage: this.currentPage,
        take: this.pageSize
      };

      iGPFilterForm["PagingData"] = pagingData;
      let fdate = new Date(iGPFilterForm.fdate);
      let tdate = new Date(iGPFilterForm.tdate);

      iGPFilterForm['fdate'] = fdate.toLocaleDateString();
      iGPFilterForm['tdate'] = tdate.toLocaleDateString();

      // Call the service method and subscribe with the observer

      (await this.saleReturnService.getAllSaleReturns(iGPFilterForm)).subscribe({
        next: (data: any) => {
          // Update data source for MatTable
          this.dataSource = new MatTableDataSource(data.item1);
          //this.totalRows = data.iGP2; // Update totalRows

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
    this.bindData(this.SaleReturnFilterForm, this.currenttab, false); // Re-fetch data on page change
  }

  openSaleReturnDialog(element: any) {
    const dialogRef = this.dialog.open(AddSaleReturnComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.SaleReturnFilterForm, this.currenttab, false);
      this.getSaleReturnCount.emit();
    });
  }

  viewSaleReturnDialog(element: any): void {
    this.dialog.open(ViewSaleReturnComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteSaleReturnDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteSaleReturnComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.SaleReturnFilterForm, this.currenttab, false);
      this.getSaleReturnCount.emit();
    });
  }

  processSaleReturnDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessSaleReturnComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.SaleReturnFilterForm, this.currenttab, false);
      this.getSaleReturnCount.emit();
    });
  }

  approveSaleReturnDialog(element: any) {
    const dialogRef = this.dialog.open(ApproveSaleReturnComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.SaleReturnFilterForm, this.currenttab, false);
      this.getSaleReturnCount.emit();
    });
  }

  printSaleReturnDialog(element: any) {

    const dialogRef = this.dialog.open(PrintSaleReturnComponent, {
      panelClass: 'cstm_width_1100',
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
    this.bindData(this.SaleReturnFilterForm, this.currenttab, false);
    this.getSaleReturnCount.emit();
  }

  
}
