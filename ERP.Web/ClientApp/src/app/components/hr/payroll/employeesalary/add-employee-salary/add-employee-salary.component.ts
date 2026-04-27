import { Component, Inject, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConstantService } from '../../../../../Service/constant.service';
import { NotificationsService } from '../../../../../Service/notification.service';
import { SalaryHeadService } from '../../salaryhead/salaryhead.service';
import { EmployeeSalaryService } from '../employeesalary.service';
import { SalaryTaxSlabService } from '../../salarytaxslab/salarytaxslab.service';

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
  employeeId: string | null = null;
  taxRate = 0;
  salaryTaxSlabList: any[] = [];
  today = new Date();

  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private dialog: MatDialog,
    private notificationsService: NotificationsService,
    private formBuilder: FormBuilder,
    private salaryHeadService: SalaryHeadService,
    private employeeSalaryService: EmployeeSalaryService,
    private salarytaxslabService: SalaryTaxSlabService,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.employeeId = this.data?.element?.id ?? null;
    this.initializeForm();

    this.getSalaryHeadList();
    this.getSalaryTaxSlabList();
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
      salaryHead: item?.salaryHead ?? null,
      amount: item?.amount ?? 0,
      effectiveFrom: item?.effectiveFrom ?? this.constantService.formatDate(new Date()),
      salaryHeadName: item?.salaryHeadName ?? item?.salaryHead?.name ?? item?.salaryHeadName,
      salaryHeadType: item?.salaryHead?.type ?? item?.salaryHeadType ?? 0,
      salaryHeadTypeName: item?.salaryHead?.typeName ?? item?.salaryHeadTypeName ?? '',
      isTaxable: item?.salaryHead?.isTaxable ?? item?.isTaxable ?? false
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
      salaryHeadName: selectedSalaryHead?.name ?? '',
      salaryHeadType: selectedSalaryHead?.type ?? 0,
      salaryHeadTypeName: selectedSalaryHead?.typeName ?? '',
      isTaxable: selectedSalaryHead?.isTaxable ?? false
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

  getSalaryTaxSlabList(): void {
    const filter = { PagingData: { currentPage: 0, take: 500 } };
    this.salarytaxslabService.getAllSalaryTaxSlab(filter).subscribe({
      next: (data: any) => {
        this.salaryTaxSlabList = Array.isArray(data?.item1) ? data.item1 : [];
      },
      error: (error: any) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
      }
    });
  }

  get earningRows(): any[] {
    return this.employeeSalaryList.filter((x: any) => x.salaryHeadType === 1 || x.salaryHeadTypeName === 'Earning');
  }

  get deductionRows(): any[] {
    return this.employeeSalaryList.filter((x: any) => x.salaryHeadType === 2 || x.salaryHeadTypeName === 'Deduction');
  }

  get basicSalary(): number {
    return this.employeeSalaryList
      .filter((x: any) => (x.salaryHeadName ?? '').toLowerCase().includes('basic'))
      .reduce((sum: number, x: any) => sum + Number(x.amount || 0), 0);
  }

  get totalEarnings(): number {
    return this.earningRows.reduce((sum: number, x: any) => sum + Number(x.amount || 0), 0);
  }

  get totalDeductions(): number {
    return this.deductionRows.reduce((sum: number, x: any) => sum + Number(x.amount || 0), 0);
  }

  get taxableAmount(): number {
    const taxableHeadsTotal = this.employeeSalaryList
      .filter((x: any) => !!x.isTaxable && (x.salaryHeadType === 1 || x.salaryHeadTypeName === 'Earning'))
      .reduce((sum: number, x: any) => sum + Number(x.amount || 0), 0);

    // If no salary head is flagged taxable, fallback to gross taxable income.
    if (taxableHeadsTotal <= 0) {
      const grossTaxable = this.totalEarnings - this.totalDeductions;
      return grossTaxable > 0 ? grossTaxable : 0;
    }

    return taxableHeadsTotal;
  }

  get taxAmount(): number {
    return (this.taxableAmount * Number(this.appliedTaxRate || 0)) / 100;
  }

  get netSalary(): number {
    return this.totalEarnings - this.totalDeductions - this.taxAmount;
  }

  get appliedTaxRate(): number {
    const taxable = this.taxableAmount;
    const slab = this.salaryTaxSlabList.find((x: any) => {
      const from = Number(x?.fromAmount ?? 0);
      const to = Number(x?.toAmount ?? 0);
      return taxable >= from && taxable <= to;
    });

    return Number(slab?.percentage ?? this.taxRate ?? 0);
  }

  get hasMatchingTaxSlab(): boolean {
    const taxable = this.taxableAmount;
    return this.salaryTaxSlabList.some((x: any) => {
      const from = Number(x?.fromAmount ?? 0);
      const to = Number(x?.toAmount ?? 0);
      return taxable >= from && taxable <= to;
    });
  }

  get employeeName(): string {
    const firstName = this.data?.element?.firstName ?? '';
    const lastName = this.data?.element?.lastName ?? '';
    return `${firstName} ${lastName}`.trim() || '-';
  }

  get employeeDepartment(): string {
    return this.data?.element?.departmentName ?? this.data?.element?.department.name ?? '-';
  }

  get employeeDesignation(): string {
    return this.data?.element?.employeeDesignation.name ?? '-';
  }

  printSalarySlip(): void {
    const printElement = document.getElementById('printsalaryslip');
    if (!printElement) {
      this.notificationsService.showNotification('Salary slip not found.', 'snack-bar-danger');
      return;
    }

    const popup = window.open('', '_blank', 'width=900,height=700');
    if (!popup) {
      this.notificationsService.showNotification('Please allow popups to print salary slip.', 'snack-bar-danger');
      return;
    }

    popup.document.open();
    popup.document.write(`
      <html>
        <head>
          <title>Salary Slip</title>
          <style>
            body { font-family: Arial, sans-serif; padding: 16px; }
            table { width: 100%; border-collapse: collapse; margin-bottom: 12px; }
            th, td { border: 1px solid #d4d4d4; padding: 6px 8px; font-size: 13px; }
            th { background: #f5f5f5; text-align: left; }
            .net-row td { font-weight: 700; }
          </style>
        </head>
        <body>${printElement.outerHTML}</body>
      </html>
    `);
    popup.document.close();
    popup.focus();
    popup.print();
    popup.close();
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
