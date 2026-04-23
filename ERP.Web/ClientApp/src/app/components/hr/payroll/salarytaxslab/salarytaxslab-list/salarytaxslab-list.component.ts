import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewSalaryTaxSlabComponent } from '../view-salarytaxslab/view-salarytaxslab.component';
import { AddSalaryTaxSlabComponent } from '../add-salarytaxslab/add-salarytaxslab.component';
import { DeleteSalaryTaxSlabComponent } from '../delete-salarytaxslab/delete-salarytaxslab.component';
import { SalaryTaxSlabService } from '../salarytaxslab.service';

@Component({
    selector: 'app-salarytaxslab-list',
    templateUrl: './salarytaxslab-list.component.html',
    styleUrls: ['./salarytaxslab-list.component.css'],
    standalone: false
})

export class SalaryTaxSlabListComponent {
  SalaryTaxSlabFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSize = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['fromAmount', 'toAmount', 'percentage',  'actions'];
  dataSource: any;
  take = 50;
  totalRows = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private salarytaxslabService: SalaryTaxSlabService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit(): Promise<void> {
    this.SalaryTaxSlabFilterForm = this.formBuilder.group({
    });
    await this.bindData(); // Await bindData if it's async
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
    const _SalaryTaxSlabFilterForm = {
      ...this.SalaryTaxSlabFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.salarytaxslabService.getAllHryear(_SalaryTaxSlabFilterForm).subscribe({
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
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(); // Re-fetch data on page change
  }

  openSalaryTaxSlabDialog(element: any) {
    const dialogRef = this.dialog.open(AddSalaryTaxSlabComponent, {
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

  viewSalaryTaxSlabDialog(element: any): void {
    this.dialog.open(ViewSalaryTaxSlabComponent, {
      data: { element: element },
     panelClass: 'cstm_width_500',
     height: 'auto',
      disableClose: true
    });
  }

  deleteSalaryTaxSlabDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteSalaryTaxSlabComponent, {
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


}
