
import { DeleteAdmissionBedComponent } from '../delete-admissionbed/delete-admissionbed.component';
import { AddAdmissionBedComponent } from '../add-admissionbed/add-admissionbed.component';
import { Component, Inject, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BedService } from '../../bed/bed.service';
import { AdmissionBedService } from '../admissionbed.service';

@Component({
  selector: 'app-admissionbed-list',
  templateUrl: './admissionbed-list.component.html',
  styleUrl: './admissionbed-list.component.css',
    standalone: false
})

export class AdmissionBedListComponent {
  BedFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['ward', 'room', 'bed', 'createdDate', 'active', 'actions'];
  dataSource: any;
  take = 50;
  totalRows = 0;

  constructor(private admissionBedService : AdmissionBedService,private bedService: BedService, private dialog: MatDialog, private formBuilder: FormBuilder, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  async ngOnInit(): Promise<void> {
    this.BedFilterForm = this.formBuilder.group({
      admissionId: [this.data.element.id]
    });
    await this.bindData();
  }

  async bindData(): Promise<void> {
    this.isLoading = true;

    const pagingData = {
      currentPage: this.currentPage,
      take: this.take
    };

    const _BedFilterForm = {
      ...this.BedFilterForm.value,
      PagingData: pagingData
    };

    this.admissionBedService.getAllAdmissionBeds(_BedFilterForm).subscribe({
      next: (data: any) => {

        this.dataSource = new MatTableDataSource(data.item1);
        this.totalRows = data.item2;

        this.dataSource.sort = this.sort;

        if (data.item1.length > 0) {
          setTimeout(() => {
            this.paginator.pageIndex = this.currentPage;
            this.paginator.length = this.totalRows;
          });
        }

        this.isLoading = false;
      },
      error: (error: any) => {

        console.error('Error fetching data:', error);
        this.isLoading = false;
      }
    });
  }

  pageChanged(event: PageEvent): void {
    this.take = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openBedDialog(element: any) {
    const dialogRef = this.dialog.open(AddAdmissionBedComponent, {
      id: 'message-tracker-dialog',
      width: '40%',
      height: 'auto',
      data: { element: element, refreshList: this.bindData.bind(this) },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  deleteBedDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteAdmissionBedComponent, {
      id: 'message-delete-tracker',
      width: '40%',
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
