import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ConstantService } from '../../../../Service/constant.service';
import { ReferrerService } from '../referrer.service';
import { AddReferrerComponent } from '../add-referrer/add-referrer.component';
import { ViewReferrerComponent } from '../view-referrer/view-referrer.component';
import { DeleteReferrerComponent } from '../delete-referrer/delete-referrer.component';
import { PageEvent } from '@angular/material/paginator';


@Component({
  selector: 'app-referrer-list',
  templateUrl: './referrer-list.component.html',
  styleUrls: ['./referrer-list.component.css'],
  standalone: false
})
export class ReferrerListComponent implements OnInit {
  dataSource: any = [];
  form!: FormGroup;
  displayedColumns: string[] = ['name', 'hospital', 'actions'];
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  take = 50;
  pageSize = 0;
  totalRows = 0;

  departments: any[] = [];

  constructor(
    private dialog: MatDialog,
    private fb: FormBuilder,
    private constantReferrer: ConstantService,
    private Referrer: ReferrerService,
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      name: [''],
      hospital: [''],
      phoneNo: [''],
    });
    this.bindData();
  }

  bindData(): void {
   // Set loading indicator
    this.isLoading = true;

    // Prepare paging data
    const pagingData = {
      currentPage: this.currentPage,
      take: this.take
    };

    // Clone the form value and add paging data
    const _ReferrersFilterForm = {
      ...this.form.value,
      PagingData: pagingData
    };
    this.Referrer.getAllReferrers(_ReferrersFilterForm).subscribe({
      next: (data: any) => {
        this.dataSource = data.item1 || [];
        this.isLoading = false;
      },
      error: () => {
        this.dataSource = [];
        this.isLoading = false;
      }
    });
  }

  filterData(): void {
    this.bindData();
  }


  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(); // Re-fetch data on page change
  }

  openReferrerDialog(element: any) {
    const dialogRef = this.dialog.open(AddReferrerComponent, {
      width: '30%',
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

  viewReferrerDialog(element: any): void {
    this.dialog.open(ViewReferrerComponent, {
      data: { element: element },
      width: '30%',
      disableClose: true
    });
  }

  deleteReferrerDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteReferrerComponent, {
      width: '30%',
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
