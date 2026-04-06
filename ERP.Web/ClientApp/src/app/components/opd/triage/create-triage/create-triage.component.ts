import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { TriageService } from '../triage.service';
import { TriageCategoryService } from '../../triage-category/triage-category.service';
import { PriorityLevelService } from '../../prioritylevel/prioritylevel.service';
import { SugarTypeService } from '../../sugar-type/sugar-type.service';

@Component({
  selector: 'app-create-triage',
  templateUrl: './create-triage.component.html',
  styleUrls: ['./create-triage.component.css'],standalone: false
})
export class CreateTriageComponent implements OnInit {
  createTriageForm!: FormGroup;
  isLoading = false;
  triageCategories: any[] = [];
  triagePriorities: any[] = [];
  sugarTypes: any[] = [];
  isEditMode = false;

  constructor(
    private notificationsService: NotificationsService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    private triageService: TriageService,
    private triageCategoryService: TriageCategoryService,
    private priorityLevelService: PriorityLevelService,
    private sugarTypeService: SugarTypeService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.buildForm();
    this.loadDropdowns();
    this.LoadData(this.data?.element);
    this.registerBmiCalculation();
  }

  get f() {
    return this.createTriageForm.controls;
  }

  private buildForm() {
    this.createTriageForm = this.formBuilder.group({
      id: [0],
      appointmentId: [null, Validators.required],
      nurseId: [null],
      temperature: [null],
      pulse: [null],
      systolicBp: [null],
      diastolicBp: [null],
      spo2: [null],
      weight: [null],
      heightFeet: [null],
      heightInches: [null],
      heightCm: [null],
      bmi: [{ value: null, disabled: true }],
      bloodSugar: [null],
      sugarTypeId: [null, Validators.required],
      triagePriorityId: [null, Validators.required],
      chiefComplaint: [''],
      allergies: [''],
      medications: [''],
      notes: [''],
      triageScore: [null],
      triageCategoryId: [null, Validators.required]
    });
  }

  private registerBmiCalculation() {
    this.createTriageForm.valueChanges.subscribe(() => {
      const weight = Number(this.createTriageForm.get('weight')?.value);
      const heightCm = Number(this.createTriageForm.get('heightCm')?.value);
      if (!!weight && !!heightCm) {
        const heightM = heightCm / 100;
        const bmi = weight / (heightM * heightM);
        this.createTriageForm.get('bmi')?.setValue(Number(bmi.toFixed(2)), { emitEvent: false });
      }
    });
  }

  async loadDropdowns() {
    await this.loadTriageCategories();
    await this.loadTriagePriorities();
    await this.loadSugarTypes();
  }

  private async loadTriageCategories() {
    let filter: any = { pagingData: { currentPage: 0, take: 1000 } };
    (await this.triageCategoryService.getAllTriageCategory(filter)).subscribe(data => {
      this.triageCategories = data?.item1 ?? data ?? [];
    });
  }

  private async loadTriagePriorities() {
    let filter: any = { pagingData: { currentPage: 0, take: 1000 } };
    (await this.priorityLevelService.getAllPriorityLevel(filter)).subscribe(data => {
      this.triagePriorities = data?.item1 ?? data ?? [];
    });
  }

  private async loadSugarTypes() {
    let filter: any = { pagingData: { currentPage: 0, take: 1000 } };
    (await this.sugarTypeService.getAllSugarType(filter)).subscribe(data => {
      this.sugarTypes = data?.item1 ?? data ?? [];
    });
  }

  async saveTriage() {
    this.isLoading = true;
    if (this.createTriageForm.invalid) {
      this.constantService.markFormGroupTouched(this.createTriageForm);
      this.isLoading = false;
      return;
    }

    let payload: any = {};
    payload = Object.assign(payload, this.createTriageForm.getRawValue());

    (await this.triageService.saveTriage(payload)).subscribe({
      next: (data: { Status: number; Message?: string; Data?: string; }) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Message || 'Triage Saved Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        }
        else if (data.Status == 409) {
          this.notificationsService.showNotification(data.Message || 'Record already exists!', 'snack-bar-danger');
        }
        else {
          this.notificationsService.showNotification(data.Message || 'There is some error!', 'snack-bar-danger');
        }
        this.isLoading = false;
      },
      error: (error: any) => {
        this.notificationsService.showNotification('Please Fill the required fields!', 'snack-bar-danger');
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  LoadData(element: any) {
    if (element?.id != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.createTriageForm);
      if (element?.bmi) {
        this.createTriageForm.get('bmi')?.setValue(element.bmi, { emitEvent: false });
      }
    }
    console.log(this.createTriageForm);
  }
}
