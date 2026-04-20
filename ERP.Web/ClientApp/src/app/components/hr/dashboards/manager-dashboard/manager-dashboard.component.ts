import { Component, OnInit } from '@angular/core';
import { RosterService } from '../../roster/roster.service';

// Manager Dashboard Component
@Component({
    selector: 'app-manager-dashboard',
    templateUrl: './manager-dashboard.component.html',
    styleUrls: ['./manager-dashboard.component.css'],
    standalone: false
})
export class ManagerDashboardComponent implements OnInit {
    profile: any;
    loading = false;
    errorMessage = '';
    rosterData: any = null;
    rosterDays: number[] = [];
    currentMonth: number = 0;
    currentYear: number = 0;
    groupedRoster: any[] = [];
    rosterSummary = {
        morning: 0,
        evening: 0,
        night: 0,
        off: 0
    };

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

    constructor(private rosterService: RosterService) { }

    ngOnInit(): void {
        this.profile = JSON.parse(localStorage.getItem('profile') || 'null');
        this.setCurrentMonthYear();
        this.loadDepartmentRoster();
    }

    setCurrentMonthYear() {
        const today = new Date();
        this.currentMonth = today.getMonth() + 1; // 1-12
        this.currentYear = today.getFullYear();
    }

    getMonthLabel(): string {
        return this.months.find(m => m.value === this.currentMonth)?.label || '';
    }

    async loadDepartmentRoster() {
        // Backend automatically filters by manager's department from session
        const filterRequest = {
            year: this.currentYear,
            month: this.currentMonth,
            statusId: 3, // Approved status
            pagingData: {
                currentPage: 0,
                take: 10
            }
        };

        this.loading = true;
        try {
            (await this.rosterService.getAllRostersByManager(filterRequest)).subscribe({
                next: (data: any) => {
                    if (data?.item1?.length > 0) {
                        this.rosterData = data.item1[0];
                        this.buildRosterDays();
                        this.groupRosterData();
                        this.buildRosterSummary();
                    }
                    this.loading = false;
                },
                error: (error: any) => {
                    this.errorMessage = 'Failed to load roster data.';
                    console.error('Error loading roster:', error);
                    this.loading = false;
                }
            });
        } catch (error) {
            this.errorMessage = 'Failed to load roster data.';
            console.error('Error loading roster:', error);
            this.loading = false;
        }
    }

    buildRosterDays() {
        if (!this.rosterData?.year || !this.rosterData?.month) return;
        const totalDays = new Date(this.rosterData.year, this.rosterData.month, 0).getDate();
        this.rosterDays = Array.from({ length: totalDays }, (_, i) => i + 1);
    }

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

    getRosterCell(emp: any, day: number): any {
        if (!emp?.data) return null;
        return emp.data.find((x: any) =>
            new Date(x.rosterDate).getDate() === day
        );
    }

    getCellClass(emp: any, day: number): string {
        const cell = this.getRosterCell(emp, day);
        if (!cell) return '';
        if (cell.isOffDay) return 'off-day';
        if (cell.employeeShiftId === 1) return 'morning-shift';
        if (cell.employeeShiftId === 2) return 'evening-shift';
        if (cell.employeeShiftId === 3) return 'night-shift';
        return '';
    }

    getShiftName(emp: any, day: number): string {
        const cell = this.getRosterCell(emp, day);
        if (!cell) return '';
        if (cell.isOffDay) return 'OFF';
        return cell.employeeShift?.name || '';
    }

    getEmployeeShiftCount(emp: any, shiftId: number): number {
        const data = emp?.data || [];
        return data.filter((x: any) => !x?.isOffDay && x?.employeeShiftId === shiftId).length;
    }

    getEmployeeOffCount(emp: any): number {
        const data = emp?.data || [];
        return data.filter((x: any) => x?.isOffDay).length;
    }

    isWeekend(day: number): boolean {
        const date = new Date(this.currentYear, this.currentMonth - 1, day);
        const d = date.getDay();
        return d === 0 || d === 6;
    }

    getStatusClass(): string {
        const status = this.rosterData?.status?.title;
        if (status === 'Approved') return 'status-approved';
        if (status === 'Processed') return 'status-processed';
        if (status === 'Created') return 'status-created';
        return '';
    }
}
