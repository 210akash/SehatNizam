import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService } from '../../../Service/constant.service';
import { AddIGPComponent } from '../add-igp/add-igp.component';
import { DeleteIGPComponent } from '../delete-igp/delete-igp.component';
import { IGPService } from '../igp.service';
import { PrintIGPComponent } from '../print-igp/print-igp.component';
import { ProcessIGPComponent } from '../process-igp/process-igp.component';
import { ViewIGPComponent } from '../view-igp/view-igp.component';

@Component({
  selector: 'app-igp-list',
  templateUrl: './igp-list.component.html',
  styleUrls: ['./igp-list.component.css'],
  standalone: false
})

export class IGPListComponent {
  IGPFilterForm!: FormGroup;
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
    private iGPService: IGPService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.IGPFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }

  async bindData(iGPFilterForm: any): Promise<void> {
    return new Promise<void>(async (resolve, reject) => {
      // Set loading indicator
      this.isLoading = true;
      this.IGPFilterForm = iGPFilterForm;

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

      (await this.iGPService.getAllIGPs(iGPFilterForm)).subscribe({
        next: (data: any) => {
          // Update data source for MatTable
          this.dataSource = new MatTableDataSource(data.item1);
          this.totalRows = data.iGP2; // Update totalRows

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

  pageChanged(event: PageEvent): void {
    this.take = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(this.IGPFilterForm); // Re-fetch data on page change
  }

  openIGPDialog(element: any) {
    const dialogRef = this.dialog.open(AddIGPComponent, {
      panelClass: 'cstm_width_850',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.IGPFilterForm);
    });
  }

  viewIGPDialog(element: any): void {
    this.dialog.open(ViewIGPComponent, {
      data: { element: element },
      panelClass: 'cstm_width_850',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteIGPDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteIGPComponent, {
      panelClass: 'cstm_width_850',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.IGPFilterForm);
    });
  }

  processIGPDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessIGPComponent, {
      panelClass: 'cstm_width_850',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.IGPFilterForm);
    });
  }

  // approveIGPDialog(element: any) {
  //   const dialogRef = this.dialog.open(ApproveIGPComponent, {
  //     panelClass: 'cstm_width_850',
  //     maxHeight: '90vh',
  //     data: {
  //       element: element,
  //     },
  //     disableClose: true
  //   });

  //   dialogRef.afterClosed().subscribe(result => {
  //     this.bindData(this.IGPFilterForm);
  //   });
  // }

  printIGPDialog(element: any) {
    debugger;
    const dialogRef = this.dialog.open(PrintIGPComponent, {
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
    this.bindData(this.IGPFilterForm);
  }


}
