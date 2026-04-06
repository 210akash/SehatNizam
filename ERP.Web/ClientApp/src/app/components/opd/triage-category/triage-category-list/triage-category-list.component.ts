import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { TriageCategoryService } from '../triage-category.service';
import { CreateTriageCategoryComponent } from '../create-triage-category/create-triage-category.component';
import { DeleteTriageCategoryComponent } from '../delete-triage-category/delete-triage-category.component';
import { ViewTriageCategoryComponent } from '../view-triage-category/view-triage-category.component';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-triage-category-list',
  templateUrl: './triage-category-list.component.html',
  styleUrls: ['./triage-category-list.component.css'],standalone: false
})

export class TriageCategoryListComponent implements OnInit {
  dataSource: any;
  appointmentTypeListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['name', 'createdDate', 'actions'];
  isLoading = false;
  element: any;
  blob: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private appointmentTypeService: TriageCategoryService, private formBuilder: FormBuilder) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.appointmentTypeListFilerForm = this.formBuilder.group({
      name: [''],
    });

    this.bindData();
  }

  openTriageCategoryDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateTriageCategoryComponent, {
      data: { element: element },
      width: '30%',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openViewTriageCategoryDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewTriageCategoryComponent, {
      data: { element: element },
      width: '30%',
      autoFocus: true,
      disableClose: true
    }),
    {
      enterAnimationDuration,
      exitAnimationDuration,
    };
  }

  async bindData() {
    this.isLoading = true;
    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    }

    let _appointmentTypeListFilerForm: any = {};
    _appointmentTypeListFilerForm = Object.assign(_appointmentTypeListFilerForm, this.appointmentTypeListFilerForm.value);
    _appointmentTypeListFilerForm["PagingData"] = pagingData;

    (await this.appointmentTypeService.getAllTriageCategory(_appointmentTypeListFilerForm)).subscribe({
      next: (data: any) => {
        this.dataSource = new MatTableDataSource(data.item1);
        if (data.item1.length > 0) {
          setTimeout(() => {
            this.paginator.pageIndex = this.currentPage;
            this.paginator.length = data.item2;
          });
        }
        console.log(this.dataSource);
        this.isLoading = false;
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  pageChanged(event: PageEvent) {
    console.log({ event });
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openDeleteDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteTriageCategoryComponent, {
      data: { element: element },
      width: '30%',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }


}