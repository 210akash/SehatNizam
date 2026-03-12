import { Component, OnInit } from '@angular/core';
import { ConstantService } from '../../../../Service/constant.service';
import { UserAttendanceService } from '../../../order/user-attendance/user-attendance.service';

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

    constructor(private constantService: ConstantService, private attendanceService: UserAttendanceService) { }

    ngOnInit(): void {
        this.profile = JSON.parse(localStorage.getItem('profile') || 'null');
        this.onSubmit();
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
}