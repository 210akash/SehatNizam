import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { DoctorService } from '../doctor.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { AccountGroupService } from '../../../accountgroup/accountgroup.service';
import { AccountService } from '../../../account/account.service';
import { MatCheckboxChange } from '@angular/material/checkbox';

@Component({
  selector: 'app-add-doctor-profile',
  templateUrl: './add-doctor-profile.component.html',
  styleUrls: ['./add-doctor-profile.component.css'],
  standalone: false
})
export class AddDoctorProfileComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  isEdit = false;
  doctorName = '';
  doctorDepartment = '';
  doctorDesignation = '';
  doctorCode = '';
  doctorEmail = '';
  doctorPhone = '';
  hospitalAmount = 0;
  accountList: any[] = [];
  accountGroupList: any[] = [];
  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddDoctorProfileComponent>,
    private service: DoctorService,
    private notifications: NotificationsService,
    private accountService: AccountService, 
    private accountgroupService: AccountGroupService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    const doctor = this.data?.element ?? {};
    const profile = doctor?.doctorProfile ?? {};
    const profileId = profile?.id ?? 0;

    this.doctorName = `${doctor?.firstName ?? ''} ${doctor?.lastName ?? ''}`.trim();
    this.doctorDepartment = doctor?.department?.name ?? '';
    this.doctorDesignation = doctor?.employeeDesignation?.name ?? '';
    this.doctorCode = doctor?.hrCode || doctor?.code || '';
    this.doctorEmail = doctor?.email ?? '';
    this.doctorPhone = doctor?.phoneNumber ?? '';

    this.isEdit = Number(profileId) > 0;

    this.form = this.fb.group({
      id: [profileId || 0],
      doctorId: [profile?.doctorId || doctor?.id || '', Validators.required],
      pmdcNumber: [profile?.pmdcNumber || ''],
      qualification: [profile?.qualification || ''],
      experienceYears: [profile?.experienceYears ?? 0, Validators.required],
      biography: [profile?.biography || ''],
      specialization: [profile?.specialization || ''],
      consultationFee: [profile?.consultationFee ?? null],
      hospitalPercentage: [profile?.hospitalPercentage ?? null],
      hospitalAmount: [{ value: 0, disabled: true }],
      isAvailableForOPD: [profile?.isAvailableForOPD ?? true],
      isAvailableForIPD: [profile?.isAvailableForIPD ?? true],
      customFieldsJson: [profile?.customFieldsJson || ''],
      accountId: [null, Validators.required],
      accountName: ['', Validators.required],
      account: [null, Validators.required], // Validation
      accountGroupId: [null, Validators.required],
      accountGroupName: ['', Validators.required],
      accountGroup: [null, Validators.required], // Validation
      isGroup: [false],
    });

  this.form.get('accountId')?.patchValue(profile.account?.id);
      this.form.get('accountName')?.patchValue(profile.account?.code + ' : ' + profile.account?.name);
      this.form.get('account')?.patchValue(profile.account);
      this.form.get('accountGroupId')?.patchValue(profile.account?.accountGroupId);
      this.form.get('accountGroupName')?.patchValue(profile.accountGroup?.code + ' : ' + profile.accountGroup?.name);
      this.form.get('accountGroup')?.patchValue(profile.account?.accountGroup);
      this.form.get('isGroup')?.patchValue(profile.account?.isGroup);

    this.updateHospitalAmount();
    this.form.get('consultationFee')?.valueChanges.subscribe(() => this.updateHospitalAmount());
    this.form.get('hospitalPercentage')?.valueChanges.subscribe(() => this.updateHospitalAmount());
  }

  private updateHospitalAmount(): void {
    const consultationFee = Number(this.form?.get('consultationFee')?.value ?? 0);
    const hospitalPercentage = Number(this.form?.get('hospitalPercentage')?.value ?? 0);

    if (!Number.isFinite(consultationFee) || !Number.isFinite(hospitalPercentage)) {
      this.hospitalAmount = 0;
      return;
    }

    this.hospitalAmount = (consultationFee * hospitalPercentage) / 100;
    this.form?.get('hospitalAmount')?.setValue(this.hospitalAmount, { emitEvent: false });
  }

  save(): void {
    if (this.isLoading) {
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notifications.showNotification('Please fill all required fields.', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    const command = this.form.value;

    this.service.saveDoctorProfile(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.Status === 200) {
          this.notifications.showNotification(res.Data || 'Doctor Profile Saved Successfully!', 'snack-bar-success');
          this.dialogRef.close(true);
        } else if (res.Status === 409) {
          this.notifications.showNotification('Doctor Profile already exists!', 'snack-bar-danger');
        } else {
          this.notifications.showNotification(res.Message || 'Error saving doctor profile!', 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        this.isLoading = false;
        const message = error?.error?.Message || 'An error occurred';
        this.notifications.showNotification(message, 'snack-bar-danger');
      }
    });
  }


    getAccountList(event: any) {
      var filter = event.currentTarget.value;
      this.getAccountByName(filter);
    }
  
    getAccountByName(filter: any) {
      var accountFlow = [''];
      this.accountService.getAccountByName(filter, accountFlow)
        .subscribe((data: any) => {
          this.accountList = data;
        });
    }
  
    onOptionAccountSelected(event: MatAutocompleteSelectedEvent): void {
      const selectedValue = event.option.value;
      this.form.get('accountId')?.patchValue(selectedValue.id);
      this.form.get('accountName')?.patchValue(selectedValue.code + ' : ' + selectedValue.name);
      this.form.get('account')?.patchValue(selectedValue);
    }
  
    getaccount(itemId: string) {
      return this.accountList.find((option: { id: string; }) => option.id === itemId);
    }
  
    onAccountCleared(event: Event): void {
      const inputValue = (event.target as HTMLInputElement)?.value;
      console.log('Current Input Value:', inputValue);
  
      if (!inputValue.trim()) {
        this.form.get('accountId')?.patchValue(null);
        this.form.get('accountName')?.patchValue('');
        this.form.get('account')?.patchValue(null);
  
        this.form.get('accountGroupId')?.patchValue(null);
        this.form.get('accountGroupName')?.patchValue('');
        this.form.get('accountGroup')?.patchValue(null);
      }
    }
  
       showOptions(event:MatCheckboxChange): void {
            this.setvalidators(event.checked);
        }
        
        setvalidators(checked:boolean){
        if(checked){
          // Set Required accountGroup
          this.form.get('accountGroupId')?.setValidators([Validators.required]);
          this.form.get('accountGroupId')?.updateValueAndValidity();
          this.form.get('accountGroupName')?.setValidators([Validators.required]);
          this.form.get('accountGroupName')?.updateValueAndValidity();
          this.form.get('accountGroup')?.setValidators([Validators.required]);
          this.form.get('accountGroup')?.updateValueAndValidity();
          //Clear Required account
          this.form.get('accountId')?.clearValidators();
          this.form.get('accountId')?.updateValueAndValidity();
          this.form.get('accountName')?.clearValidators();
          this.form.get('accountName')?.updateValueAndValidity();
          this.form.get('account')?.clearValidators();
          this.form.get('account')?.updateValueAndValidity();
        }
        else{
         // Set Required account
         this.form.get('accountId')?.setValidators([Validators.required]);
         this.form.get('accountId')?.updateValueAndValidity();
         this.form.get('accountName')?.setValidators([Validators.required]);
         this.form.get('accountName')?.updateValueAndValidity();
         this.form.get('account')?.setValidators([Validators.required]);
         this.form.get('account')?.updateValueAndValidity();
         //Clear Required accountGroup
         this.form.get('accountGroupId')?.clearValidators();
         this.form.get('accountGroupId')?.updateValueAndValidity();
         this.form.get('accountGroupName')?.clearValidators();
         this.form.get('accountGroupName')?.updateValueAndValidity();
         this.form.get('accountGroup')?.clearValidators();
         this.form.get('accountGroup')?.updateValueAndValidity();
        }
      }
      
      getAccountGroupList(event: any) {
        // Clone the form value and add paging data
        const filter = event.currentTarget.value;
        const accountFlow: string[] = [];
        this.accountGroupList = [];  // Empty the list before updating
        this.accountgroupService.getAccountGroupByName(filter, accountFlow).subscribe(
          (data: any) => {
            this.accountGroupList = data || []; // Ensure it's an array even if no data is returned
          },
          (error) => {
            console.error('Error fetching account list:', error);
            this.accountGroupList = [];  // Reset in case of an error
          }
        );
      }
  
        getaccountgroup(itemId: string) {
          return this.accountGroupList.find((option: { id: string; }) => option.id === itemId);
        }
      
    onOptionGroupSelected(event: MatAutocompleteSelectedEvent): void {
      const selectedValue = event.option.value;
      this.form.get('accountGroupId')?.patchValue(selectedValue.id);
      this.form.get('accountGroupName')?.patchValue(selectedValue.code + ' : ' + selectedValue.name);
      this.form.get('accountGroup')?.patchValue(selectedValue);
    }
}
