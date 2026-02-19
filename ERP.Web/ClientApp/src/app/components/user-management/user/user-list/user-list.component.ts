import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AddUserComponent } from '../add-user/add-user.component';
import { ResetpasswordComponent } from '../reset-password/reset-password.component';
import { UserService } from '../../user.service';

@Component({
    selector: 'app-user-list',
    templateUrl: './user-list.component.html',
    styleUrl: './user-list.component.css',
    standalone: false
})

export class UserListComponent {
  userFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['firstName', 'lastName', 'email', 'company', 'department', 'isActive', 'actions'];
  dataSource: any;
  take = 50;
  totalRows = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private userService: UserService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit(): Promise<void> {
    this.userFilterForm = this.formBuilder.group({});
    await this.bindData();
  }

  async bindData(): Promise<void> {
    this.isLoading = true;

    this.userService.getAllUsers().subscribe({
      next: (data: any) => {
        this.dataSource = new MatTableDataSource(data);
        this.totalRows = data;
        this.dataSource.sort = this.sort;
        this.isLoading = false;
      },
      error: (error: any) => {
        console.error('Error fetching data:', error);
        this.isLoading = false;
      }
    });
  }

  pageChanged(event: PageEvent): void {
    this.take = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openUserDialog(element: any) {
    const dialogRef = this.dialog.open(AddUserComponent, {
      panelClass: 'cstm_width_800',
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

  openResetPasswordDialog(element: any): void {
    this.dialog.open(ResetpasswordComponent, {
      data: { element: element },
      disableClose: true
    });
  }


}
