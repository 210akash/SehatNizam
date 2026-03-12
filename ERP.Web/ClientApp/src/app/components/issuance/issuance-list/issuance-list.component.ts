import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ConstantService } from '../../../Service/constant.service';
import { AddIssuanceComponent } from '../add-issuance/add-issuance.component';
import { DeleteIssuanceComponent } from '../delete-issuance/delete-issuance.component';
import { IssuanceService } from '../issuance.service';
import { PrintIssuanceComponent } from '../print-issuance/print-issuance.component';
import { ProcessIssuanceComponent } from '../process-issuance/process-issuance.component';
import { ViewIssuanceComponent } from '../view-issuance/view-issuance.component';
import { ApproveIssuanceComponent } from '../approve-issuance/approve-issuance.component';

@Component({
  selector: 'app-issuance-list',
  templateUrl: './issuance-list.component.html',
  styleUrls: ['./issuance-list.component.css'], standalone: false
})

export class IssuanceListComponent {
  @Output() getissuanceCount: EventEmitter<void> = new EventEmitter<void>();
  IssuanceFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['code', 'RequestNo', 'createdDate', 'createdBy', 'status', 'actions'];
  dataSource: any;
  take = 50;
  pageSize = 0;
  totalRows = 0;
  subcategoryList: any;
  currentUser: any;
  roleList: string | undefined;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private issuanceService: IssuanceService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private subcategoryService: SubcategoryService,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.IssuanceFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }

  async bindData(issuanceFilterForm: any, isFromParent: boolean): Promise<void> {
    return new Promise<void>(async (resolve, reject) => {

      if (isFromParent == true) {
        this.currentPage = 0;
      }

      this.isLoading = true;
      this.IssuanceFilterForm = issuanceFilterForm;

      const pagingData = {
        currentPage: this.currentPage,
        take: this.pageSize
      };

      issuanceFilterForm["PagingData"] = pagingData;
      let fdate = new Date(issuanceFilterForm.fdate);
      let tdate = new Date(issuanceFilterForm.tdate);

      issuanceFilterForm['fdate'] = fdate.toLocaleDateString();
      issuanceFilterForm['tdate'] = tdate.toLocaleDateString();

      (await this.issuanceService.getAllIssuances(issuanceFilterForm)).subscribe({
        next: (data: any) => {
          this.dataSource = new MatTableDataSource(data.item1);
          this.dataSource.sort = this.sort;
          if (data.item1.length > 0) {
            setTimeout(() => {
              this.paginator.pageIndex = this.currentPage;
              this.paginator.length = data.item2;
            });
          }
          this.isLoading = false;
        },
        error: (error: any) => {
          console.error('Error fetching data:', error);
          this.isLoading = false;
        }
      });
    });
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(this.IssuanceFilterForm, false);
  }

  openIssuanceDialog(element: any) {
    const dialogRef = this.dialog.open(AddIssuanceComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.IssuanceFilterForm, false);
      this.getissuanceCount.emit();
    });
  }

  viewIssuanceDialog(element: any): void {
    this.dialog.open(ViewIssuanceComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteIssuanceDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteIssuanceComponent, {
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.IssuanceFilterForm, false);
      this.getissuanceCount.emit();
    });
  }

  processIssuanceDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessIssuanceComponent, {
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.IssuanceFilterForm, false);
      this.getissuanceCount.emit();
    });
  }

  approveIssuanceDialog(element: any) {
    const dialogRef = this.dialog.open(ApproveIssuanceComponent, {
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.IssuanceFilterForm, false);
      this.getissuanceCount.emit();
    });
  }

  printIssuanceDialog(element: any) {

    const dialogRef = this.dialog.open(PrintIssuanceComponent, {
      panelClass: 'cstm_width_1300',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });
  }


  getcategoryList() {
    this.subcategoryService.getSubcategoryByCompany().subscribe((data: any) => {
      this.subcategoryList = data;
    });
  }

  filterData() {
    this.bindData(this.IssuanceFilterForm, false);
    this.getissuanceCount.emit();
  }


}