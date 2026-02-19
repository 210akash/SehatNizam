import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { PaymentModeService } from '../paymentmode.service';
import { AddPaymentModeComponent } from '../add-paymentmode/add-paymentmode.component';
import { DeletePaymentModeComponent } from '../delete-paymentmode/delete-paymentmode.component';
import { ViewPaymentModeComponent } from '../view-paymentmode/view-paymentmode.component';
import { SubcategoryService } from '../../subcategory/subcategory.service';

@Component({
    selector: 'app-paymentmode-list',
    templateUrl: './paymentmode-list.component.html',
    styleUrls: ['./paymentmode-list.component.css'],
    standalone: false
})

export class PaymentModeListComponent {
  PaymentModeFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['name', 'createdBy','company', 'actions'];
  dataSource: any;
  take = 5;
  totalRows = 0;
  subcategoryList :any;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private paymentmodeService: PaymentModeService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
  ) { }

  async ngOnInit(): Promise<void> {
    this.PaymentModeFilterForm = this.formBuilder.group({
      name: [''],
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
    const _PaymentModeFilterForm = {
      ...this.PaymentModeFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.paymentmodeService.getAllPaymentModes(_PaymentModeFilterForm).subscribe({
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

  openPaymentModeDialog(element: any) {
    const dialogRef = this.dialog.open(AddPaymentModeComponent, {
      width: '30%',
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

  viewPaymentModeDialog(element: any): void {
    this.dialog.open(ViewPaymentModeComponent, {
      data: { element: element },
      width: '30%',
      disableClose: true
    });
  }

  deletePaymentModeDialog(element: any) {
    const dialogRef = this.dialog.open(DeletePaymentModeComponent, {
      width: '30%',
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

  filterData() {
    this.bindData();
  }
}
