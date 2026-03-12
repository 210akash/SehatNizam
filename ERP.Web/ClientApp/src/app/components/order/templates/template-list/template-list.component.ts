import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { TemplateService } from '../template.service';
import { AddTemplateComponent } from '../add-template/add-template.component';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-template-list',
  templateUrl: './template-list.component.html',
  styleUrls: ['./template-list.component.css'], standalone: false
})

export class TemplateListComponent implements OnInit {
  dataSource: any;
  templatesListFilterForm!: FormGroup;
  isEditMode: boolean = false;

  displayedColumns: string[] = ['id', 'name', 'description', 'content', 'actions'];
  isLoading = false;

  pageSize = 0;
  currentPage = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  constructor(private dialog: MatDialog, private templateService: TemplateService, private formBuilder: FormBuilder, private constantService: ConstantService) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.templatesListFilterForm = this.formBuilder.group({
      type: [0],
    });

    this.bindData();
  }

  openAddTemplateDialog(element: any): void {
    const dialogRef = this.dialog.open(AddTemplateComponent, {
      data: { element: '' },
      width: '60%',
      maxHeight: '85vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  openEditTemplateDialog(element: any) {
    const dialogRef = this.dialog.open(AddTemplateComponent, {
      data: { element: element },
      width: '60%',
      maxHeight: '85vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  async bindData() {
    this.isLoading = true;

    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    }

    let _templatesListFilterForm: any = {};
    _templatesListFilterForm = Object.assign(_templatesListFilterForm, this.templatesListFilterForm.value);
    _templatesListFilterForm["PagingData"] = pagingData;

    (await this.templateService.getAllTemplates(_templatesListFilterForm)).subscribe({
      next: (data) => {
        this.dataSource = new MatTableDataSource(data.item1);

        if (data.item1.length > 0) {
          setTimeout(() => {
            this.paginator.pageIndex = this.currentPage;
            this.paginator.length = data.item2;
          });
        }

        this.isLoading = false;
      },
      error: (error) => {
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

}
