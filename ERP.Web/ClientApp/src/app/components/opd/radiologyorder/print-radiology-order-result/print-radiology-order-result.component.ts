import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AuthenticationService } from '../../../../Auth/authentication.service';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-print-radiology-order-result',
  templateUrl: './print-radiology-order-result.component.html',
  styleUrls: ['./print-radiology-order-result.component.css'],
  standalone: false
})
export class PrintRadiologyOrderResultComponent implements OnInit {
  currentUser: any;
  currentDate: any;
  currentTime: any;
  radiologyResults: any[] = [];
  radiologyOrderTitle: string = 'Radiology Order';
  orderStatus: string = 'Pending';
  clinicalNotesText: string = '-';

  private readonly printStyles = `
    <style>
      body {
        margin: 0;
        font-family: Arial, Helvetica, sans-serif;
        color: #232323;
        background: #ffffff;
        -webkit-print-color-adjust: exact;
        print-color-adjust: exact;
      }

      *,
      ::before,
      ::after {
        box-sizing: border-box;
      }

      .report-sheet,
      .card,
      .print-dialog {
        width: 100%;
        max-width: 210mm;
        margin: 0 auto;
        padding: 18mm;
        background: #ffffff;
        color: #232323;
      }

      .report-header,
      .card-body {
        display: flex;
        justify-content: space-between;
        flex-wrap: wrap;
        gap: 18px;
        align-items: flex-start;
      }

      .report-title-group,
      .slip-top {
        display: flex;
        align-items: center;
        gap: 14px;
      }

      .hospital-copy {
        text-align: center;
      }

      .slip-sheet {
        width: 100%;
        min-height: auto;
        margin: 0;
        padding: 12mm;
        background: #fff;
        color: #404040;
      }

      .summary-card {
        display: flex;
        justify-content: space-between;
        gap: 18px;
        padding: 10px 12px;
        border: 1px solid #d4d4d4;
        border-radius: 8px;
      }

      .patient-summary-grid {
        flex: 1;
        display: grid;
        grid-template-columns: repeat(3, minmax(0, 1fr));
        gap: 12px 26px;
      }

      .summary-pair {
        display: flex;
        flex-direction: column;
        gap: 3px;
      }

      .summary-head {
        font-size: 0.62rem;
        font-weight: 700;
        text-transform: uppercase;
        color: #7b7b7b;
      }

      .summary-side {
        display: flex;
        align-items: center;
        gap: 10px;
      }

      .token-circle {
        width: 58px;
        height: 58px;
        border: 2px solid #202020;
        border-radius: 50%;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
      }

      .token-radiologyel {
        font-size: 0.44rem;
        font-weight: 700;
        text-transform: uppercase;
        line-height: 1;
      }

      .token-value {
        font-size: 0.5rem;
        font-weight: 500;
        line-height: 1;
        margin-top: 2px;
      }

      .section-title {
        margin: 18px 0 8px;
        font-size: 0.96rem;
        font-weight: 700;
      }

      .top-rule,
      .section-rule,
      .footer-rule {
        border-top: 2px solid #202020;
      }

      .top-rule {
        margin: 12px 0 18px;
      }

      .section-rule {
        margin-bottom: 16px;
        border-top-width: 1px;
        border-top-color: #d9d9d9;
      }

      .slip-footer {
        padding-top: 6px;
        text-align: center;
      }

      .footer-warning {
        font-size: 0.58rem;
        font-weight: 700;
        color: #333;
      }

      .footer-meta {
        margin-top: 6px;
        font-size: 0.54rem;
        color: #666;
      }

      .report-footer,
      .slip-footer {
        display: flex;
        justify-content: space-between;
        gap: 12px;
        align-items: center;
        margin-top: 12px;
        font-size: 0.82rem;
        color: #6b7280;
      }

      .hospital-mark {
        position: relative;
        width: 46px;
        height: 46px;
        border: 2px solid #1f2937;
        border-radius: 12px;
        flex-shrink: 0;
      }

      .hospital-mark::before,
      .hospital-mark::after,
      .mark-cross::before,
      .mark-cross::after {
        content: "";
        position: absolute;
        background: #1f2937;
      }

      .hospital-mark::before {
        width: 22px;
        height: 3px;
        top: 21px;
        left: 11px;
      }

      .hospital-mark::after {
        width: 3px;
        height: 22px;
        top: 11px;
        left: 21px;
      }

      .mark-cross::before {
        width: 7px;
        height: 2px;
        top: -6px;
        right: -4px;
      }

      .mark-cross::after {
        width: 2px;
        height: 7px;
        top: -8px;
        right: -2px;
      }

      .report-pill {
        display: inline-block;
        padding: 6px 12px;
        margin-bottom: 8px;
        border-radius: 999px;
        background: #e5e7eb;
        color: #111827;
        font-size: 0.72rem;
        font-weight: 700;
        letter-spacing: 0.08em;
        text-transform: uppercase;
      }

      .hospital-copy h1,
      .report-title-group h1 {
        margin: 0;
        font-size: 1.25rem;
        font-weight: 800;
        letter-spacing: 0.01em;
      }

      .hospital-copy p,
      .report-title-group p {
        margin: 8px 0 0;
        font-size: 0.86rem;
        color: #4b5563;
        line-height: 1.5;
      }

      .report-meta {
        display: grid;
        grid-template-columns: repeat(3, minmax(120px, 1fr));
        gap: 10px;
        width: 100%;
      }

      .meta-item {
        padding: 10px 12px;
        border: 1px solid #e5e7eb;
        border-radius: 10px;
        background: #fafafa;
      }

      .meta-radiologyel {
        display: block;
        font-size: 0.68rem;
        color: #6b7280;
        text-transform: uppercase;
        letter-spacing: 0.08em;
      }

      .meta-value {
        display: block;
        margin-top: 6px;
        font-size: 0.95rem;
        font-weight: 700;
        color: #111827;
      }

      .section-title {
        margin: 24px 0 8px;
        font-size: 1rem;
        font-weight: 700;
        border-bottom: 1px solid #d1d5db;
        padding-bottom: 8px;
      }

      .details-grid {
        display: grid;
        grid-template-columns: repeat(3, minmax(0, 1fr));
        gap: 14px;
      }

      .detail-card {
        padding: 14px 16px;
        border: 1px solid #e5e7eb;
        border-radius: 10px;
        background: #fafafa;
      }

      .detail-radiologyel {
        display: block;
        font-size: 0.7rem;
        color: #6b7280;
        text-transform: uppercase;
        letter-spacing: 0.08em;
        margin-bottom: 6px;
      }

      .detail-value {
        display: block;
        font-size: 0.95rem;
        font-weight: 700;
        color: #111827;
      }

      .result-table,
      .radiology-results-table {
        width: 100%;
        border-collapse: collapse;
        margin-top: 12px;
        font-size: 0.92rem;
      }

      .result-table th,
      .result-table td,
      .radiology-results-table th,
      .radiology-results-table td {
        padding: 12px 14px;
        border: 1px solid #e5e7eb;
        text-align: left;
        vertical-align: middle;
      }

      .result-table th,
      .radiology-results-table th {
        background: #f3f4f6;
        color: #374151;
        font-size: 0.78rem;
        font-weight: 700;
        text-transform: uppercase;
        letter-spacing: 0.06em;
      }

      .result-table tbody tr:nth-child(even),
      .radiology-results-table tbody tr:nth-child(even) {
        background: #f9fafb;
      }

      .table {
        width: 100%;
        border-collapse: collapse;
      }

      .table-bordered {
        border: 1px solid #e5e7eb;
      }

      .table-bordered th,
      .table-bordered td {
        border: 1px solid #e5e7eb;
        padding: 8px 12px;
      }

      .empty-state {
        padding: 18px 0;
        color: #6b7280;
        font-size: 0.96rem;
      }

      .footer-rule {
        margin: 20px 0 0;
        border-top: 1px solid #d1d5db;
      }

      .report-footer {
        display: flex;
        justify-content: space-between;
        gap: 12px;
        align-items: center;
        margin-top: 12px;
        font-size: 0.82rem;
        color: #6b7280;
      }

      @page {
        size: A4;
        margin: 12mm;
      }

      @media print {
        body {
          background: #ffffff;
        }

        .no-print {
          display: none !important;
        }
      }
    </style>
  `;

  constructor(
    private constantService: ConstantService,
    private authenticationService: AuthenticationService,
    private dialogRef: MatDialogRef<PrintRadiologyOrderResultComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authenticationService.currentUserValue;
    this.currentDate = this.constantService.convertDate(new Date());
    this.currentTime = this.constantService.convertTime(new Date().getTime());
    this.initializeReport();
    console.log('Received data for printing:', this.data);
    console.log('Resolved radiology results:', this.radiologyResults);
  }

  private initializeReport(): void {
    this.radiologyResults = this.getRadiologyResults();
    this.radiologyOrderTitle = this.getRadiologyOrderTitle();
    this.orderStatus = this.getOrderStatus();
    this.clinicalNotesText = this.getClinicalNotes();
  }

  printDocument(): void {
    const printContent = document.getElementById('printDoc');

    if (!printContent) {
      return;
    }

    const printWindow = window.open('', '', 'left=0,top=0,width=1100,height=1100,toolbar=0,scrollbars=1,status=0');
    if (!printWindow) {
      return;
    }

    printWindow.document.open();
    printWindow.document.write(`
      <!doctype html>
      <html>
        <head>
          <title>Radiologyoratory Report</title>
          ${this.printStyles}
        </head>
        <body>
          ${printContent.innerHTML}
        </body>
      </html>
    `);
    printWindow.document.close();

    setTimeout(() => {
      printWindow.focus();
      printWindow.print();
      printWindow.close();
    }, 150);
  }

  closeDialog(): void {
    this.dialogRef.close(true);
  }

  private getSource(): any {
    // Support both direct radiology order or wrapped in element property
    return this.data?.element || this.data || {};
  }

  private getAppointment(): any {
    const source = this.getSource();
    return source?.appointment || source?.patient?.patientAppointments?.[0] || {};
  }

  private getPatient(): any {
    const source = this.getSource();
    return source?.patient || this.getAppointment()?.patient || source || {};
  }

  getHospitalName(): string {
    const source = this.getSource();
    return source?.appointment?.department?.company?.name
      || this.getAppointment()?.department?.company?.name
      || 'Sehat Nizam Diagnostic Center';
  }

  getHospitalSubtitle(): string {
    const company = this.getSource()?.department?.company
      || this.getAppointment()?.department?.company
      || {};
    const parts = [
      company?.address,
      company?.phoneNo || company?.phone || company?.mobileNo,
      company?.email
    ].filter(Boolean);

    return parts.length
      ? parts.join(' · ')
      : '123 Medical Center Drive, Health City · +1 (555) 123-4567 · contact@xyzgroup.com';
  }

  formatAppointmentDate(): string {
    return this.formatDate(this.getAppointment()?.appointmentDate, {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  }

  formatAppointmentTime(): string {
    return this.formatDate(this.getAppointment()?.appointmentDate, {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    });
  }

  formatAppointmentDateTime(): string {
    return this.formatDate(this.getAppointment()?.appointmentDate, {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    });
  }

  formatAppointmentDateLong(): string {
    return this.formatDate(this.getAppointment()?.appointmentDate, {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  }

  getPatientAgeGender(): string {
    const patient = this.getPatient();
    if (!patient || Object.keys(patient).length === 0) {
      return '-';
    }

    const age = patient.age ?? this.calculateAge(patient.dateOfBirth) ?? '-';
    const gender = patient.gender || '-';
    return `${age} / ${gender}`;
  }

  getPatientDob(): string {
    return this.formatDate(this.getPatient()?.dateOfBirth, {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  }

  getPatientPhone(): string {
    const patient = this.getPatient();
    return patient?.phoneNo || patient?.secondaryPhoneNo || '-';
  }

  getPatientAddress(): string {
    return this.getPatient()?.address || '-';
  }

  getDoctorName(): string {
    const doctor = this.getSource()?.doctor || this.getAppointment()?.doctor;
    if (!doctor) {
      return '-';
    }

    const fullName = `${doctor.firstName || ''} ${doctor.lastName || ''}`.trim();
    return fullName || doctor.name || doctor.doctorName || '-';
  }

  getOrderStatus(): string {
    const source = this.getSource();
    return source?.status?.title
      || source?.status?.name
      || source?.appointmentStatus?.name
      || this.getAppointment()?.appointmentStatus?.name
      || source?.status
      || 'Pending';
  }

  getOrderDate(): string {
    return this.formatDate(this.getAppointment()?.appointmentDate, {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  }

  getOrderTime(): string {
    return this.formatDate(this.getAppointment()?.appointmentDate, {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    });
  }

  getTokenNumber(): string {
    const source = this.getSource();
    return source?.tokenNumber
      || this.getAppointment()?.tokenNumber
      || '-';
  }

  getRadiologyOrderTitle(): string {
    const source = this.getSource();
    return source?.radiologyOrderType?.name || 'Radiology Order';
  }

  getRadiologyResults(): any[] {
    const source = this.getSource();
    // CRITICAL FIX: Use 'radiologyResult' (singular) not 'radiologyResults' (plural)
    return source?.radiologyResult || [];
  }

  getClinicalNotes(): string {
    const source = this.getSource();
    return source?.clinicalNotes || '-';
  }

  getPatientName(): string {
    return this.getPatient()?.name || '-';
  }

  getPatientMrn(): string {
    return this.getPatient()?.mrn || '-';
  }

  getDepartmentName(): string {
    const source = this.getSource();
    return source?.department?.name
      || this.getAppointment()?.department?.name
      || '-';
  }

  getPaymentStatus(): string {
    const payment = this.getPayment();
    return payment?.paymentStatus?.name || payment?.paymentStatusName || payment?.status || '-';
  }

  getVisitFee(): number {
    const payment = this.getPayment();
    return Number(payment?.visitFee ?? payment?.amount ?? 0);
  }

  getDiscount(): number {
    const payment = this.getPayment();
    return Number(payment?.discount ?? 0);
  }

  getTotalPayable(): number {
    const payment = this.getPayment();
    const explicitTotal = payment?.totalPayable;

    if (explicitTotal != null) {
      return Number(explicitTotal);
    }

    return this.getVisitFee() - this.getDiscount();
  }

  getClinicalNarrative(): string {
    const element = this.getSource();
    const parts = [
      element?.chiefComplaint,
      element?.assessment,
      element?.notes,
      element?.diagnosis,
      element?.plan,
      element?.reason
    ].filter((value: any, index: number, array: any[]) => !!value && array.indexOf(value) === index);

    return parts.join('\n\n');
  }

  getVitalValue(key: string): string {
    const source = this.getSource();
    const triage = source?.triage
      || source?.latestTriage
      || source?.triageDetail
      || {};

    switch (key) {
      case 'bp': {
        const systolic = triage?.systolicBp ?? triage?.systolicBP;
        const diastolic = triage?.diastolicBp ?? triage?.diastolicBP;
        return systolic || diastolic ? `${systolic ?? '-'} / ${diastolic ?? '-'}` : '';
      }
      case 'pulse':
        return this.toDisplayValue(triage?.pulse);
      case 'temperature':
        return this.toDisplayValue(triage?.temperature);
      case 'spo2':
        return this.toDisplayValue(triage?.spo2);
      case 'weight':
        return this.toDisplayValue(triage?.weight);
      case 'heightCm':
        return this.toDisplayValue(triage?.heightCm ?? triage?.heightCM);
      case 'rbs':
        return this.toDisplayValue(triage?.bloodSugar ?? triage?.rbs);
      default:
        return '';
    }
  }

  private getPayment(): any {
    const element = this.getSource();
    return element?.appointmentPayments?.find((item: any) => item.appointmentId === element?.id)
      || element?.appointmentPayments?.[0]
      || element?.appointmentPayment
      || {};
  }

  private calculateAge(dob: string | Date | null): number | null {
    if (!dob) {
      return null;
    }

    const birthDate = new Date(dob);
    const diff = Date.now() - birthDate.getTime();
    const ageDate = new Date(diff);
    return Math.abs(ageDate.getUTCFullYear() - 1970);
  }

  private formatDate(value: string | Date | null | undefined, options: Intl.DateTimeFormatOptions): string {
    if (!value) {
      return '-';
    }

    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
      return '-';
    }

    return new Intl.DateTimeFormat('en-US', options).format(date);
  }

  private toDisplayValue(value: any): string {
    return value == null || value === '' ? '' : String(value);
  }
}