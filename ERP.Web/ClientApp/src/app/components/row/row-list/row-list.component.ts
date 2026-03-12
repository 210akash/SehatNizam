import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { AddRowComponent } from '../add-row/add-row.component';
import { ConstantService } from '../../../Service/constant.service';
import { RowService } from '../row.service';
import { ViewRowComponent } from '../view-row/view-row.component';
import { DeleteRowComponent } from '../delete-row/delete-row.component';
@Component({
  selector: 'app-row-list',
  templateUrl: './row-list.component.html',
  styleUrl: './row-list.component.css',
  standalone: false
})
export class RowListComponent  {
  dataSource: any;
  rowListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['name','rack' ,'actions'];
  isLoading = false;
  element: any;
  blob: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private rowService: RowService, private formBuilder: FormBuilder) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.rowListFilerForm = this.formBuilder.group({
      name: [''],
    });

    this.bindData();
  }

  openRowDialog(element: any): void {
    const dialogRef = this.dialog.open(AddRowComponent, {
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

  openViewRowDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewRowComponent, {
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

    let _rowListFilerForm: any = {};
    _rowListFilerForm = Object.assign(_rowListFilerForm, this.rowListFilerForm.value);
    _rowListFilerForm["PagingData"] = pagingData;

    (await this.rowService.getAllRow(_rowListFilerForm)).subscribe({
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
    const dialogRef = this.dialog.open(DeleteRowComponent, {
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

  filterData() {
    this.bindData();
  }

  onReset() {
    this.rowListFilerForm.patchValue({
      name: ''
    });
    this.bindData();
  }
}
