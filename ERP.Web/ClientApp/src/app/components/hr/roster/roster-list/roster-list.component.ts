import { Component, EventEmitter, ViewChild, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { SafeHtml } from '@angular/platform-browser';
import { PrintRosterComponent } from '../print-roster/print-roster.component';
import { RosterService } from '../roster.service';
import { AddRosterComponent } from '../add-roster/add-roster.component';
import { DeleteRosterComponent } from '../delete-roster/delete-roster.component';
import { ViewRosterComponent } from '../view-roster/view-roster.component';
import { ProcessRosterComponent } from '../process-roster/process-roster.component';
import { ApproveRosterComponent } from '../approve-roster/approve-roster.component';
import { ConstantService } from '../../../../Service/constant.service';
@Component({
  selector: 'app-roster-list',
  templateUrl: './roster-list.component.html',
  styleUrls: ['./roster-list.component.css'],
  standalone: false
})

export class RosterListComponent {
  [x: string]: any;
  @Output() getRosterCount: EventEmitter<void> = new EventEmitter<void>();
  RosterFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = [];
  dataSource: any;
  take = 50;
  pageSize = 0;
  totalRows = 0;
  subcategoryList: any;
  currentUser: any;
  currenttab: any;
  History: any;
  roleList: string | undefined;
  dialogRef: any;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private rosterService: RosterService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.RosterFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }

  async bindData(rosterFilterForm: any, currenttab: number, isFromParent: boolean): Promise<void> {

    if (isFromParent == true) {
      this.currentPage = 0;
    }

    this.currenttab = currenttab;
    if (currenttab == 0) {
      this.displayedColumns = ['code', 'createdDate', 'product', 'createdBy', 'status', 'actions'];
    }
    else if (currenttab == 1) {
      this.displayedColumns = ['code', 'processedDate', 'product', 'processedBy', 'status', 'actions'];
    }
    else if (currenttab == 2) {
      this.displayedColumns = ['code', 'approvedDate', 'product', 'approvedBy', 'status', 'actions'];
    }

    return new Promise<void>(async (resolve, reject) => {
      // Set loading indicator
      this.isLoading = true;
      this.RosterFilterForm = rosterFilterForm;

      const pagingData = {
        currentPage: this.currentPage,
        take: this.pageSize
      };

      rosterFilterForm["PagingData"] = pagingData;
      let fdate = new Date(rosterFilterForm.fdate);
      let tdate = new Date(rosterFilterForm.tdate);

      rosterFilterForm['fdate'] = fdate.toLocaleDateString();
      rosterFilterForm['tdate'] = tdate.toLocaleDateString();

      // Call the service method and subscribe with the observer

      (await this.rosterService.getAllRosters(rosterFilterForm)).subscribe({
        next: (data: any) => {
          // Update data source for MatTable
          this.dataSource = new MatTableDataSource(data.item1);
          //this.totalRows = data.item2; // Update totalRows

          // Set up sorting
          this.dataSource.sort = this.sort;

          // If there is data, adjust paginator settings after a short delay
          if (data.item1.length > 0) {
            setTimeout(() => {
              this.paginator.pageIndex = this.currentPage;
              this.paginator.length = data.item2;
            });
          }

          // Reset loading indicator
          this.isLoading = false;
        },
        error: (error: any) => {
          // Handle errors
          console.error('Error fetching data:', error);
          this.isLoading = false;
        }
      });
    });
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(this.RosterFilterForm, this.currenttab, false); // Re-fetch data on page change
  }

  openRosterDialog(element: any) {
    const dialogRef = this.dialog.open(AddRosterComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.RosterFilterForm, this.currenttab, false);
      this.getRosterCount.emit();
    });
  }

  viewRosterDialog(element: any): void {
    this.dialog.open(ViewRosterComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteRosterDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteRosterComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.RosterFilterForm, this.currenttab, false);
      this.getRosterCount.emit();
    });
  }

  processRosterDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessRosterComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.RosterFilterForm, this.currenttab, false);
      this.getRosterCount.emit();
    });
  }

  approveRosterDialog(element: any) {
    const dialogRef = this.dialog.open(ApproveRosterComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.RosterFilterForm, this.currenttab, false);
      this.getRosterCount.emit();
    });
  }

  printRosterDialog(element: any) {
    const dialogRef = this.dialog.open(PrintRosterComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });
  }

  filterData() {
    this.bindData(this.RosterFilterForm, this.currenttab, false);
  }


}