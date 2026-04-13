import { Component, Inject, Optional } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { DepartmentService } from '../../../department/department.service';
import { EmployeeService } from '../../employee/employee.service';
import { EmployeeShiftService } from '../../employee-shift/employee-shift.service';
import { Router } from '@angular/router';
import { RosterService } from '../../roster/roster.service';

export interface Employee {
  id: string;          // Guid as string
  name: string;
  employeeShiftId: number;
}

export type ShiftCode = 'M' | 'E' | 'N' | '0' | '';

@Component({
  selector: 'app-add-roster-department',
  templateUrl: './add-roster-department.component.html',
  styleUrls: ['./add-roster-department.component.css'],
  standalone: false
})

export class AddRosterDepartmentComponent {
  rosterForm!: FormGroup;
  // quick lookup of rosterDetail index per employee/day
  private detailIndexMap: Map<string, number> = new Map();
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
    private router : Router,
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
      // keep month 1-12 to match selectors and date helpers
      month: [new Date().getMonth() + 1],
      remarks: [''],
      rosterDetail: this.formBuilder.array([]) // Initialize as a FormArray
    });
    this.getDepartmentList();
    this.buildYears();
    this.getEmployeeShiftList();
      this.getEmployeeList();

    // render days header on first load
    this.buildDays();
    const navStateElement =
      this.router.getCurrentNavigation()?.extras?.state?.['element'];
    const element = this.data?.element ?? navStateElement ?? history.state?.element;
    this.LoadData(element);
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

  get rosterDetail(): FormArray {
    return this.rosterForm.get('rosterDetail') as FormArray;
  }

  addRosterDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0], // Default value
      rosterId: [0], // Default value
      employeeId: [0, Validators.required], // Validation
      employeeShiftId: [0, Validators.required], // FK to shift table
      rosterDate: ['', Validators.required], // Validation
      isOffDay: [false], // Validation
    });

    // Insert the new group after the current index (or push if array empty)
    const targetIndex = Math.min(index + 1, this.rosterDetail.length);
    this.rosterDetail.insert(targetIndex, newDetailGroup);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.rosterForm);
      this.buildDays(); // month/year may change when editing
      this.detailIndexMap.clear();

      // Populate the rosterDetail FormArray
      const detailsArray = this.rosterForm.get('rosterDetail') as FormArray;
      detailsArray.clear(); // Clear existing data

      if (element.rosterDetail && element.rosterDetail.length > 0) {
        element.rosterDetail.forEach((detail: any, idx: number) => {
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            rosterId: [detail.rosterId],
            employeeId: [detail.employeeId, Validators.required],
         employeeShiftId: [
    detail.isOffDay === true
      ? 0
      : (detail.employeeShiftId ?? detail.shiftId ?? detail.shift ?? 0),
    Validators.required
  ],
            rosterDate: [detail.rosterDate, Validators.required],
            isOffDay: [detail.isOffDay ?? false, []]
          });
          detailsArray.push(detailGroup);
          const day = new Date(detail.rosterDate).getDate();
          this.detailIndexMap.set(this.cellKey(detail.employeeId, day), idx);
        });
        this.updateSummary();
      }
      console.log(detailsArray);
    }
    else {
      this.detailIndexMap.clear();
      this.rosterDetail.clear();
      this.rosterForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
      this.buildDays();
    }
  }

  SaveData() {
    console.log(this.rosterForm.value);
    if (this.rosterForm.invalid) {
      this.constantService.markFormGroupTouched(this.rosterForm);
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.rosterForm.value);

    this.rosterService.saveRosterByManager(_clienttemperatureForm).subscribe({
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
    this.employeeService.getEmployeeByDepartment(0)
      .subscribe((data: any) => {
        this.employeeList = data;
      });
      this.onMonthYearChange();
  }

  getEmployeeShiftList(): void {
    let _filterForm = {};
    this.employeeShiftService.getAllEmployeeShifts(_filterForm).subscribe(data => {
      this.employeeShiftList = data.item1;
    });
  }

  onMonthYearChange(): void {
    this.detailIndexMap.clear();
     this.buildDays();
    this.rosterDetail.clear();
    this.updateSummary();
  }

   cellClass(employeeId: string, day: number): string {
    const shift = this.getShift(employeeId, day);
    const weekend = this.isWeekend(day) ? 'weekend-cell' : '';
    const off = shift === '0' ? 'is-off' : '';
    return `${weekend} ${off}`.trim();
  }

  getShift(employeeId: string, day: number): ShiftCode {
    const detail = this.findDetail(employeeId, day);
    if (!detail) return '';
    if (detail.get('isOffDay')?.value) return '0';
    const shiftId = detail.get('employeeShiftId')?.value;
    return this.shiftCodeFromId(shiftId);
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
    this.rosterDetail.clear();
    this.detailIndexMap.clear();
    this.updateSummary();
  }

  // ── Summary ───────────────────────────────────────────────────────────────

  updateSummary(): void {
    this.morningCount = 0;
    this.eveningCount = 0;
    this.nightCount = 0;
    this.offCount = 0;
    this.rosterDetail.controls.forEach(ctrl => {
      const fg = ctrl as FormGroup;
      if (fg.get('isOffDay')?.value) {
        this.offCount++;
        return;
      }
      const shift = this.shiftCodeFromId(fg.get('employeeShiftId')?.value);
      if      (shift === 'M') this.morningCount++;
      else if (shift === 'E') this.eveningCount++;
      else if (shift === 'N') this.nightCount++;
    });
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

  shiftClass(employeeId: string, day: number): string {
    const shift = this.getShift(employeeId, day);
    return shift ? `sel-${shift}` : '';
  }

  detailGroup(employeeId: string, day: number): FormGroup {
    return this.ensureDetail(employeeId, day);
  }

  shiftLabel(shift: any): string {
    const name = (shift?.name ?? shift?.shiftName ?? shift?.code ?? '').toString().trim();
    return name ? name.charAt(0).toUpperCase() : '?';
  }

  private shiftCodeFromId(id: any): ShiftCode {
    if (!id || !this.employeeShiftList) return '';
    if (id === '0') return '0';
    const found = this.employeeShiftList.find((s: any) => String(s.id) === String(id));
    return found ? (this.shiftLabel(found) as ShiftCode) : '';
  }

  private getEmployeeDefaultShiftId(employeeId: string): any {
    const emp = this.employeeList?.find((e: any) => String(e.id) === String(employeeId));
    return emp?.employeeShiftId ?? 0;
  }

// ── Cell interaction ──────────────────────────────────────────────────────
 
  onShiftChange(employeeId: string, day: number, value: ShiftCode): void {
    const ctrl = this.ensureDetail(employeeId, day);
    const isOff = value === '0';
    const targetShiftId = isOff ? this.getEmployeeDefaultShiftId(employeeId) : value;
    ctrl.patchValue({
      employeeId,
      rosterDate: this.formatRosterDate(day),
      employeeShiftId: value === '0' ? targetShiftId : value,
      isOffDay: isOff
    });
    this.updateSummary();
  }

  private ensureDetail(employeeId: string, day: number): FormGroup {
    const existing = this.findDetail(employeeId, day);
    if (existing) return existing;

    const ctrl = this.formBuilder.group({
      id: [0],
      rosterId: [0],
      employeeId: [employeeId, Validators.required],
      employeeShiftId: ['', Validators.required],
      rosterDate: [this.formatRosterDate(day), Validators.required],
      isOffDay: [false]
    });
    this.rosterDetail.push(ctrl);
    this.detailIndexMap.set(this.cellKey(employeeId, day), this.rosterDetail.length - 1);
    return ctrl;
  }

  private findDetail(employeeId: string, day: number): FormGroup | null {
    const key = this.cellKey(employeeId, day);
    const idx = this.detailIndexMap.get(key);
    if (idx !== undefined) {
      return this.rosterDetail.at(idx) as FormGroup;
    }
    const foundIdx = this.rosterDetail.controls.findIndex(ctrl =>
      (ctrl as FormGroup).get('employeeId')?.value === employeeId &&
      new Date((ctrl as FormGroup).get('rosterDate')?.value).getDate() === day
    );
    if (foundIdx !== -1) {
      this.detailIndexMap.set(key, foundIdx);
      return this.rosterDetail.at(foundIdx) as FormGroup;
    }
    return null;
  }

  private formatRosterDate(day: number): string {
    const year = this.rosterForm.get('year')?.value;
    const month = this.rosterForm.get('month')?.value;
    const date = new Date(year, month - 1, day);
    return this.constantService.formatDate ? this.constantService.formatDate(date) : date.toISOString();
  }

  bulkApply(employeeId: string, value: any): void {
    if (!value) return;
    this.days.forEach(day => this.onShiftChange(employeeId, day, value));
  }

  onCancel(): void {
    // When opened as a page, navigate back to the appointment list.
    const canGoBack = window.history.length > 1;
    if (canGoBack) {
      window.history.back();
    } else {
      this.router.navigate(['/roster']);
    }
  }


  // ── TrackBy for performance ───────────────────────────────────────────────

  trackByEmp(_: number, emp: Employee): string { return emp.id; }
  trackByDay(_: number, day: number): number { return day; }

}
