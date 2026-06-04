import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AuthenticationService } from '../../../../Auth/authentication.service';
import { ConstantService } from '../../../../Service/constant.service';
import QRCode from 'qrcode';

@Component({
  selector: 'app-print-appoinment',
  templateUrl: './print-appoinment.component.html',
  styleUrls: ['./print-appoinment.component.css'],
  standalone: false
})
export class PrintAppoinmentComponent {
  currentUser: any;
  currentDate: any;
  currentTime: any;
  qrCodeUrl = '';
  qrCells: boolean[] = [
    true, true, true, false, true, false, true, true, true,
    true, false, true, true, false, true, true, false, true,
    true, true, true, false, true, false, true, true, true,
    false, true, false, true, false, true, false, true, false,
    true, false, true, true, false, true, true, false, true,
    false, true, false, true, true, false, true, false, true,
    true, true, true, false, true, false, true, true, true,
    true, false, true, true, false, true, true, false, true,
    true, true, true, false, true, false, true, true, true
  ];

  private readonly printStyles = `
    <style>
      body {
        margin: 0;
        font-family: Arial, Helvetica, sans-serif;
        color: #3a3a3a;
        background: #ffffff;
        -webkit-print-color-adjust: exact;
        print-color-adjust: exact;
      }

      *,
      ::before,
      ::after {
        box-sizing: border-box;
      }

      .slip-sheet {
        width: 100%;
        padding: 0;
        color: #404040;
      }

      .slip-top {
        display: flex;
        justify-content: center;
        align-items: center;
        gap: 16px;
      }

      .hospital-mark {
        position: relative;
        width: 36px;
        height: 36px;
        border: 2px solid #3e3e3e;
        border-radius: 8px;
      }

      .hospital-mark::before,
      .hospital-mark::after,
      .mark-cross::before,
      .mark-cross::after {
        content: "";
        position: absolute;
        background: #3e3e3e;
      }

      .hospital-mark::before {
        width: 18px;
        height: 3px;
        top: 16px;
        left: 7px;
      }

      .hospital-mark::after {
        width: 3px;
        height: 18px;
        top: 7px;
        left: 16px;
      }

      .mark-cross::before {
        width: 6px;
        height: 2px;
        top: -5px;
        right: -3px;
      }

      .mark-cross::after {
        width: 2px;
        height: 6px;
        top: -7px;
        right: -1px;
      }

      .hospital-copy {
        text-align: center;
      }

      .hospital-copy h1 {
        margin: 0;
        font-size: 18px;
        font-weight: 800;
        letter-spacing: 0.02em;
      }

      .hospital-copy p {
        margin: 4px 0 0;
        font-size: 10px;
        color: #7a7a7a;
      }

      .top-rule,
      .section-rule,
      .footer-rule {
        border-top: 2px solid #202020;
      }

      .top-rule {
        margin: 12px 0 18px;
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
        font-size: 9px;
        font-weight: 700;
        text-transform: uppercase;
        color: #7b7b7b;
      }

      .summary-pair strong {
        font-size: 13px;
        line-height: 1.2;
        color: #2f2f2f;
      }

      .summary-side {
        display: flex;
        align-items: center;
        gap: 10px;
      }

      .token-circle {
        width: 64px;
        height: 64px;
        border: 2px solid #202020;
        border-radius: 50%;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
        overflow: hidden;
        padding: 3px;
        text-align: center;
      }

      .token-label {
      font-size: 0.44rem;
  font-weight: 700;
  text-transform: uppercase;
  line-height: 1;
      }

      .token-value {
        font-size:  0.7rem;
  font-weight: 500;
  line-height: 1;
  margin-top: 2px;
      }

      .qr-box {
        width: 78px;
        height: 78px;
        border: 1px solid #d4d4d4;
        padding: 4px;
        background: #fff;
        overflow: hidden;
        display: flex;
        align-items: center;
        justify-content: center;
      }

      .qr-image {
        display: block;
        width: 100%;
        height: 100%;
        object-fit: contain;
      }

      .qr-grid {
        display: grid;
        grid-template-columns: repeat(9, 1fr);
        gap: 2px;
        width: 100%;
        height: 100%;
      }

      .qr-grid span {
        background: transparent;
      }

      .qr-grid span.filled {
        background: #111;
      }

      .section-title {
        margin: 18px 0 8px;
        font-size: 15px;
        font-weight: 700;
      }

      .section-rule {
        margin-bottom: 16px;
        border-top-width: 1px;
        border-top-color: #d9d9d9;
      }

      .clinical-board {
        display: grid;
        grid-template-columns: 124px 1fr;
        min-height: 580px;
        border: 1px solid #d8d8d8;
      }

      .vitals-column {
        padding: 10px;
        border-right: 1px solid #d8d8d8;
      }

      .vitals-title {
        margin-bottom: 12px;
        text-align: center;
        font-size: 12px;
        font-weight: 800;
        text-transform: uppercase;
      }

      .vital-box {
        min-height: 58px;
        margin-bottom: 10px;
        border: 1px solid #dddddd;
        border-radius: 4px;
        overflow: hidden;
        display: flex;
        flex-direction: column;
      }

      .vital-name {
        padding: 9px 6px;
        text-align: center;
        font-size: 11px;
        font-weight: 800;
        border-bottom: 1px solid #e4e4e4;
      }

      .vital-value {
        flex: 1;
        padding: 8px 6px;
        text-align: center;
        font-size: 12px;
        color: #4b4b4b;
      }

      .notes-column {
        position: relative;
        padding: 10px 12px 18px;
      }

      .notes-title {
        font-size: 11px;
        font-weight: 700;
        text-transform: uppercase;
        color: #b7b7b7;
      }

      .notes-space {
        min-height: 485px;
        padding-top: 10px;
        font-size: 11px;
        line-height: 1.5;
        white-space: pre-wrap;
      }

      .notes-space p {
        margin: 0;
      }

      .signature-area {
        position: absolute;
        right: 16px;
        bottom: 16px;
        width: 172px;
        text-align: center;
      }

      .signature-line {
        border-top: 1px solid #202020;
        margin-bottom: 6px;
      }

      .signature-area strong {
        display: block;
        font-size: 13px;
      }

      .signature-area span {
        font-size: 9px;
        font-weight: 700;
        text-transform: uppercase;
      }

      .footer-rule {
        margin-top: 16px;
        border-top-width: 1px;
        border-top-color: #e1e1e1;
      }

      .slip-footer {
        padding-top: 6px;
        text-align: center;
      }

      .footer-warning {
        font-size: 8px;
        font-weight: 700;
        color: #333;
      }

      .footer-meta {
        margin-top: 6px;
        font-size: 7px;
        color: #666;
      }

      @page {
        size: A4;
        margin: 10mm;
      }
    </style>
  `;

  constructor(
    private constantService: ConstantService,
    private authenticationService: AuthenticationService,
    private dialogRef: MatDialogRef<PrintAppoinmentComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authenticationService.currentUserValue;
    this.currentDate = this.constantService.convertDate(new Date());
    this.currentTime = this.constantService.convertTime(new Date().getTime());
    const appointmentId = this.data?.element?.id ?? '';
    void this.generateQrCode(`appointmentId:${appointmentId}`);
  }

  private async generateQrCode(value: string): Promise<void> {
    this.qrCodeUrl = await QRCode.toDataURL(value, {
      width: 160,
      margin: 1,
      errorCorrectionLevel: 'M'
    });
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
          <title>Patient Encounter Form</title>
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

  getHospitalName(): string {
    return this.data?.element?.department?.company?.name;
  }

  getHospitalSubtitle(): string {
    const company = this.data?.element?.department?.company;
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
    return this.formatDate(this.data?.element?.appointmentDate, {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  }

  formatAppointmentTime(): string {
    return this.formatDate(this.data?.element?.appointmentDate, {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    });
  }

  formatAppointmentDateTime(): string {
    return this.formatDate(this.data?.element?.appointmentDate, {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    });
  }

  formatAppointmentDateLong(): string {
    return this.formatDate(this.data?.element?.appointmentDate, {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  }

  formatCurrency(value: number): string {
    return Number(value || 0).toLocaleString(undefined, {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    });
  }

  getPatientAgeGender(): string {
    const patient = this.data?.element?.patient;
    if (!patient) {
      return '-';
    }

    const age = patient.age ?? this.calculateAge(patient.dateOfBirth) ?? '-';
    const gender = patient.gender || '-';
    return `${age} / ${gender}`;
  }

  getDoctorName(): string {
    const doctor = this.data?.element?.doctor;
    if (!doctor) {
      return '-';
    }

    const fullName = `${doctor.firstName || ''} ${doctor.lastName || ''}`.trim();
    return fullName || doctor.name || doctor.doctorName || '-';
  }

  getAppointmentStatus(): string {
    return this.data?.element?.appointmentStatus?.name
      || this.data?.element?.status?.name
      || this.data?.element?.status
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
    const element = this.data?.element;
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
    const triage = this.data?.element?.triage
      || this.data?.element?.appoinment?.triage
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
    const element = this.data?.element;
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
