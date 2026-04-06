import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { TriageService } from '../triage.service';
import { CreateTriageComponent } from '../create-triage/create-triage.component';
import { DeleteTriageComponent } from '../delete-triage/delete-triage.component';
import { ViewTriageComponent } from '../view-triage/view-triage.component';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-triage-list',
  templateUrl: './triage-list.component.html',
  styleUrls: ['./triage-list.component.css'],standalone: false
})
export class TriageListComponent implements OnInit {
  dataSource: any;
  triageFilterForm!: FormGroup;
  isEditMode = false;
  displayedColumns: string[] = ['appointmentId', 'triageCategoryId', 'triagePriorityId', 'sugarTypeId', 'triageScore', 'createdDate', 'actions'];
  isLoading = false;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private triageService: TriageService, private formBuilder: FormBuilder) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.triageFilterForm = this.formBuilder.group({
      appointmentId: ['']
    });

    this.bindData();
  }

  openTriageDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateTriageComponent, {
      data: { element: element },
      width: '70%',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(() => {
      this.bindData();
    });
  }

  openViewTriageDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewTriageComponent, {
      data: { element: element },
      width: '70%',
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
    };

    let _triageFilterForm: any = {};
    _triageFilterForm = Object.assign(_triageFilterForm, this.triageFilterForm.value);
    _triageFilterForm['PagingData'] = pagingData;

    (await this.triageService.getAllTriage(_triageFilterForm)).subscribe({
      next: (data: any) => {
        this.dataSource = new MatTableDataSource(data.item1);
        if (data.item1?.length > 0) {
          setTimeout(() => {
            this.paginator.pageIndex = this.currentPage;
            this.paginator.length = data.item2;
          });
        }
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
    const dialogRef = this.dialog.open(DeleteTriageComponent, {
      data: { element: element },
      width: '30%',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.bindData();
    });
  }
}
