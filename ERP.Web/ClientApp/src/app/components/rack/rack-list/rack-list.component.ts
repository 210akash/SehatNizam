import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { DeleteRackComponent } from '../delete-rack/delete-rack.component';
import { ConstantService } from '../../../Service/constant.service';
import { RackService } from '../rack.service';
import { AddRackComponent } from '../add-rack/add-rack.component';
import { ViewRackComponent } from '../view-rack/view-rack.component';

@Component({
  selector: 'app-rack-list',
  templateUrl: './rack-list.component.html',
  styleUrl: './rack-list.component.css',
  standalone: false
})
export class RackListComponent  {
  dataSource: any;
  rackListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['name', 'actions'];
  isLoading = false;
  element: any;
  blob: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private rackService: RackService, private formBuilder: FormBuilder) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.rackListFilerForm = this.formBuilder.group({
      name: [''],
    });

    this.bindData();
  }

  openRackDialog(element: any): void {
    const dialogRef = this.dialog.open(AddRackComponent, {
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

  openViewRackDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewRackComponent, {
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

    let _rackListFilerForm: any = {};
    _rackListFilerForm = Object.assign(_rackListFilerForm, this.rackListFilerForm.value);
    _rackListFilerForm["PagingData"] = pagingData;

    (await this.rackService.getAllRack(_rackListFilerForm)).subscribe({
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
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openDeleteDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteRackComponent, {
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
    this.rackListFilerForm.patchValue({
      name: ''
    });
    this.bindData();
  }
}
