import { Component, Inject } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
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
    this.employeeSalaryForm = this.formBuilder.group({
      employeeId: [this.data?.element?.id ?? null, Validators.required],
      employeeSalary: this.formBuilder.array([])
    });

    this.getSalaryHeadList();
    this.addRow();
  }

  get employeeSalary(): FormArray {
    return this.employeeSalaryForm.get('employeeSalary') as FormArray;
  }

  private createRow(): FormGroup {
    return this.formBuilder.group({
      id: [0],
      salaryHeadId: [null, Validators.required],
      amount: [0, [Validators.required, Validators.min(0)]],
      effectiveFrom: [this.constantService.formatDate(new Date()), Validators.required]
    });
  }

  addRow(index?: number): void {
    const row = this.createRow();
    if (index === undefined || index < 0 || index >= this.employeeSalary.length) {
      this.employeeSalary.push(row);
      return;
    }
    this.employeeSalary.insert(index + 1, row);
  }

  removeRow(index: number): void {
    if (this.employeeSalary.length === 1) return;
    this.employeeSalary.removeAt(index);
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

  SaveData(): void {
    if (this.employeeSalaryForm.invalid) {
      this.constantService.markFormGroupTouched(this.employeeSalaryForm);
      return;
    }

    this.isLoading = true;
    const payload = Object.assign({}, this.employeeSalaryForm.value);

    this.employeeSalaryService.saveEmployeeSalary(payload).subscribe({
      next: (data: any) => {
        if (data?.Status === 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        } else {
          this.notificationsService.showNotification(data?.Data ?? 'Unable to save salary', 'snack-bar-danger');
        }
        this.isLoading = false;
      },
      error: (error: any) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }
}
