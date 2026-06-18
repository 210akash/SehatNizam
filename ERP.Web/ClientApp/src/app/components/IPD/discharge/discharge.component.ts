import { Component, Inject } from '@angular/core';
import {  FormBuilder,FormGroup,Validators} from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { AdmissionService } from '../admission/admission.service';

@Component({
  selector: 'app-discharge',
  templateUrl: './discharge.component.html',
  styleUrl: './discharge.component.css',
  standalone: false,
})
export class AddDischargeComponent {
   dischargeForm!: FormGroup;
   isLoading = false;
   dialogRef: any;
   selectedIndexSearch: number = 0;

  constructor(
    private dialog: MatDialog,
    private notificationsService: NotificationsService,
    private formBuilder: FormBuilder,
    private admissionService: AdmissionService,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

  ngOnInit(): void {
    this.dischargeForm = this.formBuilder.group({
      id: [0],
      admissionId: [this.data.element.id],
      operationDeliveryDateTime: [null],
      diagnosis: ['', Validators.required],
      hopi: [''],
      examinationAndFindings: [''],
      investigationsResults: [''],
      procedure: [''],
      surgeonName: [''],
      operativeFindings: [''],
      operationNotes: [''],
      conditionAtDischarge: [''],
      treatmentAdvisedAtDischarge: [''],
      proposedFollowUpDateTime: [null],
      dietAndInstructions: [''],
      dischargeDoctorId: [null],
      dischargeDateTime: [new Date(), Validators.required],
    });
  }

  SaveData() {
    if (this.dischargeForm.invalid) {
      this.constantService.markFormGroupTouched(this.dischargeForm);
      this.notificationsService.showNotification(
        'Please Fill Required Fields',
        'snack-bar-danger'
      );
      return;
    }

    this.isLoading = true;
    const formValue = this.dischargeForm.value;
    let _dischargeForm: any = {
      Id: formValue.id,
      AdmissionId: formValue.admissionId,
      OperationDeliveryDateTime: formValue.operationDeliveryDateTime,
      Diagnosis: formValue.diagnosis,
      Hopi: formValue.hopi,
      ExaminationAndFindings: formValue.examinationAndFindings,
      InvestigationsResults: formValue.investigationsResults,
      Procedure: formValue.procedure,
      SurgeonName: formValue.surgeonName,
      OperativeFindings: formValue.operativeFindings,
      OperationNotes: formValue.operationNotes,
      ConditionAtDischarge: formValue.conditionAtDischarge,
      TreatmentAdvisedAtDischarge: formValue.treatmentAdvisedAtDischarge,
      ProposedFollowUpDateTime: formValue.proposedFollowUpDateTime,
      DietAndInstructions: formValue.dietAndInstructions,
      DischargeDoctorId: formValue.dischargeDoctorId,
      DischargeDateTime: formValue.dischargeDateTime,
    };

    this.admissionService.saveDischarge(_dischargeForm).subscribe({
      next: (data: { Status: number; Data: string;Message: string; }) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(
            data.Data,
            'snack-bar-success'
          );
          this.dialog.closeAll();
        }
        else if (data.Status == 500) {
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');     
        }
        else
          this.notificationsService.showNotification(
            data.Data,
            'snack-bar-danger'
          );
        this.isLoading = false;
      },
      error: (error: any) => {
        const errorMessage = error.error?.Message || error.error?.Data || error.statusText || 'An unexpected error occurred.';
        this.notificationsService.showNotification(errorMessage, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
    },
    });
  }
}
