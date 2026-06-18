import { Component, Inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { debounceTime, switchMap } from 'rxjs/operators';
import { AdmissionPackageService } from '../admission-package.service';
import { ServiceService } from '../../../opd/service/service.service';

@Component({
  selector: 'app-add-admission-package',
  templateUrl: './add-admission-package.component.html',
  styleUrls: ['./add-admission-package.component.css'],
  standalone: false
})
export class AddAdmissionPackageComponent {

  packageForm!: FormGroup;
  isLoading = false;
  isViewMode = false;
  isEditMode = false;

  serviceSearchCtrl = new FormControl('');
  filteredServices: any[] = [];
  selectedServices: any[] = [];

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private admissionPackageService: AdmissionPackageService,
    private serviceService: ServiceService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  ngOnInit(): void {

    this.isViewMode = this.data?.isViewMode === true;

    this.packageForm = this.fb.group({
      id: [0],
      name: ['', Validators.required],
      description: ['']
    });

    // SEARCH STREAM
    this.serviceSearchCtrl.valueChanges.pipe(
      debounceTime(300),
      switchMap(value => this.searchServices(value || ''))
    ).subscribe((res: any) => {
      const extractServices = (data: any): any[] => {
        if (!data) return [];
        
        // Common backend response wrappers
        const unwrap = (obj: any) => {
          if (!obj || typeof obj !== 'object') return null;
          
          // Check for common wrapper properties (both camelCase and PascalCase)
          const wrapperProps = ['Data', 'data', 'item1', 'response', 'result', 'value'];
          const candidates = [obj, ...wrapperProps.map(prop => obj[prop]).filter(Boolean)];
          for (const candidate of candidates) {
            if (Array.isArray(candidate)) {
              return candidate;
            }
            if (candidate && typeof candidate === 'object') {
              // Check if this looks like a service (has required properties)
              if (candidate.id !== undefined && (candidate.code !== undefined || candidate.name !== undefined)) {
                return [candidate];
              }
              // Check for nested service
              if (candidate.service && typeof candidate.service === 'object') {
                return [candidate.service];
              }
            }
          }
          
          // Last resort: if it's an object with id
          if (obj.id !== undefined) return [obj];
          return null;
        };
        
        const unwrapped = unwrap(data);
        if (!unwrapped) return [];
        
        return Array.isArray(unwrapped) ? unwrapped : [unwrapped];
      };
      this.filteredServices = extractServices(res);
    });

    this.loadData(this.data?.element);
  }

  get dialogTitle(): string { if (this.isViewMode) { return 'View Admission Package'; } return this.isEditMode ? 'Edit Admission Package' : 'Add Admission Package'; }

  displayService(service: any): string {
    if (!service) return '';
    return `${service.code ?? ''} - ${service.name ?? ''} (${service.basePrice ?? 0})`;
  }

  // SEARCH API
  searchServices(name: string) {
    return this.serviceService.getServiceName(name,null);
  }

  // SELECT SERVICE
  onServiceSelected(event: any) {
    let service = event.option.value;

    // EXTRACT NESTED SERVICE IF WRAPPED
    if (service && typeof service === 'object' && service.service) {
      service = service.service;
    }
    
    // SAFETY CHECK
    if (!service || typeof service !== 'object') return;

    if (!Array.isArray(this.selectedServices)) {
      this.selectedServices = [];
    }

    const exists = this.selectedServices.some(s => s.id === service.id);

    if (!exists) {
      this.selectedServices = [...this.selectedServices, service]; // immutable safe
    }

    this.serviceSearchCtrl.setValue('');
  }

  // REMOVE CHIP
  removeService(index: number) {
    this.selectedServices.splice(index, 1);
  }

  // TOTAL COUNT
  get selectedServicesCount(): number {
    return this.selectedServices.length;
  }

  // TOTAL PRICE
  get totalPackageAmount(): number {
    return this.selectedServices.reduce((sum, s) => {
      return sum + Number(s.basePrice || 0);
    }, 0);
  }

  // LOAD DATA (EDIT / VIEW)
  loadData(element: any) {
    if (!element) {
      return;
    }

    this.isEditMode = !this.isViewMode;

    this.admissionPackageService.getAdmissionPackageById(element.id)
      .subscribe((res: any) => {

        this.packageForm.patchValue({
          id: res.id,
          name: res.name,
          description: res.description
        });

        const detail = res.admissionPackageDetail;
        if (Array.isArray(detail)) {
          this.selectedServices = detail
            .map((x: { service: any }) => x.service)
            .filter((x: any) => x != null);
        } else if (detail && typeof detail === 'object') {
          this.selectedServices = [detail.service].filter((x: any) => x != null);
        } else {
          this.selectedServices = [];
        }
        if (this.isViewMode) {
          this.packageForm.disable();
        }
      });
  }

  // SAVE
  saveData() {

    if (this.packageForm.invalid || this.selectedServices.length === 0) {
      return;
    }

    this.isLoading = true;

    const payload = {
      ...this.packageForm.value,
      admissionPackageDetail: this.selectedServices.map(s => ({
        serviceId: s.id
      }))
    };

    this.admissionPackageService.saveAdmissionPackage(payload)
      .subscribe({
        next: (res: any) => {
          this.isLoading = false;
          this.dialog.closeAll();
        },
        error: () => {
          this.isLoading = false;
        }
      });
  }
}