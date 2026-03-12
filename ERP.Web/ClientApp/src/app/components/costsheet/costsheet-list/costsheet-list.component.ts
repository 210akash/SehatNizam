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
import { PrintCostSheetComponent } from '../print-costsheet/print-costsheet.component';
import { CostSheetService } from '../costsheet.service';
import { AddCostSheetComponent } from '../add-costsheet/add-costsheet.component';
import { DeleteCostSheetComponent } from '../delete-costsheet/delete-costsheet.component';
import { ViewCostSheetComponent } from '../view-costsheet/view-costsheet.component';
import { ProcessCostSheetComponent } from '../process-costsheet/process-costsheet.component';
import { ApproveCostSheetComponent } from '../approve-costsheet/approve-costsheet.component';
@Component({
  selector: 'app-costsheet-list',
  templateUrl: './costsheet-list.component.html',
  styleUrls: ['./costsheet-list.component.css'],
  standalone: false
})

export class CostsheetListComponent {
  [x: string]: any;
  @Output() getCostsheetCount: EventEmitter<void> = new EventEmitter<void>();
  CostsheetFilterForm!: FormGroup;
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
    private costsheetService: CostSheetService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.CostsheetFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }

  async bindData(costsheetFilterForm: any, currenttab: number, isFromParent: boolean): Promise<void> {

    if (isFromParent == true) {
      this.currentPage = 0;
    }

    this.currenttab = currenttab;
    if (currenttab == 0) {
      this.displayedColumns = ['code', 'createdDate', 'product', 'createdBy', 'status', 'actions'];
    }
    else if (currenttab == 1) {
      this.displayedColumns = ['code', 'processedDate', 'product', 'processedBy', 'status', 'actions'];
    }
    else if (currenttab == 2) {
      this.displayedColumns = ['code', 'approvedDate', 'product', 'approvedBy', 'status', 'actions'];
    }

    return new Promise<void>(async (resolve, reject) => {
      // Set loading indicator
      this.isLoading = true;
      this.CostsheetFilterForm = costsheetFilterForm;

      const pagingData = {
        currentPage: this.currentPage,
        take: this.pageSize
      };

      costsheetFilterForm["PagingData"] = pagingData;
      let fdate = new Date(costsheetFilterForm.fdate);
      let tdate = new Date(costsheetFilterForm.tdate);

      costsheetFilterForm['fdate'] = fdate.toLocaleDateString();
      costsheetFilterForm['tdate'] = tdate.toLocaleDateString();

      // Call the service method and subscribe with the observer

      (await this.costsheetService.getAllCostSheets(costsheetFilterForm)).subscribe({
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
    this.bindData(this.CostsheetFilterForm, this.currenttab, false); // Re-fetch data on page change
  }

  openCostsheetDialog(element: any) {
    const dialogRef = this.dialog.open(AddCostSheetComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.CostsheetFilterForm, this.currenttab, false);
      this.getCostsheetCount.emit();
    });
  }

  viewCostsheetDialog(element: any): void {
    this.dialog.open(ViewCostSheetComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteCostsheetDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteCostSheetComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.CostsheetFilterForm, this.currenttab, false);
      this.getCostsheetCount.emit();
    });
  }

  processCostsheetDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessCostSheetComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.CostsheetFilterForm, this.currenttab, false);
      this.getCostsheetCount.emit();
    });
  }

  approveCostsheetDialog(element: any) {
    const dialogRef = this.dialog.open(ApproveCostSheetComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.CostsheetFilterForm, this.currenttab, false);
      this.getCostsheetCount.emit();
    });
  }

  printCostsheetDialog(element: any) {
    const dialogRef = this.dialog.open(PrintCostSheetComponent, {
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
    this.bindData(this.CostsheetFilterForm, this.currenttab, false);
  }


}