import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService } from '../../../Service/constant.service';
import { AddPurchaseReturnComponent } from '../add-purchasereturn/add-purchasereturn.component';
import { DeletePurchaseReturnComponent } from '../delete-purchasereturn/delete-purchasereturn.component';
import { PurchaseReturnService } from '../purchasereturn.service';
import { PrintPurchaseReturnComponent } from '../print-purchasereturn/print-purchasereturn.component';
import { ProcessPurchaseReturnComponent } from '../process-purchasereturn/process-purchasereturn.component';
import { ViewPurchaseReturnComponent } from '../view-purchasereturn/view-purchasereturn.component';
import { ApprovePurchaseReturnComponent } from '../approve-purchasereturn/approve-purchasereturn.component';

@Component({
  selector: 'app-purchasereturn-list',
  templateUrl: './purchasereturn-list.component.html',
  styleUrls: ['./purchasereturn-list.component.css'], standalone: false
})

export class PurchaseReturnListComponent {
  @Output() getPurchaseReturnCount: EventEmitter<void> = new EventEmitter<void>();
  PurchaseReturnFilterForm!: FormGroup;
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
    private purchaseReturnService: PurchaseReturnService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.PurchaseReturnFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }

  async bindData(purchaseReturnFilterForm: any, currenttab: number, isFromParent: boolean): Promise<void> {

    if (isFromParent == true) {
      this.currentPage = 0;
    }

    this.currenttab = currenttab;
    if (currenttab == 0) {
      this.displayedColumns = ['date', 'code', 'invoiceNo', 'grncode', 'pocode', 'vendor', 'createdBy', 'actions'];
    }
    else if (currenttab == 1) {
      this.displayedColumns = ['date', 'code', 'invoiceNo', 'grncode', 'pocode', 'vendor', 'createdBy', 'processedDate', 'processedBy', 'actions'];
    }
    else if (currenttab == 2) {
      this.displayedColumns = ['date', 'code', 'invoiceNo', 'grncode', 'pocode', 'vendor', 'createdBy', 'approvedDate', 'approvedBy', 'actions'];
    }

    return new Promise<void>(async (resolve, reject) => {
      // Set loading indicator
      this.isLoading = true;
      this.PurchaseReturnFilterForm = purchaseReturnFilterForm;

      const pagingData = {
        currentPage: this.currentPage,
        take: this.pageSize
      };

      purchaseReturnFilterForm["PagingData"] = pagingData;
      let fdate = new Date(purchaseReturnFilterForm.fdate);
      let tdate = new Date(purchaseReturnFilterForm.tdate);

      purchaseReturnFilterForm['fdate'] = fdate.toLocaleDateString();
      purchaseReturnFilterForm['tdate'] = tdate.toLocaleDateString();

      // Call the service method and subscribe with the observer

      (await this.purchaseReturnService.getAllPurchaseReturns(purchaseReturnFilterForm)).subscribe({
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
    this.bindData(this.PurchaseReturnFilterForm, this.currenttab, false); // Re-fetch data on page change
  }

  openPurchaseReturnDialog(element: any) {
    const dialogRef = this.dialog.open(AddPurchaseReturnComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.PurchaseReturnFilterForm, this.currenttab, false);
      this.getPurchaseReturnCount.emit();
    });
  }

  viewPurchaseReturnDialog(element: any): void {
    this.dialog.open(ViewPurchaseReturnComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deletePurchaseReturnDialog(element: any) {
    const dialogRef = this.dialog.open(DeletePurchaseReturnComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.PurchaseReturnFilterForm, this.currenttab, false);
      this.getPurchaseReturnCount.emit();
    });
  }

  processPurchaseReturnDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessPurchaseReturnComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.PurchaseReturnFilterForm, this.currenttab, false);
      this.getPurchaseReturnCount.emit();
    });
  }

  approvePurchaseReturnDialog(element: any) {
    const dialogRef = this.dialog.open(ApprovePurchaseReturnComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.PurchaseReturnFilterForm, this.currenttab, false);
      this.getPurchaseReturnCount.emit();
    });
  }

  printPurchaseReturnDialog(element: any) {

    const dialogRef = this.dialog.open(PrintPurchaseReturnComponent, {
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
    this.bindData(this.PurchaseReturnFilterForm, this.currenttab, false);
    this.getPurchaseReturnCount.emit();
  }


}