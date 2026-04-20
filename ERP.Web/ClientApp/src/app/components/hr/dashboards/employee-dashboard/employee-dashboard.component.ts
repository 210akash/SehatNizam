import { Component, OnInit } from '@angular/core';
import { ConstantService } from '../../../../Service/constant.service';
import { UserAttendanceService } from '../../../order/user-attendance/user-attendance.service';
import { RosterService } from '../../roster/roster.service';
import { NotificationService } from '../../notification/notification.service';

// Employee Dashboard Component
@Component({
    selector: 'app-employee-dashboard',
    templateUrl: './employee-dashboard.component.html',
    styleUrls: ['./employee-dashboard.component.css'],
    standalone: false
})
export class EmployeeDashboardComponent implements OnInit {
    profile: any;
  loading = false;
  errorMessage = '';
  isLoading = false;
    attendanceData: any[] = [];
    notifications: any[] = [];
    rosterData: any = null;
    rosterDays: number[] = [];
    currentMonth: number = 0;
    currentYear: number = 0;
      days: number[] = [];
  groupedRoster: any[] = [];
  rosterSummary = {
    morning: 0,
    evening: 0,
    night: 0,
    off: 0
  };
    employeeData = {
        name: 'Ali Khan',
        department: 'IT',
        role: 'Software Engineer',
        leaveBalance: 12,
        attendance: [
            { date: '2025-09-01', status: 'Present' },
            { date: '2025-09-02', status: 'Late' },
            { date: '2025-09-03', status: 'Absent' }
        ],
        upcomingHolidays: ['2025-09-20', '2025-09-25']
    };

    constructor(
        private constantService: ConstantService,
        private attendanceService: UserAttendanceService,
        private rosterService: RosterService,
        private notificationService: NotificationService
    ) { }

    ngOnInit(): void {
        this.profile = JSON.parse(localStorage.getItem('profile') || 'null');
        this.setCurrentMonthYear();
        this.onSubmit();
        this.loadEmployeeRoster();
        this.loadNotifications();
    }

    setCurrentMonthYear() {
        const today = new Date();
        this.currentMonth = today.getMonth() + 1; // 1-12
        this.currentYear = today.getFullYear();
    }

    
    async loadEmployeeRoster() {
        // Backend automatically filters by logged-in employee from session
        const filterRequest = {
            year: this.currentYear,
            month: this.currentMonth,
            statusId: 3 // Approved status
        };

        try {
            (await this.rosterService.getAllRostersByEmployee(filterRequest)).subscribe({
                next: (data: any) => {
                    if (data != null) {
                        // Get the first roster (should be current month)
                        this.rosterData = data;
                        this.buildDays();
                    }
                },
                error: (error: any) => {
                    console.error('Error loading roster:', error);
                }
            });
        } catch (error) {
            console.error('Error loading roster:', error);
        }
    }


  // =========================
  // Build Month Days
  // =========================
  buildDays() {
    const year = this.rosterData?.year;
    const month = this.rosterData?.month;

    if (!year || !month) return;

    const totalDays = new Date(year, month, 0).getDate();
    this.days = Array.from({ length: totalDays }, (_, i) => i + 1);
      this.groupRosterData();
  }

  // =========================
  // Group by Employee
  // =========================
  groupRosterData() {

    const map = new Map<string, any>();
    const details = this.rosterData?.rosterDetail || [];

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
    this.buildRosterSummary();
  }

  buildRosterSummary() {
    const details = this.rosterData?.rosterDetail || [];
    const summary = {
      morning: 0,
      evening: 0,
      night: 0,
      off: 0
    };

    for (const item of details) {
      if (item?.isOffDay) {
        summary.off++;
        continue;
      }

      if (item?.employeeShiftId === 1) summary.morning++;
      else if (item?.employeeShiftId === 2) summary.evening++;
      else if (item?.employeeShiftId === 3) summary.night++;
    }

    this.rosterSummary = summary;
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
    const date = new Date(this.rosterData.year, this.rosterData.month - 1, day);
    const d = date.getDay();
    return d === 0 || d === 6;
  }

  getRosterMonthLabel(): string {
    if (!this.rosterData?.year || !this.rosterData?.month) return '';
    const date = new Date(this.rosterData.year, this.rosterData.month - 1, 1);
    return date.toLocaleString('en-US', { month: 'long', year: 'numeric' });
  }

    async onSubmit() {
  const today = new Date(); // Current date
    // Set fDate to the first day of the current month
    const firstOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
        const filterRequest = {
            // userId: '369A6AE7-AF74-4068-A07E-54F3BCDACE04', // ensure it's coming from logged-in user
            userId: this.profile.id, // ensure it's coming from logged-in user
            fDate: this.constantService.formatDate(firstOfMonth),
            tDate: this.constantService.formatDate(today)
        };

        try {
            this.loading = true;
            this.errorMessage = '';

            const response = await (await this.attendanceService
                .getUserAttendanceByUser(filterRequest))
                .toPromise(); // convert observable to promise

            this.attendanceData = response || [];
        } catch (error) {
            this.errorMessage = 'Failed to load attendance data.';
            console.error('Error fetching attendance:', error);
        } finally {
            this.loading = false;
        }
    }

    isLate(record: any): boolean {
        if (!record?.timeIn || !record?.employeeShift?.fromTime) {
            return false;
        }

        // Convert timeIn to Date
        const timeIn = new Date(record.timeIn);

        // Get the shift date part from timeIn
        const shiftDate = new Date(timeIn);
        const [shiftHour, shiftMinute] = record.employeeShift.fromTime.split(':').map(Number);

        // Set shift fromTime
        shiftDate.setHours(shiftHour, shiftMinute, 0, 0);

        // Add 15 minutes grace period
        const shiftWithGrace = new Date(shiftDate.getTime() + 15 * 60 * 1000);

        // Check if timeIn is greater than shiftWithGrace
        return timeIn > shiftWithGrace;
    }

    async loadNotifications() {
        try {
            (await this.notificationService.getEmployeeNotifications()).subscribe({
                next: (data: any) => {
                    this.notifications = data || [];
                },
                error: (error: any) => {
                    console.error('Error loading notifications:', error);
                }
            });
        } catch (error) {
            console.error('Error loading notifications:', error);
        }
    }
}
