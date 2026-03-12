import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserAttendanceService } from '../user-attendance.service';
import { ConstantService } from '../../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DeviceAttendanceService } from '../../../hr/device-attendance/device-attendance.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { UpdateUserAttendanceComponent } from '../update-user-attendance/update-user-attendance.component';

@Component({
  selector: 'app-show-user-attendance',
  templateUrl: './show-user-attendance.component.html',
  styleUrls: ['./show-user-attendance.component.css'], standalone: false
})
export class ShowUserAttendanceComponent implements OnInit {
  attendanceForm!: FormGroup;
  attendanceData: any[] = [];
  loading = false;
  errorMessage = '';
  isLoading = false;
  constructor(
    private fb: FormBuilder,
     private dialog: MatDialog,
    private attendanceService: UserAttendanceService,
    private deviceAttendanceService: DeviceAttendanceService,
    private notificationsService: NotificationsService,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    const profile = JSON.parse(localStorage.getItem('profile') || '{}');
    const today = new Date(); // Current date
    const firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1); // 1st day of current month
    this.data = this.data?.element?.id === undefined
      ? { element: profile }
      : this.data;

    this.attendanceForm = this.fb.group({
      fDate: [this.constantService.formatDate(firstDayOfMonth), Validators.required], // Default: 1st day of current month
      tDate: [this.constantService.formatDate(today), Validators.required]            // Default: today's date
    });
    this.filterData();
  }

filterData() {
  const fDateControl = this.attendanceForm.get('fDate');
  const fDate = fDateControl?.value;
  const joinDate = this.data.element.joinDate
    ? new Date(this.data.element.joinDate)
    : null;

  if (fDate && joinDate) {
    const selectedDate = new Date(fDate);

    // If fDate is before joinDate → reset to joinDate
    if (selectedDate < joinDate) {
      fDateControl?.setValue(joinDate);
    }
  }

  this.onSubmit();
  this.isLoading = false;
}

  async onSubmit() {
    if (this.attendanceForm.invalid) {
      this.errorMessage = 'Please select both From Date and To Date.';
      return;
    }

    const formValues = this.attendanceForm.value;

    // Retrieve current user ID from localStorage or AuthenticationService

    const filterRequest = {
      // userId: '369A6AE7-AF74-4068-A07E-54F3BCDACE04', // ensure it's coming from logged-in user
      userId: this.data.element.id, // ensure it's coming from logged-in user
      fDate: formValues.fDate,
      tDate: formValues.tDate
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

async syncAttendanceByEmployee() {
  const formValues = this.attendanceForm.value;
  this.isLoading = true; // Set loading state to true when the request starts

    // Check if dates are valid
  if (!this.isValidDate( formValues.fDate) || !this.isValidDate(formValues.tDate)) {
    this.isLoading = false;
    this.notificationsService.showNotification('Invalid date(s). Please check the From and To dates.', 'snack-bar-error');
    return;
  }

  // Check if FromDate is before ToDate
  if (new Date( formValues.fDate) > new Date(formValues.tDate)) {
    this.isLoading = false;
    this.notificationsService.showNotification('From Date cannot be later than To Date. Please check the dates.', 'snack-bar-error');
    return;
  }

  try {
    // Make the API call and await the result
    const data = await this.deviceAttendanceService.syncAttendanceByEmployee(
      this.data.element.id,this.constantService.formatDate( formValues.fDate), 
      this.constantService.formatDate( formValues.tDate)
    ).toPromise();

    // Check if data is not empty
    if (data && data.length > 0) {
      const responseItem = data[0]; // Get the first (and possibly only) item in the response array
      const item2 = responseItem.item2;  // Access 'item2' (the records information string)
      this.onSubmit();
      // Show a success notification with 'item2' content
      this.notificationsService.showNotification(`Attendance data synced successfully! ${item2}`, 'snack-bar-success');
    } else {
      // If no data is returned, show a generic success message
      this.notificationsService.showNotification('No attendance data found to sync.', 'snack-bar-success');
    }
  } catch (error) {
    // If any error occurs, log it and show an error notification
    console.error('Error fetching attendance data:', error);

    // Optionally, show an error notification
    this.notificationsService.showNotification('Failed to sync attendance data. Please try again.', 'snack-bar-error');
  } finally {
    // Stop loading, no matter what happens
    this.isLoading = false;
  }
}

  updateUserAttendanceDialog(element: any) {
    element.userId = this.data.element.id;
    const dialogRef = this.dialog.open(UpdateUserAttendanceComponent, {
      panelClass: 'cstm_width_600',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      // this.bindData(this.IndentrequestFilterForm, this.currenttab, false);
      // this.getIndentrequestCount.emit();
    });
  }

// Utility function to check if a date is valid
isValidDate(date: any): boolean {
  const parsedDate = new Date(date);
  return !isNaN(parsedDate.getTime());
}

  printDocument() {
    const printContent = document.getElementById("printDoc");
    const cssStyles = `
      <style>
        body {
          margin: 0;
          font-size: 1rem;
          font-weight: 400;
          line-height: 1.5;
          color: #212529;
          text-align: left;
          background-color: #fff;
          font-family: sans-serif;
        }

        .text-center {
          text-align: center !important;
        }

        .text-left {
          text-align: left !important;
        }

        .text-right {
          text-align: right !important;
        }

        .m-0 {
          margin: 0 !important;
        }

        .mb-3 {
          margin-bottom: 1rem !important;
        }

        .mb-4 {
          margin-bottom: 1.5rem !important;
        }

        .mt-4,
        .my-4 {
          margin-top: 1.5rem !important;
        }

        *,
        ::after,
        ::before {
          box-sizing: border-box;
        }

        .row {
          display: -ms-flexbox;
          display: flex;
          -ms-flex-wrap: wrap;
          flex-wrap: wrap;
          margin-right: -15px;
          margin-left: -15px;
        }

        
        hr {
          box-sizing: content-box;
          height: 0;
          overflow: visible;
          border: 0;
          border-top: 1px solid rgba(0, 0, 0);
        }

        .mt-5,
        .my-5 {
          margin-top: 2.5rem !important;
        }

        .container {
          width: 100%;
          padding-right: 20px;
          padding-left: 20px;
          margin-right: auto;
          margin-left: auto;
        }

        .container,
        .container-lg,
        .container-md,
        .container-sm,
        .container-xl {
          max-width: 1140px;
        }

        p,
        label,
        input,
        h1,
        h2,
        h3,
        h4,
        h5,
        h6,
        li {
          font-family: sans-serif;
        }

        h1,
        h2,
        h3,
        h4,
        h5,
        h6 {
          margin-top: 0;
          margin-bottom: 0.5rem;
        }

        .h1,
        .h2,
        .h3,
        .h4,
        .h5,
        .h6,
        h1,
        h2,
        h3,
        h4,
        h5,
        h6 {
          margin-bottom: 0.5rem;
          font-weight: 500;
          line-height: 1.2;
        }

        th,
        td {
          border: 1px solid;
          padding: 5px;
        }

        .bl_table td {
          text-align: center;
        }

        p {
          margin-bottom: 5px;
          margin-top: 5px;
        }

        input {
          border: none;
        }

        table {
          width: 100%;
          border-collapse: collapse;
          border: 1px solid;
        }

        p,
        p,
        input,
        h1,
        h2,
        h3,
        h4,
        h5,
        h6,
        li {
          font-family: sans-serif;
        }

        p,
        p,
        input,
        li {
          font-size: 14px;
        }

        .underline {
          border-bottom: 2px solid #000;
        }

        h5 {
          font-size: 1.2rem;
          font-weight: 600;
        }

        .col,
        .col-1,
        .col-10,
        .col-11,
        .col-12,
        .col-2,
        .col-3,
        .col-4,
        .col-5,
        .col-6,
        .col-7,
        .col-8,
        .col-9,
        .col-auto,
        .col-lg,
        .col-lg-1,
        .col-lg-10,
        .col-lg-11,
        .col-lg-12,
        .col-lg-2,
        .col-lg-3,
        .col-lg-4,
        .col-lg-5,
        .col-lg-6,
        .col-lg-7,
        .col-lg-8,
        .col-lg-9,
        .col-lg-auto,
        .col-md,
        .col-md-1,
        .col-md-10,
        .col-md-11,
        .col-md-12,
        .col-md-2,
        .col-md-3,
        .col-md-4,
        .col-md-5,
        .col-md-6,
        .col-md-7,
        .col-md-8,
        .col-md-9,
        .col-md-auto,
        .col-sm,
        .col-sm-1,
        .col-sm-10,
        .col-sm-11,
        .col-sm-12,
        .col-sm-2,
        .col-sm-3,
        .col-sm-4,
        .col-sm-5,
        .col-sm-6,
        .col-sm-7,
        .col-sm-8,
        .col-sm-9,
        .col-sm-auto,
        .col-xl,
        .col-xl-1,
        .col-xl-10,
        .col-xl-11,
        .col-xl-12,
        .col-xl-2,
        .col-xl-3,
        .col-xl-4,
        .col-xl-5,
        .col-xl-6,
        .col-xl-7,
        .col-xl-8,
        .col-xl-9,
        .col-xl-auto {
          position: relative;
          width: 100%;
          padding-right: 15px;
          padding-left: 15px;
          padding: 0;
        }

        .pt-4,
        .py-4 {
          padding-top: 1.5rem !important;
        }

        .mt-2,
        .my-2 {
          margin-top: 0.5rem !important;
        }

        .mt-3 {
          margin-top: 1rem !important;
        }

        .h1,
        h1 {
          font-size: 2.5rem;
        }

        .h3,
        h3 {
          font-size: 1.25rem;
        }

        .h2,
        h2 {
          font-size: 2rem;
        }

        .mb-0,
        .my-0 {
          margin-bottom: 0 !important;
        }

        .mb-5,
        .my-5 {
          margin-bottom: 3rem !important;
        }

        .possession_counter td:nth-child(even) {
          text-align: center;
          padding: 0;
        }

        .justify-content-start {
          justify-content: flex-start;
        }

        .justify-content-center {
          justify-content: center;
        }

        .justify-content-end {
          justify-content: flex-end;
        }

        .p-0 {
          padding: 0 !important;
        }

        @media (min-width: 768px) {
          .col-md-1 {
            -ms-flex: 0 0 8.333333%;
            flex: 0 0 8.333333%;
            max-width: 8.333333%;
          }

          .col-md-2 {
            -ms-flex: 0 0 16.666667%;
            flex: 0 0 16.666667%;
            max-width: 16.666667%;
          }

          .col-md-3 {
            -ms-flex: 0 0 25%;
            flex: 0 0 25%;
            max-width: 25%;
          }

          .col-md-4 {
            -ms-flex: 0 0 33.333333%;
            flex: 0 0 33.333333%;
            max-width: 33.333333%;
          }

          .col-md-6 {
            -ms-flex: 0 0 50%;
            flex: 0 0 50%;
            max-width: 50%;
          }

          .col-md-8 {
            -ms-flex: 0 0 66.666667%;
            flex: 0 0 66.666667%;
            max-width: 66.666667%;
          }

          .col-md-10 {
            -ms-flex: 0 0 83.333333%;
            flex: 0 0 83.333333%;
            max-width: 83.333333%;
          }

          .col-md-11 {
            -ms-flex: 0 0 91.666667%;
            flex: 0 0 91.666667%;
            max-width: 91.666667%;
          }

          .col-md-12 {
            -ms-flex: 0 0 100%;
            flex: 0 0 100%;
            max-width: 100%;
          }
        }

        @media print {
.heading_hide{
  width:100%;
  display:inline-block;
}

          *,
          ::after,
          ::before {
            text-shadow: none !important;
            box-shadow: none !important
          }

          a:not(.btn) {
            text-decoration: underline
          }

          abbr[title]::after {
            content: " (" attr(title) ")"
          }

          pre {
            white-space: pre-wrap !important
          }

          blockquote,
          pre {
            border: 1px solid #adb5bd;
            page-break-inside: avoid
          }

          thead {
            display: table-header-group
          }

          img,
          tr {
            page-break-inside: avoid
          }

          h2,
          h3,
          p {
            orphans: 3;
            widows: 3
          }

          h2,
          h3 {
            page-break-after: avoid
          }

          body {
            min-width: 992px !important
          }

          .container {
            min-width: 992px !important
          }
            .print_hide{
              display: none !important;
            }
        }

        @media print {
          .row {
            display: -ms-flexbox;
            display: flex;
            -ms-flex-wrap: wrap;
            flex-wrap: wrap;
            margin-right: -15px;
            margin-left: -15px;
          }

          .col-md-1 {
            -ms-flex: 0 0 8.333333%;
            flex: 0 0 8.333333%;
            max-width: 8.333333%;
          }

          .col-md-2 {
            -ms-flex: 0 0 16.666667%;
            flex: 0 0 16.666667%;
            max-width: 16.666667%;
          }

          .col-md-3 {
            -ms-flex: 0 0 25%;
            flex: 0 0 25%;
            max-width: 25%;
          }

          .col-md-4 {
            -ms-flex: 0 0 33.333333% !important;
            flex: 0 0 33.333333% !important;
            max-width: 33.333333% !important;
          }

          .col-md-6 {
            -ms-flex: 0 0 50%;
            flex: 0 0 50%;
            max-width: 50%;
          }

          .col-md-8 {
            -ms-flex: 0 0 66.666667%;
            flex: 0 0 66.666667%;
            max-width: 66.666667%;
          }

          .col-md-11 {
            -ms-flex: 0 0 91.666667%;
            flex: 0 0 91.666667%;
            max-width: 91.666667%;
          }

          .col-md-12 {
            -ms-flex: 0 0 100%;
            flex: 0 0 100%;
            max-width: 100%;
          }

          .container {
            max-width: 1200px;
          }

          body {
            -webkit-print-color-adjust: exact;
            page-break-after: always;
          }

          .blue_header {
            background-color: #1f5ca8 !important;
            print-color-adjust: exact;
          }

          .lightblue_header {
            background-color: #D9E2F3 !important;
            print-color-adjust: exact;
          }
        }

        @media print {
          .pg_brk {
            page-break-after: always;
          }

          .container {
            margin: 0 !important;
          }
        }


        .fw-bold {
          font-weight: 600;
        }

        th {
          font-weight: 600;
          font-family: sans-serif;
        }

        .no_border {
          border: 0;
        }

        .logo {
          width: 100%;
          text-align: center;
          display: inline-flex;
          justify-content: flex-start;
          align-items: center;
          font-weight: 600;
          font-size: 1rem;
          gap: 5px;
        }

        .left_side {
          display: inline-flex;
          align-items: end;
          flex-direction: column;
        }

        .top_left {
          border: 0;
          font-size: 14px;
        }

        .top_left th,
        .top_left td {
          border: 0;
          padding-top: 5px;
          padding-bottom: 0;
        }

        .top_left th {
          padding-left: 0;
        }

        .top_right {
          font-size: 14px;
        }

        .table_one {
          text-align: center;
          font-size: 14px;
        }

        .table_one th {
          font-size: 14px;
          height: 40px;
        }

        .table_one td {
          font-size: 14px;
          height: 40px;
        }

        .sign_wrap {
          width: 100%;
          display: inline-flex;
        }

        .signed_by {
          width: 100%;
          max-width: 250px;
          display: inline-flex;
          justify-content: center;
          align-items: center;
          flex-direction: column;
          font-size: 14px;
        }

        .heading {
          border-top: 2px solid;
          width: 100%;
          text-align: center;
        }

        .remarks {
          width: 100%;
          display: inline-block;
          border: 1px solid;
          padding: 5px;
          min-height: 70px;
        }

        .remarks span {
          font-size: 14px;
        }

        .sign td {
          height: 70px;
        }

        /* .dates td:nth-child(1),
                .dates td:nth-child(3){
                    border-right: 0;
                } */
        .dates td:nth-child(2),
        .dates td:nth-child(4) {
          border-left: 0;
        }

        .sign td:nth-child(3) {
          border-right: 0;
        }

        .print_date {
          width: 100%;
          display: inline-flex;
          gap: 20px;
          font-size: 12px;
          justify-content: flex-end;
        }

        .cv-card {
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(0,0,0,0.1);
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
  background: #fff;
}

.cv-header {
  background: linear-gradient(135deg, #2c3e50 0%, #3498db 100%);
  padding: 32px;
  color: #fff;
  border-top-left-radius: 12px;
  border-top-right-radius: 12px;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.employee-info {
  flex: 1;
}

.padding
{
padding : 0 15px;
}
.employee-name {
  font-size: 28px;
  font-weight: 700;
  margin: 0;
  line-height: 1.2;
}

.employee-title {
  font-size: 18px;
  font-weight: 400;
  margin: 8px 0;
  opacity: 0.9;
}

.employee-code {
  font-size: 14px;
  font-weight: 300;
  margin: 0;
  opacity: 0.8;
}

.status-badge {
  display: inline-block;
  padding: 6px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
  margin-top: 8px;
}

.status-badge.active {
  background: #27ae60;
  color: #fff;
}

.status-badge.inactive {
  background: #c0392b;
  color: #fff;
}

.employee-photo {
  width: 120px;
  height: 120px;
  border-radius: 50%;
  overflow: hidden;
  border: 3px solid #fff;
  background: #f5f5f5;
  display: flex;
  align-items: center;
  justify-content: center;
}

.photo-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.photo-placeholder {
  text-align: center;
  color: #666;
  font-size: 12px;
}

.photo-placeholder i {
  color: #999;
}

.cv-content {
  padding: 32px;
}

.section {
  margin-bottom: 32px;
}

.section-title {
  font-size: 20px;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 16px;
  position: relative;
}

.section-title::after {
  content: '';
  position: absolute;
  bottom: -4px;
  left: 0;
  width: 40px;
  height: 3px;
  background: #3498db;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 16px;
}

.info-item {
  display: flex;
  flex-direction: column;
}

.info-label {
  font-size: 14px;
  font-weight: 600;
  color: #000000;
  margin-bottom: 4px;
}

.info-value {
  font-size: 16px;
  color: #2c3e50;
  background: #ffffff;
  padding: 8px 12px;
  border-radius: 6px;
  border: 1px solid #7e7e7e;
}

.documents-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
}

.document-card {
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
}

.document-card mat-card-header {
  padding: 12px 16px;
}

.document-card mat-card-title {
  font-size: 16px;
  font-weight: 500;
}

.document-img {
  width: 80px;
  height: 80px;
  object-fit: cover;
  border-radius: 4px;
  margin-bottom: 8px;
}

.document-link {
  color: #3498db;
  text-decoration: none;
  font-weight: 500;
}

.document-link:hover {
  text-decoration: underline;
}

.no-data {
  color: #7f8c8d;
  font-style: italic;
}

.button-container {
  display: flex;
  justify-content: flex-end;
  margin-top: 24px;
}

button[mat-flat-button] {
  background: #3498db;
  color: #fff;
  padding: 10px 24px;
  border-radius: 6px;
  font-weight: 500;
}

button[mat-flat-button]:hover {
  background: #2980b9;
}
      </style>`;

    if (printContent) {
      const WindowPrt = window.open('', '', 'left=0,top=0,width=1100,height=1100,toolbar=0,scrollbars=0,status=0');
      var fdate = this.constantService.formatDate(this.attendanceForm.get('fDate')?.value);
      var tdate = this.constantService.formatDate(this.attendanceForm.get('tDate')?.value);
      var title = this.data.element.firstName + " " + this.data.element.lastName + " (" + fdate + " To " + tdate + ")";
      if (WindowPrt) {
        const fullHtml = `
      <html>
        <head>
          <title> ${title}  </title>
          ${cssStyles}
        </head>
        <body>
          ${printContent.innerHTML}
        </body>
      </html>
    `;

        WindowPrt.document.write(fullHtml);
        WindowPrt.document.close();

        setTimeout(() => {
          WindowPrt.focus();
          WindowPrt.print();
          WindowPrt.close();
        }, 100);
      }
    }

  }
}
