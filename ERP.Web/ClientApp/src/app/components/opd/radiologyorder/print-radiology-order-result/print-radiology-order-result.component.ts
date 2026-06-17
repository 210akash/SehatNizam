import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AuthenticationService } from '../../../../Auth/authentication.service';
import { ConstantService } from '../../../../Service/constant.service';
import { RadiologyOrderService } from '../radiologyorder.service';

interface ReportImage {
  url: string;
  fileName: string;
  sequenceNo: number;
  remarks: string;
  isImage: boolean;
}

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
  order: any;
  studyResult: any = null;
  reportImages: ReportImage[] = [];
  radiologyOrderTitle = 'Radiology Order';
  clinicalNotesText = '-';
  isLoading = true;

  private readonly printStyles = `
    <style>
      body { margin: 0; font-family: Arial, Helvetica, sans-serif; color: #232323; background: #fff; -webkit-print-color-adjust: exact; print-color-adjust: exact; }
      *, ::before, ::after { box-sizing: border-box; }
      .slip-sheet { width: 100%; margin: 0; padding: 10mm; background: #fff; }
      .report-header { text-align: center; margin-bottom: 8px; }
      .slip-top { display: flex; align-items: center; justify-content: center; gap: 14px; }
      .hospital-copy h1 { margin: 0; font-size: 1.15rem; font-weight: 800; }
      .hospital-copy p { margin: 4px 0 0; font-size: 0.68rem; color: #666; }
      .report-pill { display: inline-block; margin-top: 10px; padding: 5px 14px; border-radius: 999px; background: #eef2f7; font-size: 0.68rem; font-weight: 700; letter-spacing: 0.08em; text-transform: uppercase; }
      .top-rule, .footer-rule { border-top: 2px solid #202020; margin: 12px 0; }
      .section-rule { border-top: 1px solid #d9d9d9; margin-bottom: 14px; }
      .summary-card { display: flex; justify-content: space-between; gap: 16px; padding: 12px; border: 1px solid #d4d4d4; border-radius: 8px; }
      .patient-summary-grid { flex: 1; display: grid; grid-template-columns: repeat(3, minmax(0, 1fr)); gap: 10px 20px; }
      .summary-head { font-size: 0.62rem; font-weight: 700; text-transform: uppercase; color: #7b7b7b; }
      .summary-pair strong { font-size: 0.86rem; color: #2f2f2f; }
      .token-circle { width: 58px; height: 58px; border: 2px solid #202020; border-radius: 50%; display: flex; flex-direction: column; align-items: center; justify-content: center; }
      .token-label { font-size: 0.44rem; font-weight: 700; text-transform: uppercase; }
      .token-value { font-size: 0.5rem; margin-top: 2px; }
      .study-banner { margin: 14px 0 8px; padding: 10px 12px; border-left: 4px solid #1e88e5; background: #f8fafc; }
      .study-banner h2 { margin: 0; font-size: 1rem; }
      .study-banner p { margin: 4px 0 0; font-size: 0.75rem; color: #64748b; }
      .report-block { margin-bottom: 14px; page-break-inside: avoid; }
      .report-block h4 { margin: 0 0 6px; font-size: 0.76rem; text-transform: uppercase; color: #555; letter-spacing: 0.04em; }
      .report-block p { margin: 0; white-space: pre-wrap; line-height: 1.55; font-size: 0.88rem; }
      .image-grid { display: grid; grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 12px; }
      .image-item { margin: 0; page-break-inside: avoid; }
      .image-item img { width: 100%; max-height: 220px; object-fit: contain; border: 1px solid #d4d4d4; border-radius: 6px; background: #f8fafc; }
      .file-chip { min-height: 100px; padding: 16px; border: 1px solid #d4d4d4; border-radius: 6px; text-align: center; font-size: 0.75rem; }
      .image-caption { margin-top: 4px; font-size: 0.68rem; color: #666; display: flex; justify-content: space-between; gap: 8px; }
      .signature-area { margin-top: 24px; width: 200px; margin-left: auto; text-align: center; }
      .signature-line { border-top: 1px solid #202020; margin-bottom: 6px; }
      .signature-area strong { display: block; font-size: 0.84rem; }
      .signature-area span { font-size: 0.62rem; text-transform: uppercase; color: #666; }
      .empty-state { padding: 24px; text-align: center; color: #7a7a7a; }
      .slip-footer { text-align: center; padding-top: 8px; }
      .footer-warning { font-size: 0.58rem; font-weight: 700; }
      .footer-meta { margin-top: 6px; font-size: 0.54rem; color: #666; }
      @page { size: A4; margin: 10mm; }
      @media print { .no-print { display: none !important; } }
    </style>
  `;

  constructor(
    private constantService: ConstantService,
    private authenticationService: AuthenticationService,
    private radiologyOrderService: RadiologyOrderService,
    private dialogRef: MatDialogRef<PrintRadiologyOrderResultComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authenticationService.currentUserValue;
    this.currentDate = this.constantService.convertDate(new Date());
    this.currentTime = this.constantService.convertTime(new Date().getTime());

    const element = this.data?.element ?? this.data;
    this.radiologyOrderService.getRadiologyOrderById(element.id).subscribe({
      next: (res: any) => {
        this.order = this.normalizeOrder(res?.Data ?? res?.data ?? res, element);
        this.initializeReport();
        this.isLoading = false;
      },
      error: () => {
        this.order = this.normalizeOrder(element);
        this.initializeReport();
        this.isLoading = false;
      }
    });
  }

  private initializeReport(): void {
    this.studyResult = this.getSource()?.radiologyStudyResult ?? null;
    this.radiologyOrderTitle = this.getRadiologyOrderTitle();
    this.clinicalNotesText = this.getSource()?.clinicalNotes || '-';
    this.reportImages = (this.studyResult?.images ?? [])
      .slice()
      .sort((a: any, b: any) => (a.sequenceNo ?? 0) - (b.sequenceNo ?? 0))
      .map((image: any) => ({
        url: this.resolveImageUrl(image.imageUrl, true),
        fileName: this.getFileNameFromUrl(image.imageUrl),
        sequenceNo: image.sequenceNo ?? 0,
        remarks: image.remarks ?? '',
        isImage: this.isImageUrl(image.imageUrl)
      }));
  }

  hasReportContent(): boolean {
    return !!(
      this.studyResult?.clinicalHistory ||
      this.studyResult?.findings ||
      this.studyResult?.impression ||
      this.studyResult?.conclusion ||
      this.reportImages.length > 0
    );
  }

  printDocument(): void {
    const printContent = document.getElementById('printDoc');
    if (!printContent) return;

    const printWindow = window.open('', '', 'left=0,top=0,width=1100,height=1100,toolbar=0,scrollbars=1,status=0');
    if (!printWindow) return;

    printWindow.document.open();
    printWindow.document.write(`
      <!doctype html>
      <html>
        <head>
          <title>Radiology Report - ${this.getPatientName()}</title>
          ${this.printStyles}
        </head>
        <body>${printContent.innerHTML}</body>
      </html>
    `);
    printWindow.document.close();

    setTimeout(() => {
      printWindow.focus();
      printWindow.print();
      printWindow.close();
    }, 300);
  }

  closeDialog(): void {
    this.dialogRef.close(true);
  }

  getOrderId(): string {
    return String(this.getSource()?.id ?? '-');
  }

  getOrderStatus(): string {
    return this.getSource()?.status?.title
      || this.getSource()?.status?.name
      || 'Completed';
  }

  private getSource(): any {
    return this.order ?? this.data?.element ?? this.data ?? {};
  }

  private getAppointment(): any {
    return this.getSource()?.appointment ?? {};
  }

  private getPatient(): any {
    const patient = this.getAppointment()?.patient ?? {};
    const master = patient.patientMaster ?? {};
    return {
      name: master.name ?? patient.name ?? '',
      mrn: patient.mrn ?? '',
      gender: master.gender ?? patient.gender ?? '',
      age: master.age ?? patient.age ?? '',
      phoneNo: master.phoneNo ?? patient.phoneNo ?? '',
      dateOfBirth: master.dateOfBirth ?? patient.dateOfBirth
    };
  }

  getHospitalName(): string {
    return this.getAppointment()?.department?.company?.name || 'Sehat Nizam Diagnostic Center';
  }

  getHospitalSubtitle(): string {
    const company = this.getAppointment()?.department?.company ?? {};
    const parts = [company?.address, company?.phoneNo || company?.phone, company?.email].filter(Boolean);
    return parts.length ? parts.join(' · ') : '';
  }

  formatPerformedDate(): string {
    const date = this.studyResult?.performedDate || this.getAppointment()?.appointmentDate;
    return this.formatDate(date, { day: '2-digit', month: 'short', year: 'numeric' });
  }

  formatAppointmentDateLong(): string {
    return this.formatDate(this.getAppointment()?.appointmentDate, {
      day: '2-digit', month: 'short', year: 'numeric'
    });
  }

  getPatientAgeGender(): string {
    const patient = this.getPatient();
    const age = patient.age ?? this.calculateAge(patient.dateOfBirth) ?? '-';
    return `${age} / ${patient.gender || '-'}`;
  }

  getPatientPhone(): string {
    return this.getPatient().phoneNo || '-';
  }

  getDoctorName(): string {
    const doctor = this.getAppointment()?.doctor;
    if (!doctor) return '-';
    return doctor.fullName || doctor.name || `${doctor.firstName || ''} ${doctor.lastName || ''}`.trim() || '-';
  }

  getTokenNumber(): string {
    return this.getAppointment()?.tokenNumber || '-';
  }

  getRadiologyOrderTitle(): string {
    const source = this.getSource();
    return source?.radiologyType?.name || source?.radiologyOrderType?.name || 'Radiology Order';
  }

  getPatientName(): string {
    return this.getPatient().name || '-';
  }

  getPatientMrn(): string {
    return this.getPatient().mrn || '-';
  }

  getDepartmentName(): string {
    return this.getAppointment()?.department?.name || '-';
  }

  resolveImageUrl(url: string, forPrint = false): string {
    if (!url) return '';
    if (url.startsWith('data:') || url.startsWith('http')) return url;

    if (url.startsWith('/assets/')) {
      return forPrint ? `${window.location.origin}${url}` : url;
    }

    const absolute = url.startsWith('/') ? url : `/${url}`;
    return forPrint ? `${window.location.origin}${absolute}` : absolute;
  }

  isImageUrl(url: string): boolean {
    if (!url) return false;
    if (url.startsWith('data:image')) return true;
    return /\.(png|jpe?g|gif|webp|bmp|svg)$/i.test(url);
  }

  private getFileNameFromUrl(url: string): string {
    if (!url || url.startsWith('data:')) return 'image';
    const parts = url.split('/');
    return parts[parts.length - 1] || 'image';
  }

  private normalizeOrder(primary: any, fallback?: any): any {
    const order = primary ?? fallback ?? {};
    const appointment = order.appointment ?? fallback?.appointment ?? {};
    const patient = appointment.patient ?? fallback?.appointment?.patient ?? {};

    return {
      ...fallback,
      ...order,
      appointment: {
        ...fallback?.appointment,
        ...appointment,
        patient: {
          ...patient,
          patientMaster: patient.patientMaster ?? fallback?.appointment?.patient?.patientMaster
        },
        doctor: appointment.doctor ?? fallback?.appointment?.doctor
      },
      radiologyType: order.radiologyType ?? fallback?.radiologyType ?? order.radiologyOrderType,
      radiologyStudyResult: order.radiologyStudyResult ?? fallback?.radiologyStudyResult,
      status: order.status ?? fallback?.status
    };
  }

  private calculateAge(dob: string | Date | null): number | null {
    if (!dob) return null;
    const birthDate = new Date(dob);
    const diff = Date.now() - birthDate.getTime();
    return Math.abs(new Date(diff).getUTCFullYear() - 1970);
  }

  private formatDate(value: string | Date | null | undefined, options: Intl.DateTimeFormatOptions): string {
    if (!value) return '-';
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return '-';
    return new Intl.DateTimeFormat('en-US', options).format(date);
  }
}
