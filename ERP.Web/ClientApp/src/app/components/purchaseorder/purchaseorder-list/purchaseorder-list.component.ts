import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { PurchaseOrderService } from '../purchaseorder.service';
import { AddPurchaseOrderComponent } from '../add-purchaseorder/add-purchaseorder.component';
import { DeletePurchaseOrderComponent } from '../delete-purchaseorder/delete-purchaseorder.component';
import { ViewPurchaseOrderComponent } from '../view-purchaseorder/view-purchaseorder.component';
import { CategoryService } from '../../category/category.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService } from '../../../Service/constant.service';
import { ProcessPurchaseOrderComponent } from '../process-purchaseorder/process-purchaseorder.component';
import { ApprovePurchaseOrderComponent } from '../approve-purchaseorder/approve-purchaseorder.component';
import { PrintPurchaseOrderComponent } from '../print-purchaseorder/print-purchaseorder.component';

@Component({
  selector: 'app-purchaseorder-list',
  templateUrl: './purchaseorder-list.component.html',
  styleUrls: ['./purchaseorder-list.component.css'],
  standalone: false
})

export class PurchaseOrderListComponent {
  PurchaseOrderFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['code', 'createdDate', 'createdBy', 'status', 'actions'];
  dataSource: any;
  take = 50;
  pageSize = 0;
  totalRows = 0;
  subcategoryList: any;
  currentUser: any;
  roleList: string | undefined;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private purchaseorderService: PurchaseOrderService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.PurchaseOrderFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }

  async bindData(purchaseorderFilterForm: any): Promise<void> {
    return new Promise<void>(async (resolve, reject) => {
      // Set loading indicator
      this.isLoading = true;
      this.PurchaseOrderFilterForm = purchaseorderFilterForm;

      const pagingData = {
        currentPage: this.currentPage,
        take: this.pageSize
      };

      purchaseorderFilterForm["PagingData"] = pagingData;
      let fdate = new Date(purchaseorderFilterForm.fdate);
      let tdate = new Date(purchaseorderFilterForm.tdate);

      purchaseorderFilterForm['fdate'] = fdate.toLocaleDateString();
      purchaseorderFilterForm['tdate'] = tdate.toLocaleDateString();

      // Call the service method and subscribe with the observer

      (await this.purchaseorderService.getAllPurchaseOrders(purchaseorderFilterForm)).subscribe({
        next: (data: any) => {
          // Update data source for MatTable
          this.dataSource = new MatTableDataSource(data.item1);
          this.totalRows = data.purchaseorder2; // Update totalRows

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
    });
  }

  pageChanged(event: PageEvent): void {
    this.take = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(this.PurchaseOrderFilterForm); // Re-fetch data on page change
  }

  openPurchaseOrderDialog(element: any) {
    const dialogRef = this.dialog.open(AddPurchaseOrderComponent, {
      panelClass: 'cstm_width_1000',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.PurchaseOrderFilterForm);
    });
  }

  viewPurchaseOrderDialog(element: any): void {
    this.dialog.open(ViewPurchaseOrderComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1000',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deletePurchaseOrderDialog(element: any) {
    const dialogRef = this.dialog.open(DeletePurchaseOrderComponent, {
      panelClass: 'cstm_width_1000',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.PurchaseOrderFilterForm);
    });
  }

  processPurchaseOrderDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessPurchaseOrderComponent, {
      panelClass: 'cstm_width_1000',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.PurchaseOrderFilterForm);
    });
  }

  approvePurchaseOrderDialog(element: any) {
    const dialogRef = this.dialog.open(ApprovePurchaseOrderComponent, {
      panelClass: 'cstm_width_1000',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.PurchaseOrderFilterForm);
    });
  }

  printPurchaseOrderDialog(element: any) {
    const dialogRef = this.dialog.open(PrintPurchaseOrderComponent, {
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
    this.bindData(this.PurchaseOrderFilterForm);
  }
}
