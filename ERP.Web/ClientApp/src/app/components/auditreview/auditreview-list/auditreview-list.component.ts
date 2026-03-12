import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService, OrderStatusEnum } from '../../../Service/constant.service';
import { DeleteAuditReviewComponent } from '../delete-auditreview/delete-auditreview.component';
import { AuditReviewService } from '../auditreview.service';
import { PrintAuditReviewComponent } from '../print-auditreview/print-auditreview.component';
import { ProcessAuditReviewComponent } from '../process-auditreview/process-auditreview.component';
import { ViewAuditReviewComponent } from '../view-auditreview/view-auditreview.component';
import { ApproveAuditReviewComponent } from '../approve-auditreview/approve-auditreview.component';
import { OrderHistoryComponent } from '../../order/primary-order/order-history/order-history.component';

@Component({
  selector: 'app-auditreview-list',
  templateUrl: './auditreview-list.component.html',
  styleUrls: ['./auditreview-list.component.css'], standalone: false
})

export class AuditReviewListComponent {
  @Output() getauditreviewCount: EventEmitter<void> = new EventEmitter<void>();
  AuditReviewFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = [];
  dataSource: any;
  take = 10;
  pageSize = 10;
  totalRows = 0;
  subcategoryList: any;
  currentUser: any;
  currenttab: any;
  roleList: string | undefined;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort
  statusEnum: any;

  constructor(
    private iGPService: AuditReviewService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.statusEnum = OrderStatusEnum;
    this.pageSize = this.constantService.defaultItemPerPage;
    this.AuditReviewFilterForm = this.formBuilder.group({
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
      this.displayedColumns = ['code', 'createdDate', 'distributor', 'processedDate', 'processedBy', 'status', 'actions'];
    }
    else if (currenttab == 1) {
      this.displayedColumns = ['code', 'createdDate', 'distributor', 'accountrevieweddate', 'accountreviewedby', 'status', 'actions'];
    }
    else if (currenttab == 2) {
      this.displayedColumns = ['code', 'createdDate', 'distributor', 'approvedate', 'approveby', 'status', 'actions'];
    }
    else if (currenttab == 3) {
      this.displayedColumns = ['code', 'createdDate', 'distributor', 'confirmdate', 'confirmby', 'status', 'actions'];
    }
    return new Promise<void>(async (resolve, reject) => {
      // Set loading indicator
      this.isLoading = true;
      this.AuditReviewFilterForm = iGPFilterForm;

      const pagingData = {
        currentPage: this.currentPage,
        take: this.take
      };

      iGPFilterForm["PagingData"] = pagingData;
      let fdate = new Date(iGPFilterForm.fdate);
      let tdate = new Date(iGPFilterForm.tdate);

      iGPFilterForm['fdate'] = fdate.toLocaleDateString();
      iGPFilterForm['tdate'] = tdate.toLocaleDateString();

      // Call the service method and subscribe with the observer

      (await this.iGPService.getAllAuditReviews(iGPFilterForm)).subscribe({
        next: (data: any) => {
          // Update data source for MatTable
          this.dataSource = new MatTableDataSource(data.item1);
          this.totalRows = data.item2; // Update totalRows

          // Set up sorting
          this.dataSource.sort = this.sort;

          // If there is data, adjust paginator settings after a short delay
          if (data.item1?.length > 0) {
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
  openOrderHistoryDialog(element: any) {
    const dialogRef = this.dialog.open(OrderHistoryComponent, {
      data: { element: element },
      width: '50%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log(`Dialog result: ${result}`);
    });
  }
  pageChanged(event: PageEvent): void {
    this.take = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(this.AuditReviewFilterForm, this.currenttab, false); // Re-fetch data on page change
  }

  viewAuditReviewDialog(element: any): void {
    this.dialog.open(ViewAuditReviewComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteAuditReviewDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteAuditReviewComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.AuditReviewFilterForm, this.currenttab, false);
      this.getauditreviewCount.emit();
    });
  }

  processAuditReviewDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessAuditReviewComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.AuditReviewFilterForm, this.currenttab, false);
      this.getauditreviewCount.emit();
    });
  }

  approveAuditReviewDialog(element: any) {
    const dialogRef = this.dialog.open(ApproveAuditReviewComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.AuditReviewFilterForm, this.currenttab, false);
      this.getauditreviewCount.emit();
    });
  }

  printAuditReviewDialog(element: any) {
    const dialogRef = this.dialog.open(PrintAuditReviewComponent, {
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
    this.bindData(this.AuditReviewFilterForm, this.currenttab, false);
    this.getauditreviewCount.emit();
  }

  getLatestOrderProcessByStatus(element: { orderProcess?: any[] }, toStatusId: number): any | null {
    if (!Array.isArray(element.orderProcess)) return null;
    const matchedProcess = element.orderProcess
      ?.filter(p => Number(p.toStatusId) === toStatusId)
      .sort((a, b) => new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime())[0] ?? null;
    return matchedProcess;
  }


}