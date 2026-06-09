import { Component, Inject } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { ServiceService } from '../../../opd/service/service.service';
import { AdmissionPackageService } from '../admission-package.service';

@Component({
    selector: 'app-add-admission-package',
    templateUrl: './add-admission-package.component.html',
    styleUrl: './add-admission-package.component.css',
    standalone: false
})
export class AddAdmissionPackageComponent {
    packageForm!: FormGroup;
    isLoading = false;
    isEditMode = false;
    isViewMode = false;
    serviceList: any[] = [];
    packageDetails: any[] = [];

    constructor(
        private dialog: MatDialog,
        private notificationsService: NotificationsService,
        private formBuilder: FormBuilder,
        private admissionPackageService: AdmissionPackageService,
        private serviceService: ServiceService,
        private constantService: ConstantService,
        @Inject(MAT_DIALOG_DATA) public data: { element: any; isViewMode?: boolean }
    ) { }

    ngOnInit(): void {
        this.isViewMode = this.data.isViewMode === true;

        this.packageForm = this.formBuilder.group({
            id: [0],
            name: ['', Validators.required],
            description: [''],
            admissionPackageDetail: this.formBuilder.array([])
        });

        this.loadServices();
        this.loadData(this.data.element);
    }

    get dialogTitle(): string {
        if (this.isViewMode) {
            return 'View Admission Package';
        }
        return this.isEditMode ? 'Edit Admission Package' : 'Add Admission Package';
    }

    get admissionPackageDetail(): FormArray {
        return this.packageForm.get('admissionPackageDetail') as FormArray;
    }

    loadServices() {
        const serviceFilter: any = { PagingData: { currentPage: 0, take: 1000 } };
        this.serviceService.getAllServices(serviceFilter).subscribe((data: any) => {
            this.serviceList = data.item1 || [];
        });
    }

    loadData(element: any) {
        if (element != null) {
            this.isEditMode = !this.isViewMode;
            this.isLoading = true;
            this.admissionPackageService.getAdmissionPackageById(element.id).subscribe({
                next: (pkg: any) => {
                    this.packageForm.patchValue({
                        id: pkg.id,
                        name: pkg.name,
                        description: pkg.description
                    });

                    this.packageDetails = pkg.admissionPackageDetail || [];

                    this.admissionPackageDetail.clear();
                    this.packageDetails.forEach((detail: any) => {
                        this.admissionPackageDetail.push(this.createDetailRow(detail));
                    });

                    if (this.isViewMode) {
                        this.packageForm.disable();
                    }

                    this.isLoading = false;
                },
                error: () => {
                    this.isLoading = false;
                    this.notificationsService.showNotification('Failed to load package', 'snack-bar-danger');
                }
            });
        } else {
            this.addDetailRow();
        }
    }

    createDetailRow(detail?: any): FormGroup {
        return this.formBuilder.group({
            id: [detail?.id || 0],
            admissionPackageMasterId: [detail?.admissionPackageMasterId || 0],
            serviceId: [detail?.serviceId || null, Validators.required]
        });
    }

    addDetailRow() {
        this.admissionPackageDetail.push(this.createDetailRow());
    }

    removeDetailRow(index: number) {
        if (this.admissionPackageDetail.length > 1) {
            this.admissionPackageDetail.removeAt(index);
        }
    }

    getServiceName(serviceId: number): string {
        const service = this.serviceList.find(s => s.id === serviceId);
        return service ? `${service.code} - ${service.name}` : '';
    }

    getSelectedServiceIds(currentIndex: number): number[] {
        return this.admissionPackageDetail.controls
            .map((control, index) => index !== currentIndex ? control.get('serviceId')?.value : null)
            .filter((id: number) => id != null);
    }

    isServiceDisabled(serviceId: number, currentIndex: number): boolean {
        return this.getSelectedServiceIds(currentIndex).includes(serviceId);
    }

    getServiceById(serviceId: number): any {
        return this.serviceList.find(s => s.id === serviceId);
    }

    getRowBasePrice(index: number): number | null {
        const serviceId = this.admissionPackageDetail.at(index).get('serviceId')?.value;
        if (!serviceId) {
            return null;
        }
        return this.getServiceById(serviceId)?.basePrice ?? null;
    }

    get selectedServicesCount(): number {
        return this.admissionPackageDetail.controls
            .map(control => control.get('serviceId')?.value)
            .filter((id: number) => id != null).length;
    }

    get totalPackageAmount(): number {
        return this.admissionPackageDetail.controls.reduce((total, control) => {
            const serviceId = control.get('serviceId')?.value;
            if (!serviceId) {
                return total;
            }
            const basePrice = this.getServiceById(serviceId)?.basePrice ?? 0;
            return total + Number(basePrice);
        }, 0);
    }

    get viewTotalAmount(): number {
        return this.packageDetails.reduce((total, detail) => {
            return total + Number(detail?.service?.basePrice ?? 0);
        }, 0);
    }

    saveData() {
        if (this.isViewMode) {
            return;
        }

        if (this.packageForm.invalid || this.admissionPackageDetail.length === 0) {
            this.constantService.markFormGroupTouched(this.packageForm);
            if (this.admissionPackageDetail.length === 0) {
                this.notificationsService.showNotification('Add at least one service', 'snack-bar-danger');
            }
            return;
        }

        this.isLoading = true;
        const payload = Object.assign({}, this.packageForm.getRawValue());

        this.admissionPackageService.saveAdmissionPackage(payload).subscribe({
            next: (data: { Status: number; Data: string }) => {
                if (data.Status == 200) {
                    this.notificationsService.showNotification(data.Data, 'snack-bar-success');
                    this.dialog.closeAll();
                } else {
                    this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
                }
                this.isLoading = false;
            },
            error: (error: string) => {
                this.notificationsService.showNotification(error, 'snack-bar-danger');
                this.isLoading = false;
            }
        });
    }
}
