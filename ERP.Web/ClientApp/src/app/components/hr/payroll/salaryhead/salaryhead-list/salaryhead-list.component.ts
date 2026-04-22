import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewSalaryHeadComponent } from '../view-salaryhead/view-salaryhead.component';
import { AddSalaryHeadComponent } from '../add-salaryhead/add-salaryhead.component';
import { DeleteSalaryHeadComponent } from '../delete-salaryhead/delete-salaryhead.component';
import { SalaryHeadService } from '../salaryhead.service';
import { ConstantService, SalaryHeadTypeEnum } from '../../../../../Service/constant.service';

@Component({
    selector: 'app-salaryhead-list',
    templateUrl: './salaryhead-list.component.html',
    styleUrls: ['./salaryhead-list.component.css'],
    standalone: false
})

export class SalaryHeadListComponent {
  SalaryHeadFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSize = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['name', 'type' , 'actions'];
  dataSource: any;
  take = 50;
  totalRows = 0;
  salaryHeadTypes: { [key: number]: string } = {};

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private salaryheadService: SalaryHeadService,
    private constantService: ConstantService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit(): Promise<void> {
    this.salaryHeadTypes = Object.keys(SalaryHeadTypeEnum)
      .filter(key => isNaN(Number(key))) // Filter out numeric keys
      .reduce((acc, key) => {
        const value = SalaryHeadTypeEnum[key as keyof typeof SalaryHeadTypeEnum];
        acc[value] = key; // Map numeric value to string name
        return acc;
      }, {} as { [key: number]: string });

    this.SalaryHeadFilterForm = this.formBuilder.group({
      name: ['']
    });

    await this.bindData();
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
    const _SalaryHeadFilterForm = {
      ...this.SalaryHeadFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.salaryheadService.getAllSalaryHeads(_SalaryHeadFilterForm).subscribe({
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

  openSalaryHeadDialog(element: any) {
    const dialogRef = this.dialog.open(AddSalaryHeadComponent, {
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

  viewSalaryHeadDialog(element: any): void {
    this.dialog.open(ViewSalaryHeadComponent, {
      data: { element: element },
     panelClass: 'cstm_width_500',
     height: 'auto',
      disableClose: true
    });
  }

  deleteSalaryHeadDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteSalaryHeadComponent, {
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
