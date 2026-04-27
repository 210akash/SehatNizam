import { Component, Inject, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConstantService } from '../../../../../Service/constant.service';
import { NotificationsService } from '../../../../../Service/notification.service';
import { SalaryHeadService } from '../../salaryhead/salaryhead.service';
import { EmployeeSalaryService } from '../employeesalary.service';

@Component({
    selector: 'app-add-employee-salary',
    templateUrl: './add-employee-salary.component.html',
    styleUrl: './add-employee-salary.component.css',
    standalone: false
})
export class AddEmployeeSalaryComponent {
  employeeSalaryForm!: FormGroup;
  isLoading = false;
  salaryHeadList: any[] = [];
  employeeSalaryList: any[] = [];
  displayedColumns: string[] = ['salaryHead', 'amount', 'effectiveFrom'];
  dataSource = new MatTableDataSource<any>([]);
  employeeId: number | null = null;

  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private dialog: MatDialog,
    private notificationsService: NotificationsService,
    private formBuilder: FormBuilder,
    private salaryHeadService: SalaryHeadService,
    private employeeSalaryService: EmployeeSalaryService,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.employeeId = this.data?.element?.id ?? null;
    this.initializeForm();

    this.getSalaryHeadList();
    this.getEmployeeSalaryByEmployeeId();
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
  }

  private initializeForm(): void {
    this.employeeSalaryForm = this.formBuilder.group({
      id: [0],
      salaryHeadId: [null, Validators.required],
      amount: [0, [Validators.required, Validators.min(0)]],
      effectiveFrom: [this.constantService.formatDate(new Date()), Validators.required]
    });
  }

  getEmployeeSalaryByEmployeeId(): void {
    if (!this.employeeId) {
      this.refreshDataSource([]);
      return;
    }

    this.employeeSalaryService.getEmployeeSalaryByEmployeeId(this.employeeId).subscribe({
      next: (data: any) => {
        const employeeSalary = data?.item1 ?? data?.data ?? data?.Data ?? data ?? [];
        this.refreshDataSource(Array.isArray(employeeSalary) ? employeeSalary : []);
      },
      error: (error: any) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.refreshDataSource([]);
      }
    });
  }

  private refreshDataSource(employeeSalary: any[]): void {
    this.employeeSalaryList = employeeSalary.map((item: any) => ({
      id: item?.id ?? 0,
      salaryHeadId: item?.salaryHeadId ?? null,
      amount: item?.amount ?? 0,
      effectiveFrom: item?.effectiveFrom ?? this.constantService.formatDate(new Date()),
      salaryHeadName: item?.salaryHeadName ?? item?.salaryHead?.name ?? item?.salaryHeadName
    }));
    this.dataSource.data = [...this.employeeSalaryList];
    if (this.sort) {
      this.dataSource.sort = this.sort;
    }
  }

  getSalaryHeadList(): void {
    const filter = { name: '', PagingData: { currentPage: 0, take: 500 } };
    this.salaryHeadService.getAllSalaryHeads(filter).subscribe({
      next: (data: any) => {
        this.salaryHeadList = data?.item1 ?? [];
      },
      error: (error: any) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
      }
    });
  }

  addSalaryEntry(): void {
    if (this.employeeSalaryForm.invalid) {
      this.constantService.markFormGroupTouched(this.employeeSalaryForm);
      return;
    }

    const formValue = this.employeeSalaryForm.getRawValue();
    const selectedSalaryHead = this.salaryHeadList.find((head: any) => head.id === formValue.salaryHeadId);

    this.employeeSalaryList.push({
      ...formValue,
      salaryHeadName: selectedSalaryHead?.name ?? ''
    });

    this.dataSource.data = [...this.employeeSalaryList];
    this.resetForm();
  }

  private resetForm(): void {
    this.employeeSalaryForm.reset({
      id: 0,
      salaryHeadId: null,
      amount: 0,
      effectiveFrom: this.constantService.formatDate(new Date())
    });
    this.employeeSalaryForm.markAsPristine();
    this.employeeSalaryForm.markAsUntouched();
  }

  getSalaryHeadName(salaryHeadId: number): string {
    return this.salaryHeadList.find((head: any) => head.id === salaryHeadId)?.name ?? '';
  }

  SaveData(): void {
    if (!this.employeeId) {
      this.notificationsService.showNotification('Employee not found.', 'snack-bar-danger');
      return;
    }

    if (this.employeeSalaryList.length === 0) {
      this.notificationsService.showNotification('Add at least one salary entry.', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    const payload = {
      employeeId: this.employeeId,
      employeeSalary: this.employeeSalaryList.map(({ salaryHeadName, ...item }) => item)
    };

    this.employeeSalaryService.saveEmployeeSalary(payload).subscribe({
      next: (data: any) => {
        if (data?.Status === 200) {
          this.notificationsService.showNotification(data.Message, 'snack-bar-success');
        } else {
          this.notificationsService.showNotification(data?.Message ?? 'Unable to save salary', 'snack-bar-danger');
        }
        this.getEmployeeSalaryByEmployeeId();
        this.isLoading = false;
      },
      error: (error: any) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }
}
