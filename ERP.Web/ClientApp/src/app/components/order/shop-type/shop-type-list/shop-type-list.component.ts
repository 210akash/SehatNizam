import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ShopTypeService } from '../shop-type.service';
import { CreateShopTypeComponent } from '../create-shop-type/create-shop-type.component';
import { DeleteShopTypeComponent } from '../delete-shop-type/delete-shop-type.component';
import { ViewShopTypeComponent } from '../view-shop-type/view-shop-type.component';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-shop-type-list',
  templateUrl: './shop-type-list.component.html',
  styleUrls: ['./shop-type-list.component.css'],standalone: false
})

export class ShopTypeListComponent implements OnInit {
  dataSource: any;
  shopTypeListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['name', 'createdDate', 'actions'];
  isLoading = false;
  element: any;
  blob: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private shopTypeService: ShopTypeService, private formBuilder: FormBuilder) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.shopTypeListFilerForm = this.formBuilder.group({
      name: [''],
    });

    this.bindData();
  }

  openShopTypeDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateShopTypeComponent, {
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

  openViewShopTypeDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewShopTypeComponent, {
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

    let _shopTypeListFilerForm: any = {};
    _shopTypeListFilerForm = Object.assign(_shopTypeListFilerForm, this.shopTypeListFilerForm.value);
    _shopTypeListFilerForm["PagingData"] = pagingData;

    (await this.shopTypeService.getAllShopType(_shopTypeListFilerForm)).subscribe({
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
    const dialogRef = this.dialog.open(DeleteShopTypeComponent, {
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