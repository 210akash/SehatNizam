import { Component, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AuthenticationService } from '../../../../Auth/authentication.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { RadiologyOrderService } from '../radiologyorder.service';
import { environment } from '../../../../../environments/environment';

interface RadiologyImagePreview {
  url: string;
  fileName: string;
  isImage: boolean;
}

@Component({
  selector: 'app-save-radiology-result',
  templateUrl: './save-radiology-result.component.html',
  styleUrls: ['./save-radiology-result.component.css'],
  standalone: false
})
export class SaveRadiologyResultComponent implements OnInit {
  form: FormGroup;
  isSaving = false;
  isLoading = false;
  order: any;
  currentUser: any;
  imagePreviews: RadiologyImagePreview[] = [];

  constructor(
    private fb: FormBuilder,
    private radiologyOrderService: RadiologyOrderService,
    private authenticationService: AuthenticationService,
    private notifications: NotificationsService,
    private dialogRef: MatDialogRef<SaveRadiologyResultComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { order: any }
  ) {
    this.form = this.fb.group({
      id: [0],
      radiologyOrderId: [0, Validators.required],
      performedDate: [new Date(), Validators.required],
      clinicalHistory: [''],
      findings: [''],
      impression: [''],
      conclusion: [''],
      images: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.currentUser = this.authenticationService.currentUserValue;
    this.isLoading = true;

    this.radiologyOrderService.getRadiologyOrderById(this.data.order.id).subscribe({
      next: (res: any) => {
        this.order = this.normalizeOrder(res?.Data ?? res?.data ?? res, this.data.order);
        this.buildForm(this.order);
        this.isLoading = false;
      },
      error: () => {
        this.order = this.normalizeOrder(this.data.order);
        this.buildForm(this.order);
        this.isLoading = false;
      }
    });
  }

  get patient(): any {
    const patient = this.order?.appointment?.patient;
    if (!patient) {
      return {};
    }

    const master = patient.patientMaster ?? {};
    return {
      name: master.name ?? patient.name ?? '',
      mrn: patient.mrn ?? '',
      gender: master.gender ?? patient.gender ?? '',
      age: master.age ?? patient.age ?? '',
      phoneNo: master.phoneNo ?? patient.phoneNo ?? ''
    };
  }

  get imagesFormArray(): FormArray {
    return this.form.get('images') as FormArray;
  }

  getTestName(): string {
    return this.order?.radiologyType?.name
      || this.order?.radiologyOrderType?.name
      || '-';
  }

  private normalizeOrder(primary: any, fallback?: any): any {
    const order = primary ?? fallback ?? {};
    const appointment = order.appointment ?? fallback?.appointment ?? {};
    const patient = appointment.patient ?? fallback?.appointment?.patient ?? {};
    const patientMaster = patient.patientMaster ?? fallback?.appointment?.patient?.patientMaster;

    return {
      ...fallback,
      ...order,
      appointment: {
        ...fallback?.appointment,
        ...appointment,
        patient: {
          ...patient,
          patientMaster: patientMaster ?? patient.patientMaster
        },
        doctor: appointment.doctor ?? fallback?.appointment?.doctor
      },
      radiologyType: order.radiologyType ?? fallback?.radiologyType ?? order.radiologyOrderType ?? fallback?.radiologyOrderType,
      radiologyStudyResult: order.radiologyStudyResult ?? fallback?.radiologyStudyResult
    };
  }

  buildForm(order: any): void {
    const existing = order?.radiologyStudyResult;

    this.form.patchValue({
      id: existing?.id ?? 0,
      radiologyOrderId: order.id,
      performedDate: existing?.performedDate ? new Date(existing.performedDate) : new Date(),
      clinicalHistory: existing?.clinicalHistory || order.clinicalNotes || '',
      findings: existing?.findings || '',
      impression: existing?.impression || '',
      conclusion: existing?.conclusion || ''
    });

    this.imagesFormArray.clear();
    this.imagePreviews = [];

    (existing?.images || []).forEach((image: any, index: number) => {
      this.imagesFormArray.push(this.createImageRow(image, index));
      this.imagePreviews.push({
        url: this.resolveImageUrl(image.imageUrl),
        fileName: this.getFileNameFromUrl(image.imageUrl),
        isImage: this.isImageUrl(image.imageUrl)
      });
    });
  }

  createImageRow(image: any = {}, index = 0): FormGroup {
    return this.fb.group({
      id: [image?.id ?? 0],
      imageUrl: [image?.imageUrl ?? ''],
      fileName: [image?.fileName ?? this.getFileNameFromUrl(image?.imageUrl)],
      extension: [image?.extension ?? this.getExtensionFromUrl(image?.imageUrl)],
      sequenceNo: [image?.sequenceNo ?? index + 1],
      remarks: [image?.remarks ?? '']
    });
  }

  onFileBrowse(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) {
      return;
    }

    Array.from(input.files).forEach((file) => {
      const extension = file.name.split('.').pop()?.toLowerCase() ?? 'png';
      const reader = new FileReader();

      reader.onload = () => {
        const imageUrl = reader.result as string;
        const nextIndex = this.imagesFormArray.length;

        this.imagesFormArray.push(this.createImageRow({
          imageUrl,
          fileName: file.name,
          extension,
          sequenceNo: nextIndex + 1
        }, nextIndex));

        this.imagePreviews.push({
          url: imageUrl,
          fileName: file.name,
          isImage: file.type.startsWith('image/')
        });
      };

      reader.readAsDataURL(file);
    });

    input.value = '';
  }

  removeImage(index: number): void {
    this.imagesFormArray.removeAt(index);
    this.imagePreviews.splice(index, 1);
    this.resequenceImages();
  }

  moveImage(index: number, direction: -1 | 1): void {
    const target = index + direction;
    if (target < 0 || target >= this.imagesFormArray.length) {
      return;
    }

    const currentControl = this.imagesFormArray.at(index);
    const targetControl = this.imagesFormArray.at(target);
    this.imagesFormArray.setControl(index, targetControl);
    this.imagesFormArray.setControl(target, currentControl);

    const currentPreview = this.imagePreviews[index];
    this.imagePreviews[index] = this.imagePreviews[target];
    this.imagePreviews[target] = currentPreview;

    this.resequenceImages();
  }

  resequenceImages(): void {
    this.imagesFormArray.controls.forEach((group, index) => {
      group.get('sequenceNo')?.setValue(index + 1, { emitEvent: false });
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notifications.showNotification('Please complete required report fields.', 'snack-bar-danger');
      return;
    }

    this.isSaving = true;

    const performedDate = this.form.value.performedDate;
    const payload = {
      id: this.form.value.id || 0,
      radiologyOrderId: this.form.value.radiologyOrderId,
      performedById: this.currentUser?.userId,
      reportedById: this.currentUser?.userId,
      performedDate: performedDate instanceof Date ? performedDate.toISOString() : performedDate,
      clinicalHistory: this.form.value.clinicalHistory,
      findings: this.form.value.findings,
      impression: this.form.value.impression,
      conclusion: this.form.value.conclusion,
      images: this.imagesFormArray.value.map((image: any, index: number) => ({
        id: image.id || 0,
        radiologyStudyResultId: this.form.value.id || 0,
        imageUrl: image.imageUrl,
        fileName: image.fileName,
        extension: image.extension,
        sequenceNo: image.sequenceNo || index + 1,
        remarks: image.remarks
      }))
    };

    this.radiologyOrderService.saveRadiologyResult(payload).subscribe({
      next: (res: any) => {
        this.isSaving = false;
        if (res?.Status === 200) {
          this.notifications.showNotification('Radiology result saved successfully!', 'snack-bar-success');
          this.dialogRef.close(true);
        } else {
          this.notifications.showNotification(res?.Message || 'Unable to save radiology result.', 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        this.isSaving = false;
        const msg = error?.error?.Message || 'An unexpected error occurred.';
        this.notifications.showNotification(msg, 'snack-bar-danger');
      }
    });
  }

  resolveImageUrl(url: string): string {
    if (!url) {
      return '';
    }

    if (url.startsWith('data:') || url.startsWith('http')) {
      return url;
    }

    // Uploaded files live under Angular assets (see LocalBlob config), not API wwwroot
    if (url.startsWith('/assets/')) {
      return url;
    }

    const base = environment.dev_uri.replace('/api', '');
    return url.startsWith('/') ? `${base}${url}` : `${base}/${url}`;
  }

  isImageUrl(url: string): boolean {
    if (!url) {
      return false;
    }

    if (url.startsWith('data:image')) {
      return true;
    }

    return /\.(png|jpe?g|gif|webp|bmp|svg)$/i.test(url);
  }

  private getFileNameFromUrl(url: string): string {
    if (!url) {
      return 'image';
    }

    if (url.startsWith('data:')) {
      return 'uploaded-image';
    }

    const parts = url.split('/');
    return parts[parts.length - 1] || 'image';
  }

  private getExtensionFromUrl(url: string): string {
    const fileName = this.getFileNameFromUrl(url);
    return fileName.includes('.') ? fileName.split('.').pop()?.toLowerCase() ?? 'png' : 'png';
  }
}
