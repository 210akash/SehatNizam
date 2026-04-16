import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-view-roster',
  templateUrl: './view-roster.component.html',
  styleUrl: './view-roster.component.css',
  standalone: false
})
export class ViewRosterComponent implements OnInit {

  isLoading = false;

  roster: any;

  days: number[] = [];
  groupedRoster: any[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { element: any },
    private dialogRef: MatDialogRef<ViewRosterComponent>
  ) {
    this.roster = data?.element;
  }

  ngOnInit(): void {
    if (!this.roster) return;

    this.buildDays();
    this.groupRosterData();
  }

  // =========================
  // CLOSE DIALOG
  // =========================
  close() {
    this.dialogRef.close();
  }

  // =========================
  // Build Month Days
  // =========================
  buildDays() {
    const year = this.roster?.year;
    const month = this.roster?.month;

    if (!year || !month) return;

    const totalDays = new Date(year, month, 0).getDate();
    this.days = Array.from({ length: totalDays }, (_, i) => i + 1);
  }

  // =========================
  // Group by Employee
  // =========================
  groupRosterData() {

    const map = new Map<string, any>();
    const details = this.roster?.rosterDetail || [];

    for (let item of details) {

      const empId = item.employeeId;

      if (!map.has(empId)) {
        map.set(empId, {
          employeeId: empId,
          employeeName: this.getEmployeeName(item),
          employeeCode: item.employee?.code,
          data: []
        });
      }

      map.get(empId).data.push(item);
    }

    this.groupedRoster = Array.from(map.values());
  }

  getEmployeeName(item: any): string {
    return ((item?.employee?.firstName || '') + ' ' + (item?.employee?.lastName || '')).trim();
  }

  getRosterCell(emp: any, day: number) {
    if (!emp?.data) return null;

    return emp.data.find((x: any) =>
      new Date(x.rosterDate).getDate() === day
    );
  }

  getCellClass(emp: any, day: number): string {

    const cell = this.getRosterCell(emp, day);

    if (!cell) return '';

    if (cell.isOffDay) return 'off-cell';

    if (cell.employeeShiftId === 1) return 'morning-cell';
    if (cell.employeeShiftId === 2) return 'evening-cell';
    if (cell.employeeShiftId === 3) return 'night-cell';

    return '';
  }

  shiftClass(id: number): string {
    if (id === 1) return 'm';
    if (id === 2) return 'e';
    if (id === 3) return 'n';
    return '';
  }

  isWeekend(day: number): boolean {
    const date = new Date(this.roster.year, this.roster.month - 1, day);
    const d = date.getDay();
    return d === 0 || d === 6;
  }

  // =========================
  // STATUS HELPERS (NEW)
  // =========================
  getStatusClass(): string {
    const status = this.roster?.status?.title;

    if (status === 'Approved') return 'status-approved';
    if (status === 'Processed') return 'status-processed';
    if (status === 'Created') return 'status-created';

    return '';
  }
}