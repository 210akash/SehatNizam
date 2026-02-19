import { Component, EventEmitter, ViewChild,Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService } from '../../../Service/constant.service';
import { SafeHtml } from '@angular/platform-browser';
import { TransactionService } from '../../transaction/transaction.service';
import { AddBpvComponent } from '../add-bpv/add-bpv.component';
import { ViewBpvComponent } from '../view-bpv/view-bpv.component';
import { DeleteBpvComponent } from '../delete-bpv/delete-bpv.component';
import { ProcessBpvComponent } from '../process-bpv/process-bpv.component';
import { PrintBpvComponent } from '../print-bpv/print-bpv.component';
import { ApproveBpvComponent } from '../approve-bpv/approve-bpv.component';
@Component({
    selector: 'app-bpv-list',
    templateUrl: './bpv-list.component.html',
    styleUrls: ['./bpv-list.component.css'],
    standalone: false
})

export class BpvListComponent {
[x: string]: any;
  @Output() getTransactionCount: EventEmitter<void> = new EventEmitter<void>();
  TransactionFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = [];
  dataSource: any;
  take = 50;
  pageSize = 0;
  totalRows = 0;
  subcategoryList :any;
  currentUser: any;
  currenttab: any;
  History: any;
  roleList: string | undefined;
  dialogRef: any;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private transactionService: TransactionService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.TransactionFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: [],
      voucherTypeId: [4]
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }
  
  async bindData(transactionFilterForm: any,currenttab:number): Promise<void> {
  this.currenttab  = currenttab;
    if(currenttab == 0){
      this.displayedColumns = ['code','date','createdDate', 'referenceNumber','createdBy','status','actions'];
    }
    else  if(currenttab == 1){
      this.displayedColumns = ['code','date','processedDate', 'referenceNumber','processedBy','status','actions'];
    }
    else  if(currenttab == 2){
      this.displayedColumns = ['code','date','approvedDate', 'referenceNumber','approvedBy','status','actions'];
    }

    return new Promise<void>(async (resolve, reject) => {
    // Set loading indicator
    this.isLoading = true;
    this.TransactionFilterForm = transactionFilterForm;

    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    };

    transactionFilterForm["PagingData"] = pagingData;
    let fdate = new Date(transactionFilterForm.fdate);
    let tdate = new Date(transactionFilterForm.tdate);

    transactionFilterForm['fdate'] = fdate.toLocaleDateString();
    transactionFilterForm['tdate'] = tdate.toLocaleDateString();

     // Call the service method and subscribe with the observer

    (await this.transactionService.getAllTransactions(transactionFilterForm)).subscribe({
      next: (data: any) => {
        // Update data source for MatTable
        this.dataSource = new MatTableDataSource(data.item1);
        this.totalRows = data.transaction2; // Update totalRows

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
    this.bindData(this.TransactionFilterForm,this.currenttab); // Re-fetch data on page change
  }
   
  openTransactionDialog(element: any) {
    const dialogRef = this.dialog.open(AddBpvComponent, {
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.TransactionFilterForm,this.currenttab);
      this.getTransactionCount.emit();
    });
  }

  viewTransactionDialog(element: any): void {
    this.dialog.open(ViewBpvComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteTransactionDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteBpvComponent, {
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.TransactionFilterForm,this.currenttab);
      this.getTransactionCount.emit();
    });
  }

  processTransactionDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessBpvComponent, {
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.TransactionFilterForm,this.currenttab);
      this.getTransactionCount.emit();
    });
  }

  approveTransactionDialog(element: any) {
    const dialogRef = this.dialog.open(ApproveBpvComponent, {
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.TransactionFilterForm,this.currenttab);
      this.getTransactionCount.emit();
    });
  }

  printTransactionDialog(element: any) {
    const dialogRef = this.dialog.open(PrintBpvComponent, {
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
    this.bindData(this.TransactionFilterForm,this.currenttab);
  }
}
