import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { PurchaseDemandService } from '../purchasedemand.service';
import { AddPurchaseDemandComponent } from '../add-purchasedemand/add-purchasedemand.component';
import { DeletePurchaseDemandComponent } from '../delete-purchasedemand/delete-purchasedemand.component';
import { ViewPurchaseDemandComponent } from '../view-purchasedemand/view-purchasedemand.component';
import { CategoryService } from '../../category/category.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService } from '../../../Service/constant.service';
import { ProcessPurchaseDemandComponent } from '../process-purchasedemand/process-purchasedemand.component';
import { ApprovePurchaseDemandComponent } from '../approve-purchasedemand/approve-purchasedemand.component';
import { PrintPurchaseDemandComponent } from '../print-purchasedemand/print-purchasedemand.component';

@Component({
    selector: 'app-purchasedemand-list',
    templateUrl: './purchasedemand-list.component.html',
    styleUrls: ['./purchasedemand-list.component.css'],
    standalone: false
})

export class PurchaseDemandListComponent {
  PurchaseDemandFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['code','createdDate','createdBy','status','actions'];
  dataSource: any;
  take = 50;
  pageSize = 0;
  totalRows = 0;
  subcategoryList :any;
  currentUser: any;
  roleList: string | undefined;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private purchasedemandService: PurchaseDemandService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.PurchaseDemandFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }
  
  async bindData(purchasedemandFilterForm: any): Promise<void> {
    return new Promise<void>(async (resolve, reject) => {
    // Set loading indicator
    this.isLoading = true;
    this.PurchaseDemandFilterForm = purchasedemandFilterForm;

    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    };

    purchasedemandFilterForm["PagingData"] = pagingData;
    let fdate = new Date(purchasedemandFilterForm.fdate);
    let tdate = new Date(purchasedemandFilterForm.tdate);

    purchasedemandFilterForm['fdate'] = fdate.toLocaleDateString();
    purchasedemandFilterForm['tdate'] = tdate.toLocaleDateString();

     // Call the service method and subscribe with the observer

    (await this.purchasedemandService.getAllPurchaseDemands(purchasedemandFilterForm)).subscribe({
      next: (data: any) => {
        // Update data source for MatTable
        this.dataSource = new MatTableDataSource(data.item1);
        this.totalRows = data.purchasedemand2; // Update totalRows

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
    this.bindData(this.PurchaseDemandFilterForm); // Re-fetch data on page change
  }
   
  openPurchaseDemandDialog(element: any) {
    const dialogRef = this.dialog.open(AddPurchaseDemandComponent, {
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.PurchaseDemandFilterForm);
    });
  }

  viewPurchaseDemandDialog(element: any): void {
    this.dialog.open(ViewPurchaseDemandComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deletePurchaseDemandDialog(element: any) {
    const dialogRef = this.dialog.open(DeletePurchaseDemandComponent, {
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.PurchaseDemandFilterForm);
    });
  }

  processPurchaseDemandDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessPurchaseDemandComponent, {
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.PurchaseDemandFilterForm);
    });
  }

  approvePurchaseDemandDialog(element: any) {
    const dialogRef = this.dialog.open(ApprovePurchaseDemandComponent, {
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.PurchaseDemandFilterForm);
    });
  }

    printPurchaseDemandDialog(element: any) {
      const dialogRef = this.dialog.open(PrintPurchaseDemandComponent, {
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
    this.bindData(this.PurchaseDemandFilterForm);
  }
}
