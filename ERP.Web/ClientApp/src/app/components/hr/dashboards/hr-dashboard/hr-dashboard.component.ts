import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../dashboard.service';
import { EmployeeLeaveService } from '../../employee-leave/employee-leave.service';
import { ConstantService } from '../../../../Service/constant.service';
import { ProcessManageEmployeeLeaveComponent } from '../../manage-employee-leave/process-manage-employee-leave/process-manage-employee-leave.component';
import { MatDialog } from '@angular/material/dialog';
import { AddEmployeeComponent } from '../../employee/add-employee/add-employee.component';
import { AddCommentsComponent } from '../../../interview/add-comments/add-comments.component';

@Component({
    selector: 'app-hr-dashboard',
    templateUrl: './hr-dashboard.component.html',
    styleUrls: ['./hr-dashboard.component.css'],
    standalone: false
})

export class HrDashboardComponent implements OnInit {
    days = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30', '31'];

    attendanceData = [
        { name: 'John Doe', days: [true, true, false, true, true, false, true, true, true, false, true, true, false, true, true, true, false, true, true, false, true, true, true, false, true, true, false, true, true, false, true] },
        { name: 'Jane Smith', days: [true, true, false, true, true, false, true, true, true, false, true, true, false, true, true, true, false, true, true, false, true, true, true, false, true, true, false, true, true, false, true] },
        { name: 'Alice Brown', days: [true, true, false, true, true, false, true, true, true, false, true, true, false, true, true, true, false, true, true, false, true, true, true, false, true, true, false, true, true, false, true] },
    ];

    // upcomingLeaves = [
    //     { name: 'John Doe', date: 'Sep 25', reason: 'Medical' },
    //     { name: 'Jane Smith', date: 'Sep 27', reason: 'Personal' },
    //     { name: 'Michael Chan', date: 'Sep 29', reason: 'Vacation' },
    // ];
    upcomingLeaves: any[] = [];

    hrData: any;
    hrCardData: any;
    todayAttendance: any;
    isLoadingHRData: boolean = true;
    isLoadingLeaves: boolean = true;
    isLoadingAttendance: boolean = true;

    todayInterviews: any;
    isLoadingInterviews: boolean = true;

    constructor(
        private dashboardService: DashboardService,
        private employeeLeaveService: EmployeeLeaveService,
        private constantService: ConstantService,
        private dialog: MatDialog
    ) { }

    ngOnInit(): void {
        this.getHRDashboardData();
    }

    async getHRDashboardData(): Promise<void> {
        this.isLoadingHRData = true;

        this.dashboardService.getHRDashboardData().subscribe({
            next: (data: any) => {
                this.hrData = data;
                this.hrCardData = [
                    { department: 'Employee (Sales)', count: this.hrData?.saleEmployees },
                    { department: 'Field Workers (Sales)', count: this.hrData?.saleFieldWorkers },
                    ...(this.hrData?.getDepartmentWiseCount || [])
                ];

                this.getAllEmployeeLeaves();
            },
            error: (error: any) => {
                console.error('Error fetching HR data:', error);
                this.isLoadingHRData = false;
            },
            complete: () => {
                this.isLoadingHRData = false;
            }
        });
    }

    async getAllEmployeeLeaves(): Promise<void> {
        this.isLoadingLeaves = true;

        let _payload = {
            fdate: this.constantService.formatDate(new Date().setDate(new Date().getDate() - 365)),
            tdate: this.constantService.formatDate(new Date()),
            statusId: 3
        };

        this.employeeLeaveService.getAllEmployeeLeaves(_payload).subscribe({
            next: (data: any) => {
                this.upcomingLeaves = data.item1;
                this.getTodayAttendance(); // call attendance after leaves
            },
            error: (error: any) => {
                console.error('Error fetching leaves:', error);
                this.isLoadingLeaves = false;
            },
            complete: () => {
                this.isLoadingLeaves = false;
            }
        });
    }

    processLeaveDialog(element: any) {
        const dialogRef = this.dialog.open(ProcessManageEmployeeLeaveComponent, {
            panelClass: 'cstm_width_700',
            maxHeight: '90vh',
            data: {
                element: element,
            },
            disableClose: true
        });

        dialogRef.afterClosed().subscribe(result => {
            this.getAllEmployeeLeaves();
        });
    }

    openEmployeeDialog() {
        const dialogRef = this.dialog.open(AddEmployeeComponent, {
            width: '60%',
            height: 'auto',
            maxHeight: '95vh',
            // data: {
            //     element: element,
            // },
            disableClose: true
        });

        dialogRef.afterClosed().subscribe(result => {
            this.getHRDashboardData();
        });
    }

    async getTodayAttendance(): Promise<void> {
        this.isLoadingAttendance = true;

        this.dashboardService.getTodayAttendance().subscribe({
            next: (data: any) => {
                this.todayAttendance = data;
                this.getTodayInterviews();
            },
            error: (error: any) => {
                console.error('Error fetching attendance:', error);
            },
            complete: () => {
                this.isLoadingAttendance = false;
            }
        });
    }

    async getTodayInterviews(): Promise<void> {
        this.isLoadingInterviews = true;

        this.dashboardService.getTodayInterviews().subscribe({
            next: (data: any) => {
                this.todayInterviews = data;
                console.log(data);
            },
            error: (error: any) => {
                console.error('Error fetching attendance:', error);
            },
            complete: () => {
                this.isLoadingInterviews = false;
            }
        });
    }

    processInterviewDialog(element: any) {
        const dialogRef = this.dialog.open(AddCommentsComponent, {
            panelClass: 'cstm_width_1300',
            height: 'auto',
            maxHeight: '90vh',
            data: {
                element: element,
            },
            disableClose: true
        });

        dialogRef.afterClosed().subscribe(result => {
            this.getTodayInterviews();
        });
    }

    getInterviewDate(interviewHistory: any): string {
        if (Array.isArray(interviewHistory)) {
            const todayHistory = interviewHistory.find(
                (history: any) =>
                    new Date(history.createdDate).toDateString() === new Date().toDateString()
            );

            if (todayHistory && todayHistory.interviewDate) {
                return new Date(todayHistory.interviewDate).toLocaleDateString(); // or custom format
            }
        }

        return '';
    }

    getAttendeesString(interview: any) {
        debugger
        let history = interview.interviewHistory
            .filter((history: any) => new Date(history.createdDate).toDateString() === new Date().toDateString())[0];

        if (history && Array.isArray(history.interviewAttendees)) {
            return history.interviewAttendees
                .map((attendee: any) => {
                    const user = attendee.aspNetUsers;
                    return user ? `${user.firstName} ${user.lastName}` : null;
                })
                .filter((name: string | null) => !!name)
                .join(', ');
        }

        return '';
    }


}