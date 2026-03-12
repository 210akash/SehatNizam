import { Component, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ConstantService } from '../../../Service/constant.service';
import { SectionService } from '../section.service';
import { AddSectionComponent } from '../add-section/add-section.component';
import { DeleteSectionComponent } from '../delete-section/delete-section.component';
import { ViewSectionComponent } from '../view-section/view-section.component';
@Component({
  selector: 'app-section-list',
  templateUrl: './section-list.component.html',
  styleUrl: './section-list.component.css',
  standalone: false
})
export class SectionListComponent  {
  dataSource: any;
  sectionListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['name','rack','row','actions'];
  isLoading = false;
  element: any;
  blob: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private sectionService: SectionService, private formBuilder: FormBuilder) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.sectionListFilerForm = this.formBuilder.group({
      name: [''],
    });

    this.bindData();
  }

  openSectionDialog(element: any): void {
    const dialogRef = this.dialog.open(AddSectionComponent, {
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

  openViewSectionDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewSectionComponent, {
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

    let _sectionListFilerForm: any = {};
    _sectionListFilerForm = Object.assign(_sectionListFilerForm, this.sectionListFilerForm.value);
    _sectionListFilerForm["PagingData"] = pagingData;

    (await this.sectionService.getAllSection(_sectionListFilerForm)).subscribe({
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
    const dialogRef = this.dialog.open(DeleteSectionComponent, {
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
    this.sectionListFilerForm.patchValue({
      name: ''
    });
    this.bindData();
  }
}
