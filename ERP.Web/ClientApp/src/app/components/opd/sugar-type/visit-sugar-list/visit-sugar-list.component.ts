import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { SugarTypeService } from '../sugar-type.service';
import { CreateSugarTypeComponent } from '../create-sugar-type/create-sugar-type.component';
import { DeleteSugarTypeComponent } from '../delete-sugar-type/delete-sugar-type.component';
import { ViewSugarTypeComponent } from '../view-sugar-type/view-sugar-type.component';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-sugar-type-list',
  templateUrl: './sugar-type-list.component.html',
  styleUrls: ['./sugar-type-list.component.css'],standalone: false
})

export class SugarTypeListComponent implements OnInit {
  dataSource: any;
  visitTypeListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['name', 'createdDate', 'actions'];
  isLoading = false;
  element: any;
  blob: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private visitTypeService: SugarTypeService, private formBuilder: FormBuilder) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.visitTypeListFilerForm = this.formBuilder.group({
      name: [''],
    });

    this.bindData();
  }

  openSugarTypeDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateSugarTypeComponent, {
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

  openViewSugarTypeDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewSugarTypeComponent, {
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

    let _visitTypeListFilerForm: any = {};
    _visitTypeListFilerForm = Object.assign(_visitTypeListFilerForm, this.visitTypeListFilerForm.value);
    _visitTypeListFilerForm["PagingData"] = pagingData;

    (await this.visitTypeService.getAllSugarType(_visitTypeListFilerForm)).subscribe({
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
    const dialogRef = this.dialog.open(DeleteSugarTypeComponent, {
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