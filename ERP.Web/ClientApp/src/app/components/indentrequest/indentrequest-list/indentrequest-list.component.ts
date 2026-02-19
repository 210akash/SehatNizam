import { Component, EventEmitter, ViewChild,Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { IndentrequestService } from '../indentrequest.service';
import { AddIndentrequestComponent } from '../add-indentrequest/add-indentrequest.component';
import { DeleteIndentrequestComponent } from '../delete-indentrequest/delete-indentrequest.component';
import { ViewIndentrequestComponent } from '../view-indentrequest/view-indentrequest.component';
import { CategoryService } from '../../category/category.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService } from '../../../Service/constant.service';
import { ProcessIndentrequestComponent } from '../process-indentrequest/process-indentrequest.component';
import { ApproveIndentrequestComponent } from '../approve-indentrequest/approve-indentrequest.component';
import { SafeHtml } from '@angular/platform-browser';
import { PrintIndentrequestComponent } from '../print-indentrequest/print-indentrequest.component';
@Component({
    selector: 'app-indentrequest-list',
    templateUrl: './indentrequest-list.component.html',
    styleUrls: ['./indentrequest-list.component.css'],
    standalone: false
})

export class IndentrequestListComponent {
[x: string]: any;
  @Output() getIndentrequestCount: EventEmitter<void> = new EventEmitter<void>();
  IndentrequestFilterForm!: FormGroup;
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
    private indentrequestService: IndentrequestService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.IndentrequestFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }
  
  async bindData(indentrequestFilterForm: any,currenttab:number): Promise<void> {
  this.currenttab  = currenttab;
    if(currenttab == 0){
      this.displayedColumns = ['code','createdDate', 'requiredDate','store','createdBy','status','actions'];
    }
    else  if(currenttab == 1){
      this.displayedColumns = ['code','processedDate', 'requiredDate','store','processedBy','status','actions'];
    }
    else  if(currenttab == 2){
      this.displayedColumns = ['code','approvedDate', 'requiredDate','store','approvedBy','status','actions'];
    }
    else  if(currenttab == 3){
      this.displayedColumns = ['code','issuedDate', 'requiredDate','store','issuedBy','status','actions'];
    }

    return new Promise<void>(async (resolve, reject) => {
    // Set loading indicator
    this.isLoading = true;
    this.IndentrequestFilterForm = indentrequestFilterForm;

    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    };

    indentrequestFilterForm["PagingData"] = pagingData;
    let fdate = new Date(indentrequestFilterForm.fdate);
    let tdate = new Date(indentrequestFilterForm.tdate);

    indentrequestFilterForm['fdate'] = fdate.toLocaleDateString();
    indentrequestFilterForm['tdate'] = tdate.toLocaleDateString();

     // Call the service method and subscribe with the observer

    (await this.indentrequestService.getAllIndentrequests(indentrequestFilterForm)).subscribe({
      next: (data: any) => {
        // Update data source for MatTable
        this.dataSource = new MatTableDataSource(data.item1);
        this.totalRows = data.indentrequest2; // Update totalRows

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
    this.bindData(this.IndentrequestFilterForm,this.currenttab); // Re-fetch data on page change
  }
   
  openIndentrequestDialog(element: any) {
    const dialogRef = this.dialog.open(AddIndentrequestComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.IndentrequestFilterForm,this.currenttab);
      this.getIndentrequestCount.emit();
    });
  }

  viewIndentrequestDialog(element: any): void {
    this.dialog.open(ViewIndentrequestComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteIndentrequestDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteIndentrequestComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.IndentrequestFilterForm,this.currenttab);
      this.getIndentrequestCount.emit();
    });
  }

  processIndentrequestDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessIndentrequestComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.IndentrequestFilterForm,this.currenttab);
      this.getIndentrequestCount.emit();
    });
  }

  approveIndentrequestDialog(element: any) {
    const dialogRef = this.dialog.open(ApproveIndentrequestComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.IndentrequestFilterForm,this.currenttab);
      this.getIndentrequestCount.emit();
    });
  }

  printIndentrequestDialog(element: any) {
    const dialogRef = this.dialog.open(PrintIndentrequestComponent, {
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
    this.bindData(this.IndentrequestFilterForm,this.currenttab);
  }
}
