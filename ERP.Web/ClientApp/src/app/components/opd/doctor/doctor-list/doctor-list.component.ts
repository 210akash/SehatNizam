import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { ConstantService } from '../../../../Service/constant.service';
import { DoctorService } from '../doctor.service';

@Component({
  selector: 'app-doctor-list',
  templateUrl: './doctor-list.component.html',
  styleUrls: ['./doctor-list.component.css'],
  standalone: false
})
export class DoctorListComponent implements OnInit {
  doctorFilterForm!: FormGroup;
  dataSource: any;
  displayedColumns: string[] = [
    'name',
    'department',
    'designation',
    'phoneNumber',
    'email',
    'status'
  ];
  isLoading = false;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  constructor(
    private formBuilder: FormBuilder,
    private doctorService: DoctorService,
    private constantService: ConstantService
  ) {}

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.doctorFilterForm = this.formBuilder.group({
      name: [''],
      departmentId: [null],
      employeeDesignationId: [null],
    });
    this.bindData();
  }

  async bindData(): Promise<void> {
    this.isLoading = true;
    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    };

    const filterForm = {
      ...this.doctorFilterForm.value,
      PagingData: pagingData
    };

    (await this.doctorService.getAllDoctors(filterForm)).subscribe({
      next: (data: any) => {
        this.dataSource = new MatTableDataSource(data.item1);
        this.totalRows = data.item2 ?? 0;
        if (this.sort) {
          this.dataSource.sort = this.sort;
        }
        if (data.item1?.length > 0) {
          setTimeout(() => {
            this.paginator.pageIndex = this.currentPage;
            this.paginator.length = data.item2;
          });
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  pageChanged(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }
}
