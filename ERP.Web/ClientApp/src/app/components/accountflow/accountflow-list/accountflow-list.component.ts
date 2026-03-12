import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewAccountFlowComponent } from '../view-accountflow/view-accountflow.component';
import { AccountFlowService } from '../accountflow.service';
import { AddAccountFlowComponent } from '../add-accountflow/add-accountflow.component';
import { DeleteAccountFlowComponent } from '../delete-accountflow/delete-accountflow.component';

@Component({
    selector: 'app-accountflow-list',
    templateUrl: './accountflow-list.component.html',
    styleUrls: ['./accountflow-list.component.css'],
    standalone: false
})

export class AccountFlowListComponent {
  AccountFlowFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['code','name', 'createdBy','company', 'actions'];
  dataSource: any;
  take = 5;
  totalRows = 0;
  accountflowList :any;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private accountflowService: AccountFlowService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit(): Promise<void> {
    this.AccountFlowFilterForm = this.formBuilder.group({
      name: ['']
    });
    await this.bindData(); // Await bindData if it's async
    this.getaccountflowList();
  }

  async bindData(): Promise<void> {
    // Set loading indicator
    this.isLoading = true;

    // Prepare paging data
    const pagingData = {
      currentPage: this.currentPage,
      take: this.take
    };

    // Clone the form value and add paging data
    const _AccountFlowFilterForm = {
      ...this.AccountFlowFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.accountflowService.getAllAccountFlows(_AccountFlowFilterForm).subscribe({
      next: (data: any) => {
        // Update data source for MatTable
        this.dataSource = new MatTableDataSource(data.item1);
        this.totalRows = data.item2; // Update totalRows

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
  }

  pageChanged(event: PageEvent): void {
    this.take = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(); // Re-fetch data on page change
  }

  openAccountFlowDialog(element: any) {
    const dialogRef = this.dialog.open(AddAccountFlowComponent, {
      panelClass: 'cstm_width_500',
      height: 'auto',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  viewAccountFlowDialog(element: any): void {
    this.dialog.open(ViewAccountFlowComponent, {
      data: { element: element },
      panelClass: 'cstm_width_500',
      height: 'auto',
      disableClose: true
    });
  }

  deleteAccountFlowDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteAccountFlowComponent, {
      panelClass: 'cstm_width_500',
      height: 'auto',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }
  getaccountflowList() {
    let _AccountFlowFilter: any = {};
    this.accountflowService.getAllAccountFlows(_AccountFlowFilter).subscribe((data: any) => {
     this.accountflowList = data.item1;
    });
  }
  filterData() {
    this.bindData();
  }
  
}
