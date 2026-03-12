import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { AccountGroupService } from '../accountgroup.service';
import { AccountService } from '../../account/account.service';
import { DealershipService } from '../../order/dealership/dealership.service';
import { VendorService } from '../../vendor/vendor.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'app-add-accountgroup',
  templateUrl: './add-accountgroup.component.html',
  styleUrl: './add-accountgroup.component.css',
  standalone: false
})

export class AddAccountGroupComponent {
  accountgroupForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  accountList :any;
  customersList :any;
  vendorsList :any;
  accountFlow :any;
  i : number = 0;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private accountgroupService: AccountGroupService,
    private accountService: AccountService,
    private dealershipService: DealershipService,
    private vendorService: VendorService,
    private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.accountgroupForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      accountId: ['', Validators.required],
      dealershipId: [''],
      dealershipName: [''],
      vendorId : [''],
      vendorName: [''],
      opening: [0],
      creditLimit: [0],
    });

    this.LoadData(this.data.element);
    this.getAccountList();
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;

      this.constantService.LoadData(element, this.accountgroupForm);
      this.accountFlow = element.account?.accountFlow?.name;
      this.accountgroupForm.get('vendorName')?.patchValue(element.vendor?.name + ' | ' +  element.vendor?.address );
      this.accountgroupForm.get('dealershipName')?.patchValue(element.dealership?.name + ' | ' +  element.dealership?.address );
      this.setValidators();
      this.getAccountList();
    }
  }

  SaveData() {
    if (this.accountgroupForm.invalid) {
      this.constantService.markFormGroupTouched(this.accountgroupForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.accountgroupForm.value);
    this.accountgroupService.saveAccountGroup(_clienttemperatureForm).subscribe({
      next: (data: { Status: number; Data: string; }) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error: string) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }

  getAccountList() {
    let accountgrouptypeFilter  = {};
    this.accountService.getGroupAccount(accountgrouptypeFilter).subscribe((data: any) => {
     this.accountList = data;
     this.getaccountFlow();
    });
  }

  getaccountFlow() {
    var accountId = this.accountgroupForm.get('accountId')?.value;
    return this.accountList.find((option: { id: string; }) => option.id === accountId).accountFlow?.name;
  }

  async getCustomersList(event: any) {
    // Clone the form value and add paging data
    const filter = event.currentTarget.value;
    const accountFlow: string[] = [];
    this.customersList = [];  // Empty the list before updating
    (await this.dealershipService.getAllByName(filter)).subscribe(
      (data: any) => {
        this.customersList = data || []; // Ensure it's an array even if no data is returned
      },
      (error:any) => {
        console.error('Error fetching account list:', error);
        this.customersList = [];  // Reset in case of an error
      }
    );
  }

  async getSupplierList(event: any) {
    // Clone the form value and add paging data
    const filter = event.currentTarget.value;
    const accountFlow: string[] = [];
    this.vendorsList = [];  // Empty the list before updating
    (await this.vendorService.getVendorByName(filter)).subscribe(
      (data: any) => {
        this.vendorsList = data || []; // Ensure it's an array even if no data is returned
      },
      (error:any) => {
        console.error('Error fetching account list:', error);
        this.vendorsList = [];  // Reset in case of an error
      }
    );
  }


  async getVendorsList() {
    let accountgrouptypeFilter  = {};
    (await this.vendorService.getAllVendors(accountgrouptypeFilter)).subscribe((data: any) => {
     this.vendorsList = data.item1;
    });
  }

  getAccountGroupCode() {
    this.accountFlow =  this.getaccountFlow();
    var AccountGroupTypeId =  this.accountgroupForm.get('accountId')?.value;
    var Id =  this.accountgroupForm.get('id')?.value;
    this.accountgroupService.getAccountGroupCode(AccountGroupTypeId,Id).subscribe((data: any) => {
      this.accountgroupForm.get('code')?.patchValue(data.code);
    });
    this.accountgroupForm.get('dealershipId')?.patchValue('');
    this.accountgroupForm.get('vendorId')?.patchValue('');
    this.setValidators();
  }

  setValidators(){
    this.customersList = [];
    this.vendorsList = [];
    if(this.accountFlow == 'Customers'){
      // this.getCustomersList();
      // Set Required customer
      this.accountgroupForm.get('dealershipId')?.setValidators([Validators.required]);
      this.accountgroupForm.get('dealershipId')?.updateValueAndValidity();
      //Clear Required vendor
      this.accountgroupForm.get('vendorId')?.clearValidators();
      this.accountgroupForm.get('vendorId')?.updateValueAndValidity();
    }
    else if(this.accountFlow == 'Suppliers'){
      this.getVendorsList();
      // Set Required vendor
      this.accountgroupForm.get('vendorId')?.setValidators([Validators.required]);
      this.accountgroupForm.get('vendorId')?.updateValueAndValidity();
      //Clear Required vendor
      this.accountgroupForm.get('dealershipId')?.clearValidators();
      this.accountgroupForm.get('dealershipId')?.updateValueAndValidity();
    }
    else{
            //Clear Required vendor
            this.accountgroupForm.get('vendorId')?.clearValidators();
            this.accountgroupForm.get('vendorId')?.updateValueAndValidity();

          //Clear Required customer
          this.accountgroupForm.get('dealershipId')?.clearValidators();
          this.accountgroupForm.get('dealershipId')?.updateValueAndValidity();
    }
  }

  setName(id: string) {
    this.accountgroupForm.get('name')?.patchValue('');
    this.accountgroupForm.get('description')?.patchValue('');
    if(this.accountFlow == 'Customers'){
      const selectedVendor = this.customersList.find((customer: { id: any; }) => customer.id === id);
      if (selectedVendor) {
        this.accountgroupForm.get('name')?.patchValue(selectedVendor.name);
        this.accountgroupForm.get('description')?.patchValue(selectedVendor.address);
      }
    }
    else{
      const selectedVendor = this.vendorsList.find((vendor: { id: any; }) => vendor.id === id);
      if (selectedVendor) {
        this.accountgroupForm.get('name')?.patchValue(selectedVendor.name);
        this.accountgroupForm.get('description')?.patchValue(selectedVendor.address);
      }
    }
  }

  onInputCleared(event: Event, index: number): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      console.log(`Input cleared at row index: ${index}`);
      //this.resetitem(index); // Call a function when cleared
    }
  }

  onOptionDealershipSelected(event: MatAutocompleteSelectedEvent, index: number): void {
      const selectedValue = event.option.value;
      if (!selectedValue) {
        console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
        return;
      }
      this.accountgroupForm.get('dealershipId')?.patchValue(selectedValue.id);
      this.accountgroupForm.get('dealershipName')?.patchValue(selectedValue.name + ' | ' + selectedValue.address);
  }

  onOptionSupplierSelected(event: MatAutocompleteSelectedEvent, index: number): void {
      const selectedValue = event.option.value;
      if (!selectedValue) {
        console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
        return;
      }
      this.accountgroupForm.get('vendorId')?.patchValue(selectedValue.id);
      this.accountgroupForm.get('vendorName')?.patchValue(selectedValue.name + ' | ' + selectedValue.address);
  }
}
