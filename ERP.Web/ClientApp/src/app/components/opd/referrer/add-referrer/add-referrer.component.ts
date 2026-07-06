import { Component, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ReferrerService } from '../referrer.service';
import { ConstantService } from '../../../../Service/constant.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatSelectChange } from '@angular/material/select';
import { AccountService } from '../../../account/account.service';
import { AccountGroupService } from '../../../accountgroup/accountgroup.service';

@Component({
  selector: 'app-add-referrer',
  templateUrl: './add-referrer.component.html',
  styleUrls: ['./add-referrer.component.css'],
  standalone: false
})
export class AddReferrerComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  departments: any[] = [];
  isEditMode: boolean = false;
  accountList: any[] = [];
  accountGroupList: any[] = [];
  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private Referrer: ReferrerService,
    private notifications: NotificationsService,
    private constantService: ConstantService,
      private accountService: AccountService, 
    private accountgroupService: AccountGroupService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      id: [0],
      name: ['', Validators.required],
      hospital: ['', Validators.required],
      phoneNo: [''],
      accountId: [null, Validators.required],
      accountName: ['', Validators.required],
      account: [null, Validators.required], // Validation
      accountGroupId: [null, Validators.required],
      accountGroupName: ['', Validators.required],
      accountGroup: [null, Validators.required], // Validation
      isGroup: [false],
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.form);
      this.form.get('accountId')?.patchValue(element.account?.id);
      this.form.get('accountName')?.patchValue(element.account?.code + ' : ' + element.account?.name);
      this.form.get('account')?.patchValue(element.account);
      this.form.get('accountGroupId')?.patchValue(element.account?.accountGroupId);
      this.form.get('accountGroupName')?.patchValue(element.accountGroup?.code + ' : ' + element.accountGroup?.name);
      this.form.get('accountGroup')?.patchValue(element.account?.accountGroup);
      this.form.get('isGroup')?.patchValue(element.account?.isGroup);
    }
  }

  save(): void {
    if (this.form.invalid) return;

    this.isLoading = true;
    const command = this.form.value;

    this.Referrer.saveReferrer(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.Status === 200) {
          this.notifications.showNotification(res.Data || 'Referrer Saved Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        } else if (res.Status === 409) {
          this.notifications.showNotification('Referrer with this code already exists!', 'snack-bar-danger');
        } else {
          this.notifications.showNotification(res.Message || 'Error saving Referrer!', 'snack-bar-danger');
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
