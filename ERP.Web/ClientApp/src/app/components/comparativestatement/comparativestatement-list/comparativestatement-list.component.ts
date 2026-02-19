import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ComparativeStatementService } from '../comparativestatement.service';
import { AddComparativeStatementComponent } from '../add-comparativestatement/add-comparativestatement.component';
import { DeleteComparativeStatementComponent } from '../delete-comparativestatement/delete-comparativestatement.component';
import { ViewComparativeStatementComponent } from '../view-comparativestatement/view-comparativestatement.component';
import { CategoryService } from '../../category/category.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService } from '../../../Service/constant.service';
import { ProcessComparativeStatementComponent } from '../process-comparativestatement/process-comparativestatement.component';
import { ApproveComparativeStatementComponent } from '../approve-comparativestatement/approve-comparativestatement.component';
import { PrintComparativeStatementComponent } from '../print-comparativestatement/print-comparativestatement.component';

@Component({
    selector: 'app-comparativestatement-list',
    templateUrl: './comparativestatement-list.component.html',
    styleUrls: ['./comparativestatement-list.component.css'],
    standalone: false
})

export class ComparativeStatementListComponent {
  ComparativeStatementFilterForm!: FormGroup;
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
    private comparativestatementService: ComparativeStatementService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.ComparativeStatementFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }
  
  async bindData(comparativestatementFilterForm: any): Promise<void> {
    return new Promise<void>(async (resolve, reject) => {
    // Set loading indicator
    this.isLoading = true;
    this.ComparativeStatementFilterForm = comparativestatementFilterForm;

    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    };

    comparativestatementFilterForm["PagingData"] = pagingData;
    let fdate = new Date(comparativestatementFilterForm.fdate);
    let tdate = new Date(comparativestatementFilterForm.tdate);

    comparativestatementFilterForm['fdate'] = fdate.toLocaleDateString();
    comparativestatementFilterForm['tdate'] = tdate.toLocaleDateString();

     // Call the service method and subscribe with the observer

    (await this.comparativestatementService.getAllComparativeStatements(comparativestatementFilterForm)).subscribe({
      next: (data: any) => {
        // Update data source for MatTable
        this.dataSource = new MatTableDataSource(data.item1);
        this.totalRows = data.comparativestatement2; // Update totalRows

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
    this.bindData(this.ComparativeStatementFilterForm); // Re-fetch data on page change
  }
   
  openComparativeStatementDialog(element: any) {
    const dialogRef = this.dialog.open(AddComparativeStatementComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.ComparativeStatementFilterForm);
    });
  }

  viewComparativeStatementDialog(element: any): void {
    this.dialog.open(ViewComparativeStatementComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteComparativeStatementDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteComparativeStatementComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.ComparativeStatementFilterForm);
    });
  }

  processComparativeStatementDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessComparativeStatementComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.ComparativeStatementFilterForm);
    });
  }

  approveComparativeStatementDialog(element: any) {
    const dialogRef = this.dialog.open(ApproveComparativeStatementComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.ComparativeStatementFilterForm);
    });
  }

    printComparativeStatementDialog(element: any) {
      debugger;
      const dialogRef = this.dialog.open(PrintComparativeStatementComponent, {
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
    this.bindData(this.ComparativeStatementFilterForm);
  }
}
