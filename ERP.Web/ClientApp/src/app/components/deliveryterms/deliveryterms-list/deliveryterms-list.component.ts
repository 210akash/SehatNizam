import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { DeliveryTermsService } from '../deliveryterms.service';
import { AddDeliveryTermsComponent } from '../add-deliveryterms/add-deliveryterms.component';
import { DeleteDeliveryTermsComponent } from '../delete-deliveryterms/delete-deliveryterms.component';
import { ViewDeliveryTermsComponent } from '../view-deliveryterms/view-deliveryterms.component';

@Component({
    selector: 'app-deliveryterms-list',
    templateUrl: './deliveryterms-list.component.html',
    styleUrls: ['./deliveryterms-list.component.css'],
    standalone: false
})

export class DeliveryTermsListComponent {
  DeliveryTermsFilterForm!: FormGroup;
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
    private deliverytermsService: DeliveryTermsService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
  ) { }

  async ngOnInit(): Promise<void> {
    this.DeliveryTermsFilterForm = this.formBuilder.group({
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
    const _DeliveryTermsFilterForm = {
      ...this.DeliveryTermsFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.deliverytermsService.getAllDeliveryTerms(_DeliveryTermsFilterForm).subscribe({
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

  openDeliveryTermsDialog(element: any) {
    const dialogRef = this.dialog.open(AddDeliveryTermsComponent, {
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

  viewDeliveryTermsDialog(element: any): void {
    this.dialog.open(ViewDeliveryTermsComponent, {
      data: { element: element },
      width: '30%',
      disableClose: true
    });
  }

  deleteDeliveryTermsDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteDeliveryTermsComponent, {
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
