import { Component, Inject, Optional } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RosterService } from '../roster.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { DepartmentService } from '../../../department/department.service';
import { EmployeeService } from '../../employee/employee.service';
import { EmployeeShiftService } from '../../employee-shift/employee-shift.service';

export interface Employee {
  id: string;          // Guid as string
  name: string;
  employeeShiftId: number;
}

export type ShiftCode = 'M' | 'E' | 'N' | 'O' | '';

export interface CellState {
  shift: ShiftCode;
}

export interface CellState {
  shift: ShiftCode;
}

@Component({
  selector: 'app-add-roster',
  templateUrl: './add-roster.component.html',
  styleUrl: './add-roster.component.css',
  standalone: false
})

export class AddRosterComponent {
  rosterForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  departmentList: any;
  employeeList: any;
  employeeShiftList: any;
  years: number[] = [];
  months = [
    { value: 1, label: 'January' },
    { value: 2, label: 'February' },
    { value: 3, label: 'March' },
    { value: 4, label: 'April' },
    { value: 5, label: 'May' },
    { value: 6, label: 'June' },
    { value: 7, label: 'July' },
    { value: 8, label: 'August' },
    { value: 9, label: 'September' },
    { value: 10, label: 'October' },
    { value: 11, label: 'November' },
    { value: 12, label: 'December' },
  ];
  dayNames = ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'];
  days: number[] = [];
  // Summary counts
  morningCount = 0;
  eveningCount = 0;
  nightCount = 0;
  offCount = 0;

  constructor(
    private notificationsService: NotificationsService,
    private formBuilder: FormBuilder,
    private rosterService: RosterService,
    private departmentService: DepartmentService,
    private constantService: ConstantService,
    private employeeService: EmployeeService,
    private employeeShiftService: EmployeeShiftService,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: { element: any } | null
  ) { }
  ngOnInit(): void {
    this.rosterForm = this.formBuilder.group({
      id: [0],
      year: [new Date().getFullYear()],
      month: [new Date().getMonth()],
      departmentId: [27],
      remarks: [''],
      costSheetDetail: this.formBuilder.array([]) // Initialize as a FormArray
    });
    this.getDepartmentList();
    this.buildYears();
    this.getEmployeeList();
    this.LoadData(this.data?.element);
  }

  buildYears(): void {
    const current = new Date().getFullYear();
    for (let y = current; y <= current + 1; y++) {
      this.years.push(y);
    }
  }

  buildDays(): void {
    const totalDays = new Date(this.rosterForm.get('year')?.value, this.rosterForm.get('month')?.value, 0).getDate();
    this.days = Array.from({ length: totalDays }, (_, i) => i + 1);
  }



  get costSheetDetail(): FormArray {
    return this.rosterForm.get('costSheetDetail') as FormArray;
  }

  addRosterDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0], // Default value
      rosterId: [0], // Default value
      employeeId: [0, Validators.required], // Validation
      shift : [''],
      employeeShiftId: [0, Validators.required], // Validation
      rosterDate: ['', Validators.required], // Validation
      isOffDay: [false, [Validators.required, Validators.min(0.001)]], // Validation
    });

    // Insert the new group after the current index
    this.costSheetDetail.insert(index + 1, newDetailGroup);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.rosterForm);

      // Populate the costSheetDetail FormArray
      const detailsArray = this.rosterForm.get('costSheetDetail') as FormArray;
      detailsArray.clear(); // Clear existing data

      if (element.costSheetDetail && element.costSheetDetail.length > 0) {
        element.costSheetDetail.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            costSheetId: [detail.costSheetId],
            itemId: [detail.itemId, Validators.required],
            item: [detail.item ? detail.item : '', Validators.required],
            itemName: [detail.item ? detail.item.code + ':' + detail.item.name : '', Validators.required],
            quantity: [detail.quantity, [Validators.required, Validators.min(0.001)]],
            rate: [detail.rate, [Validators.required, Validators.min(1)]],
            amount: [detail.quantity * detail.rate, [Validators.required, Validators.min(1)]]
          });
          detailsArray.push(detailGroup);
        });
      }
    }
    else {
      this.addRosterDetail(0);
      this.rosterForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
    }
  }

  SaveData() {
    if (this.rosterForm.invalid) {
      this.constantService.markFormGroupTouched(this.rosterForm);
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.rosterForm.value);

    this.rosterService.saveRoster(_clienttemperatureForm).subscribe({
      next: (data: { Status: number; Data: string; }) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error: string) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }

  getDepartmentList(): void {
    let _departmentsForm: any = {};
    this.departmentService.getAllDepartments(_departmentsForm).subscribe(data => {
      this.departmentList = data.item1;
    });
  }

  getEmployeeList() {
    var departmentId = this.rosterForm.get('departmentId')?.value;
    this.employeeService.getEmployeeByDepartment(departmentId)
      .subscribe((data: any) => {
        this.employeeList = data;
      });
  }

  getEmployeeShiftList(): void {
    let _filterForm = {};
    this.employeeShiftService.getAllEmployeeShifts(_filterForm).subscribe(data => {
      this.employeeShiftList = data.item1;
    });
  }

  onMonthYearChange(): void {
    // this.rosterData.clear();
     this.buildDays();
    // this.updateSummary();
  }

   cellClass(employeeId: string, day: number): string {
    const shift = this.getShift(employeeId, day);
    const weekend = this.isWeekend(day) ? 'weekend-cell' : '';
    const off = shift === 'O' ? 'is-off' : '';
    return `${weekend} ${off}`.trim();
  }

    getShift(employeeId: string, day: number): ShiftCode {
    return this.costSheetDetail.get(this.cellKey(employeeId, day))?.shift ?? '';
  }

  // ── Bulk actions ──────────────────────────────────────────────────────────

  // bulkApply(shift: ShiftCode): void {
  //   this.employees.forEach(emp => {
  //     this.days.forEach(day => {
  //       this.rosterData.set(this.cellKey(emp.id, day), { shift });
  //     });
  //   });
  //   this.updateSummary();
  // }

  clearAll(): void {
    this.updateSummary();
  }

  // ── Summary ───────────────────────────────────────────────────────────────

  updateSummary(): void {
    this.morningCount = 0;
    this.eveningCount = 0;
    this.nightCount = 0;
    this.offCount = 0;
    // this.rosterData.forEach(cell => {
    //   if      (cell.shift === 'M') this.morningCount++;
    //   else if (cell.shift === 'E') this.eveningCount++;
    //   else if (cell.shift === 'N') this.nightCount++;
    //   else if (cell.shift === 'O') this.offCount++;
    // });
  }


  getDayName(day: number): string {
    return this.dayNames[new Date(this.rosterForm.get('year')?.value, this.rosterForm.get('month')?.value - 1, day).getDay()];
  }

  isWeekend(day: number): boolean {
    const dow = new Date(this.rosterForm.get('year')?.value, this.rosterForm.get('month')?.value - 1, day).getDay();
    return dow === 0 || dow === 6;
  }

  // cellClass(employeeId: string, day: number): string {
  //   const shift = this.getShift(employeeId, day);
  //   const weekend = this.isWeekend(day) ? 'weekend-cell' : '';
  //   const off = shift === 'O' ? 'is-off' : '';
  //   return `${weekend} ${off}`.trim();
  // }

  //   getShift(employeeId: string, day: number): ShiftCode {
  //   return this.rosterForm.get(this.cellKey(employeeId, day))?.shift ?? '';
  // }

  cellKey(employeeId: string, day: number): string {
    return `${employeeId}_${day}`;
  }

  // ── TrackBy for performance ───────────────────────────────────────────────

  trackByEmp(_: number, emp: Employee): string { return emp.id; }
  trackByDay(_: number, day: number): number { return day; }

}
