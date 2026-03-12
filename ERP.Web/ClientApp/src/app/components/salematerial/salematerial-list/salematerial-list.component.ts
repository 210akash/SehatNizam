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
import { PrintSaleMaterialComponent } from '../print-salematerial/print-salematerial.component';
import { SaleMaterialService } from '../salematerial.service';
import { AddSaleMaterialComponent } from '../add-salematerial/add-salematerial.component';
import { DeleteSaleMaterialComponent } from '../delete-salematerial/delete-salematerial.component';
import { ViewSaleMaterialComponent } from '../view-salematerial/view-salematerial.component';
import { ProcessSaleMaterialComponent } from '../process-salematerial/process-salematerial.component';
import { ApproveSaleMaterialComponent } from '../approve-salematerial/approve-salematerial.component';
@Component({
  selector: 'app-salematerial-list',
  templateUrl: './salematerial-list.component.html',
  styleUrls: ['./salematerial-list.component.css'],
  standalone: false
})

export class SaleMaterialListComponent {
  [x: string]: any;
  @Output() getSaleMaterialCount: EventEmitter<void> = new EventEmitter<void>();
  SaleMaterialFilterForm!: FormGroup;
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
    private salematerialService: SaleMaterialService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.SaleMaterialFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }

  async bindData(salematerialFilterForm: any, currenttab: number, isFromParent: boolean): Promise<void> {

    if (isFromParent == true) {
      this.currentPage = 0;
    }

    this.currenttab = currenttab;
    if (currenttab == 0) {
      this.displayedColumns = ['code', 'customer', 'createdDate', 'createdBy', 'status', 'actions'];
    }
    else if (currenttab == 1) {
      this.displayedColumns = ['code', 'customer', 'processedDate', 'processedBy', 'status', 'actions'];
    }
    else if (currenttab == 2) {
      this.displayedColumns = ['code', 'customer', 'approvedDate', 'approvedBy', 'status', 'actions'];
    }

    return new Promise<void>(async (resolve, reject) => {
      // Set loading indicator
      this.isLoading = true;
      this.SaleMaterialFilterForm = salematerialFilterForm;

      const pagingData = {
        currentPage: this.currentPage,
        take: this.pageSize
      };

      salematerialFilterForm["PagingData"] = pagingData;
      let fdate = new Date(salematerialFilterForm.fdate);
      let tdate = new Date(salematerialFilterForm.tdate);

      salematerialFilterForm['fdate'] = fdate.toLocaleDateString();
      salematerialFilterForm['tdate'] = tdate.toLocaleDateString();

      // Call the service method and subscribe with the observer

      (await this.salematerialService.getAllSaleMaterials(salematerialFilterForm)).subscribe({
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
    this.bindData(this.SaleMaterialFilterForm, this.currenttab, false); // Re-fetch data on page change
  }

  openSaleMaterialDialog(element: any) {
    const dialogRef = this.dialog.open(AddSaleMaterialComponent, {
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.SaleMaterialFilterForm, this.currenttab, false);
      this.getSaleMaterialCount.emit();
    });
  }

  viewSaleMaterialDialog(element: any): void {
    this.dialog.open(ViewSaleMaterialComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteSaleMaterialDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteSaleMaterialComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.SaleMaterialFilterForm, this.currenttab, false);
      this.getSaleMaterialCount.emit();
    });
  }

  processSaleMaterialDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessSaleMaterialComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.SaleMaterialFilterForm, this.currenttab, false);
      this.getSaleMaterialCount.emit();
    });
  }

  approveSaleMaterialDialog(element: any) {
    const dialogRef = this.dialog.open(ApproveSaleMaterialComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.SaleMaterialFilterForm, this.currenttab, false);
      this.getSaleMaterialCount.emit();
    });
  }

  printSaleMaterialDialog(element: any) {
    const dialogRef = this.dialog.open(PrintSaleMaterialComponent, {
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
    this.bindData(this.SaleMaterialFilterForm, this.currenttab, false);
  }


}