import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService } from '../../../Service/constant.service';
import { AddSaleMaterialReturnComponent } from '../add-salematerialreturn/add-salematerialreturn.component';
import { DeleteSaleMaterialReturnComponent } from '../delete-salematerialreturn/delete-salematerialreturn.component';
import { SaleMaterialReturnService } from '../salematerialreturn.service';
import { PrintSaleMaterialReturnComponent } from '../print-salematerialreturn/print-salematerialreturn.component';
import { ProcessSaleMaterialReturnComponent } from '../process-salematerialreturn/process-salematerialreturn.component';
import { ViewSaleMaterialReturnComponent } from '../view-salematerialreturn/view-salematerialreturn.component';
import { ApproveSaleMaterialReturnComponent } from '../approve-salematerialreturn/approve-salematerialreturn.component';

@Component({
  selector: 'app-salematerialreturn-list',
  templateUrl: './salematerialreturn-list.component.html',
  styleUrls: ['./salematerialreturn-list.component.css'], standalone: false
})

export class SaleMaterialReturnListComponent {
  @Output() getSaleMaterialReturnCount: EventEmitter<void> = new EventEmitter<void>();
  SaleMaterialReturnFilterForm!: FormGroup;
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
    private saleMaterialReturnService: SaleMaterialReturnService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.SaleMaterialReturnFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }

  async bindData(saleMaterialReturnFilterForm: any, currenttab: number, isFromParent: boolean): Promise<void> {

    if (isFromParent == true) {
      this.currentPage = 0;
    }

    this.currenttab = currenttab;
    if (currenttab == 0) {
      this.displayedColumns = ['date', 'code', 'salematerialcode', 'customer', 'createdBy', 'actions'];
    }
    else if (currenttab == 1) {
      this.displayedColumns = ['date', 'code', 'salematerialcode', 'customer', 'createdBy', 'processedDate', 'processedBy', 'actions'];
    }
    else if (currenttab == 2) {
      this.displayedColumns = ['date', 'code', 'salematerialcode', 'customer', 'createdBy', 'approvedDate', 'approvedBy', 'actions'];
    }

    return new Promise<void>(async (resolve, reject) => {
      // Set loading indicator
      this.isLoading = true;
      this.SaleMaterialReturnFilterForm = saleMaterialReturnFilterForm;

      const pagingData = {
        currentPage: this.currentPage,
        take: this.pageSize
      };

      saleMaterialReturnFilterForm["PagingData"] = pagingData;
      let fdate = new Date(saleMaterialReturnFilterForm.fdate);
      let tdate = new Date(saleMaterialReturnFilterForm.tdate);

      saleMaterialReturnFilterForm['fdate'] = fdate.toLocaleDateString();
      saleMaterialReturnFilterForm['tdate'] = tdate.toLocaleDateString();

      // Call the service method and subscribe with the observer

      (await this.saleMaterialReturnService.getAllSaleMaterialReturns(saleMaterialReturnFilterForm)).subscribe({
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
    this.bindData(this.SaleMaterialReturnFilterForm, this.currenttab, false); // Re-fetch data on page change
  }

  openSaleMaterialReturnDialog(element: any) {
    const dialogRef = this.dialog.open(AddSaleMaterialReturnComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.SaleMaterialReturnFilterForm, this.currenttab, false);
      this.getSaleMaterialReturnCount.emit();
    });
  }

  viewSaleMaterialReturnDialog(element: any): void {
    this.dialog.open(ViewSaleMaterialReturnComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteSaleMaterialReturnDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteSaleMaterialReturnComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.SaleMaterialReturnFilterForm, this.currenttab, false);
      this.getSaleMaterialReturnCount.emit();
    });
  }

  processSaleMaterialReturnDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessSaleMaterialReturnComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.SaleMaterialReturnFilterForm, this.currenttab, false);
      this.getSaleMaterialReturnCount.emit();
    });
  }

  approveSaleMaterialReturnDialog(element: any) {
    const dialogRef = this.dialog.open(ApproveSaleMaterialReturnComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.SaleMaterialReturnFilterForm, this.currenttab, false);
      this.getSaleMaterialReturnCount.emit();
    });
  }

  printSaleMaterialReturnDialog(element: any) {

    const dialogRef = this.dialog.open(PrintSaleMaterialReturnComponent, {
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
    this.bindData(this.SaleMaterialReturnFilterForm, this.currenttab, false);
    this.getSaleMaterialReturnCount.emit();
  }


}