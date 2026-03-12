import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService } from '../../../Service/constant.service';
import { AddShopOrderReturnComponent } from '../add-shoporderreturn/add-shoporderreturn.component';
import { DeleteShopOrderReturnComponent } from '../delete-shoporderreturn/delete-shoporderreturn.component';
import { ShopOrderReturnService } from '../shoporderreturn.service';
import { PrintShopOrderReturnComponent } from '../print-shoporderreturn/print-shoporderreturn.component';
import { ProcessShopOrderReturnComponent } from '../process-shoporderreturn/process-shoporderreturn.component';
import { ViewShopOrderReturnComponent } from '../view-shoporderreturn/view-shoporderreturn.component';

@Component({
  selector: 'app-shoporderreturn-list',
  templateUrl: './shoporderreturn-list.component.html',
  styleUrls: ['./shoporderreturn-list.component.css'], standalone: false
})

export class ShopOrderReturnListComponent {
  @Output() getShopOrderReturnCount: EventEmitter<void> = new EventEmitter<void>();
  ShopOrderReturnFilterForm!: FormGroup;
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
    private shopOrderReturnService: ShopOrderReturnService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.ShopOrderReturnFilterForm = this.formBuilder.group({
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
      this.displayedColumns = ['date', 'code', 'orderId', 'createdBy', 'actions'];
    }
    else if (currenttab == 1) {
      this.displayedColumns = ['date', 'code', 'orderId', 'createdBy', 'modifiedDate', 'actions'];
    }

    return new Promise<void>(async (resolve, reject) => {
      // Set loading indicator
      this.isLoading = true;
      this.ShopOrderReturnFilterForm = iGPFilterForm;

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

      (await this.shopOrderReturnService.getAllShopOrderReturns(iGPFilterForm)).subscribe({
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
    this.bindData(this.ShopOrderReturnFilterForm, this.currenttab, false); // Re-fetch data on page change
  }

  openShopOrderReturnDialog(element: any) {
    const dialogRef = this.dialog.open(AddShopOrderReturnComponent, {
      panelClass: 'cstm_width_1000',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.ShopOrderReturnFilterForm, this.currenttab, false);
      this.getShopOrderReturnCount.emit();
    });
  }

  viewShopOrderReturnDialog(element: any): void {
    this.dialog.open(ViewShopOrderReturnComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1000',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteShopOrderReturnDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteShopOrderReturnComponent, {
      panelClass: 'cstm_width_1000',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.ShopOrderReturnFilterForm, this.currenttab, false);
      this.getShopOrderReturnCount.emit();
    });
  }

  processShopOrderReturnDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessShopOrderReturnComponent, {
      panelClass: 'cstm_width_1000',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.ShopOrderReturnFilterForm, this.currenttab, false);
      this.getShopOrderReturnCount.emit();
    });
  }

  printShopOrderReturnDialog(element: any) {

    const dialogRef = this.dialog.open(PrintShopOrderReturnComponent, {
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
    this.bindData(this.ShopOrderReturnFilterForm, this.currenttab, false);
    this.getShopOrderReturnCount.emit();
  }


}