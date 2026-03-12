import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService } from '../../../Service/constant.service';
import { PrintPurchaseInvoiceComponent } from '../print-purchaseinvoice/print-purchaseinvoice.component';
import { ViewPurchaseInvoiceComponent } from '../view-purchaseinvoice/view-purchaseinvoice.component';
import { ApprovePurchaseInvoiceComponent } from '../approve-purchaseinvoice/approve-purchaseinvoice.component';
import { GRNService } from '../../grn/grn.service';
import { ProcessPurchaseInvoiceComponent } from '../process-purchaseinvoice/process-purchaseinvoice.component';

@Component({
  selector: 'app-purchaseinvoice-list',
  templateUrl: './purchaseinvoice-list.component.html',
  styleUrls: ['./purchaseinvoice-list.component.css'], standalone: false
})

export class PurchaseInvoiceListComponent {
  @Output() getpurchaseinvoiceCount1: EventEmitter<void> = new EventEmitter<void>();
  PurchaseInvoiceFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['invoiceNo', 'invoiceDate', 'grncode', 'vendor', 'actions'];
  dataSource: any;
  take = 50;
  pageSize = 0;
  totalRows = 0;
  subcategoryList: any;
  currentUser: any;
  currenttab: any;
  roleList: string | undefined;
  dialogRef: any;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private grnService: GRNService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.PurchaseInvoiceFilterForm = this.formBuilder.group({
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
      this.displayedColumns = ['invoiceDate', 'invoiceNo', 'grncode', 'vendor', 'actions'];
    }
    else if (currenttab == 1) {
      this.displayedColumns = ['invoiceDate', 'invoiceNo', 'grncode', 'vendor', 'invoiceProcessedBy', 'invoiceProcessedDate', 'actions'];
    }
    else if (currenttab == 2) {
      this.displayedColumns = ['invoiceDate', 'invoiceNo', 'grncode', 'vendor', 'invoiceAuditVerifiedBy', 'invoiceAuditVerifiedDate', 'actions'];
    }
    else if (currenttab == 3) {
      this.displayedColumns = ['invoiceDate', 'invoiceNo', 'grncode', 'vendor', 'invoiceApprovedBy', 'invoiceApprovedDate', 'actions'];
    }

    return new Promise<void>(async (resolve, reject) => {
      // Set loading indicator
      this.isLoading = true;
      this.PurchaseInvoiceFilterForm = iGPFilterForm;

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

      (await this.grnService.getAllPurchaseInvoices(iGPFilterForm)).subscribe({
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
    this.bindData(this.PurchaseInvoiceFilterForm, this.currenttab, false); // Re-fetch data on page change
  }



  viewPurchaseInvoiceDialog(element: any, check: number): void {
    const dialogRef = this.dialog.open(ViewPurchaseInvoiceComponent, {
      data: { element: element, check: check },
      maxHeight: '90vh',
      maxWidth: '90%',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.getpurchaseinvoiceCount1.emit();
      this.bindData(this.PurchaseInvoiceFilterForm, this.currenttab, false);
    });
  }

  approvePurchaseInvoiceDialog(element: any) {
    const dialogRef = this.dialog.open(ApprovePurchaseInvoiceComponent, {
      maxWidth: '90%',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.getpurchaseinvoiceCount1.emit();
      this.bindData(this.PurchaseInvoiceFilterForm, this.currenttab, false);
    });
  }

  processPurchaseInvoiceDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessPurchaseInvoiceComponent, {
      maxWidth: '90%',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.getpurchaseinvoiceCount1.emit();
      this.bindData(this.PurchaseInvoiceFilterForm, this.currenttab, false);
    });
  }

  printPurchaseInvoiceDialog(element: any) {
    const dialogRef = this.dialog.open(PrintPurchaseInvoiceComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });
  }

  filterData() {
    this.getpurchaseinvoiceCount1.emit();
    this.bindData(this.PurchaseInvoiceFilterForm, this.currenttab, false);
  }


}