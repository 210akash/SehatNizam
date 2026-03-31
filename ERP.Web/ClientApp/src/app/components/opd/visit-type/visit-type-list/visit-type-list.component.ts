import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { VisitTypeService } from '../visit-type.service';
import { CreateVisitTypeComponent } from '../create-visit-type/create-visit-type.component';
import { DeleteVisitTypeComponent } from '../delete-visit-type/delete-visit-type.component';
import { ViewVisitTypeComponent } from '../view-visit-type/view-visit-type.component';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-visit-type-list',
  templateUrl: './visit-type-list.component.html',
  styleUrls: ['./visit-type-list.component.css'],standalone: false
})

export class VisitTypeListComponent implements OnInit {
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

  constructor(private constantService: ConstantService, private dialog: MatDialog, private visitTypeService: VisitTypeService, private formBuilder: FormBuilder) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.visitTypeListFilerForm = this.formBuilder.group({
      name: [''],
    });

    this.bindData();
  }

  openVisitTypeDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateVisitTypeComponent, {
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

  openViewVisitTypeDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewVisitTypeComponent, {
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

    (await this.visitTypeService.getAllVisitType(_visitTypeListFilerForm)).subscribe({
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
    const dialogRef = this.dialog.open(DeleteVisitTypeComponent, {
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