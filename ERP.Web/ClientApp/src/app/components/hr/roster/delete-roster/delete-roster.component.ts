import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { RosterService } from '../roster.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
    selector: 'app-delete-roster',
    templateUrl: './delete-roster.component.html',
    styleUrl: './delete-roster.component.css',
    standalone: false
})

export class DeleteRosterComponent {
  isLoading = false;
   roster: any;
   days: number[] = [];
   groupedRoster: any[] = [];
 
 constructor(
     @Inject(MAT_DIALOG_DATA) public data: { element: any },
     private notificationsService: NotificationsService, private rosterService: RosterService, private constantService: ConstantService,private dialogRef: MatDialogRef<DeleteRosterComponent>
   ) {
     this.roster = data?.element;
   }
 
   ngOnInit(): void {
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
           employeeCode: item.employee?.hrCode ?? item.employee?.code,
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
 
     const code = this.getShiftCode(cell);
     if (code === 'M') return 'morning-cell';
     if (code === 'E') return 'evening-cell';
     if (code === 'N') return 'night-cell';
 
     return '';
   }
 
   shiftClass(cell: any): string {
     const code = this.getShiftCode(cell);
     if (code === 'M') return 'm';
     if (code === 'E') return 'e';
     if (code === 'N') return 'n';
     return '';
   }
 
   private getShiftCode(cell: any): string {
     if (!cell) return '';
     if (cell.isOffDay) return '0';
     const code = (cell.employeeShift?.code ?? cell.employeeShift?.name ?? '').toString().trim().toUpperCase();
     if (code) return code;
     if (cell.employeeShiftId === 1) return 'M';
     if (cell.employeeShiftId === 2) return 'E';
     if (cell.employeeShiftId === 3) return 'N';
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
 
   shiftText(cell: any): string {
     if (!cell) return '';
     if (cell.isOffDay) return 'OFF';
     const code = this.getShiftCode(cell);
     const name = (cell.employeeShift?.name ?? '').toString().trim();
     if (code && name && code !== name.toUpperCase()) return `${code}`;
     return code || name;
   }
 
   getShiftLegend(): Array<{ code: string; name: string }> {
     const map = new Map<string, string>();
     const details = this.roster?.rosterDetail || [];
     for (const item of details) {
       const code = this.getShiftCode(item);
       if (!code || code === '0') continue;
       const name = (item?.employeeShift?.name ?? code).toString().trim() || code;
       if (!map.has(code)) map.set(code, name);
     }
     return Array.from(map.entries()).map(([code, name]) => ({ code, name }));
   }


  async delete() {
    (await this.rosterService.deleteRoster(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Delete Successfully', 'snack-bar-success');
          this.dialogRef.close(true);
        }
      },
      error: (error) => {
        console.log(error);
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }
}
