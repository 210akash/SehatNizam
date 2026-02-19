import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { IndentTypeService } from '../../indenttype/indenttype.service';
import { StoreService } from '../../store/store.service';
import { DepartmentService } from '../../department/department.service';
import { ProjectService } from '../../project/project.service';
import { AccountService } from '../../account/account.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { MediaService } from '../../../Service/media.service';
import { Subject, takeUntil } from 'rxjs';
import { TransactionService } from '../../transaction/transaction.service';
import { AccountSubcategoryService } from '../../accountsubcategory/accountsubcategory.service';
import { AccountTypeService } from '../../accounttype/accounttype.service';
import { AccountCategoryService } from '../../accountcategory/accountcategory.service';

@Component({
  selector: 'app-add-crv',
  templateUrl: './add-crv.component.html',
  styleUrl: './add-crv.component.css',
  standalone: false
})

export class AddCrvComponent {
  transactionForm!: FormGroup;
  accountSearchForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  indentTypeList : any;
  storeList : any;
  transactionTypeList: any;
  accountList: any[] = [];
  isdataload: boolean = false;
  departmentList : any;
  projectList : any;
  tDebit  = 0;
  tCredit  = 0;
  urlSafe: SafeResourceUrl | undefined;
  dialogRef: any;
  uploadedMedia: Array<any> = [];
  documents: any[] = [];
  categoryList :any;
  subcategoryList :any;
  accountTypeList :any;
  accountSearchList :any;
  selectedIndexSearch : number = 0;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private indentTypeService: IndentTypeService, private storeService: StoreService, private transactionService: TransactionService, private departmentService: DepartmentService, private projectService: ProjectService, 
    private accountService: AccountService, 
    private mediaService: MediaService, public sanitizer: DomSanitizer,
         private subcategoryService: AccountSubcategoryService, private accounttypeService: AccountTypeService,
         private categoryService: AccountCategoryService,
    private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.transactionForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      date: [new Date(), Validators.required],
      voucherTypeId: [2, Validators.required],
      status: [''], // Validation
      remarks: [''], // Validation
      chequeNo: [''], // Validation
      chequeTitle: [''], // Validation
      chequeDate: [null],
      chequeClearDate: [null],
      paidReceiveBy: [''], // Validation
      referenceNumber: [''], // Validation
      statusName: ['New'], // Validation
      statusId: [1], // Validation
      transactionDetails: this.formBuilder.array([]) // Initialize as a FormArray
    });

    this.accountSearchForm = this.formBuilder.group({
      accountCategoryId: ['', Validators.required],
      accountSubCategoryId: ['', Validators.required],
      accountTypeId: ['', Validators.required],
      account: ['', Validators.required],
      accountNameSearch: ['', Validators.required], // Validation
      accountId: ['', Validators.required],
    });

    this.LoadData(this.data.element);
    this.getprojectList();
    this.getdepartmentList();
    this.getcategoryList();
  }

  get transactionDetails(): FormArray {
    return this.transactionForm.get('transactionDetails') as FormArray;
  }

  addTransactionDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0], // Default value
      transactionId: [0], // Default value
      departmentId: [0, Validators.required],
      projectId: [0, Validators.required],
      accountId: ['', Validators.required], // Validation
      account: ['', Validators.required], // Validation
      accountName: ['', Validators.required], // Validation
      debitAmount:  [0, Validators.required], // Validation
      creditAmount:  [0, Validators.required], // Validation
    });

    // Insert the new group after the current index
    this.transactionDetails.insert(index + 1, newDetailGroup);
  }

  removeTransactionDetail(index: number) {
    if (this.transactionDetails.length > 1) {
      this.transactionDetails.removeAt(index);
    } else {
      this.notificationsService.showNotification('At least one Account is required.', 'snack-bar-danger');
    }
  }

  getuomValue(index: number): any {
    const detailControl = (this.transactionForm.get('transactionDetails') as FormArray).at(index);
    return detailControl?.value.item?.uom?.name || '';
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.transactionForm);

      // Populate the transactionDetails FormArray
      const detailsArray = this.transactionForm.get('transactionDetails') as FormArray;
      detailsArray.clear(); // Clear existing data

      if (element.transactionDetails && element.transactionDetails.length > 0) {
        element.transactionDetails.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            transactionId: [detail.transactionId],
            accountId: [detail.accountId, Validators.required],
            account: [detail.account ? detail.account : '', Validators.required],
            accountName: [detail.account ? detail.account.code + ':' +  detail.account.name : '', Validators.required],
            departmentId: [detail.departmentId, Validators.required],
            projectId: [detail.projectId, Validators.required],
            debitAmount: [detail.debitAmount ,Validators.required], // Validation
            creditAmount: [detail.creditAmount,Validators.required], // Validation
          });
          detailsArray.push(detailGroup);
        });
      }

      this.data.element.transactionDocuments.forEach((filedata: any) => {
        this.uploadedMedia.push({
          FileName: filedata.path,
          FileSize: this.mediaService.getFileSize(filedata.path) + ' ' + this.mediaService.getFileSizeUnit(filedata.path),
          FileType: filedata.path,
          FileUrl: filedata.path,
          FileProgessSize: 0,
          FileProgress: 0,
          ngUnsubscribe: new Subject<any>(),
        });
        this.documents.push({
          id: filedata.id,
          transactionId : filedata.transactionId,
          path: filedata.path,
        });
      });
      this.calculateTotals();

    }
    else {
      this.addTransactionDetail(0);
      this.transactionForm.get('date')?.patchValue(this.constantService.formatDate(new Date()));
      this.getTransactionCode();
    }
  }

  SaveData() {
    if (this.transactionForm.invalid) {
      this.constantService.markFormGroupTouched(this.transactionForm);
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    if (this.tDebit != this.tCredit) {
      this.notificationsService.showNotification('Debit is not equal to credit.', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _transactionForm: any = {};
    _transactionForm = Object.assign(_transactionForm, this.transactionForm.value);
    let date = new Date(this.transactionForm.get('date')?.value);
    _transactionForm['date'] = date.toLocaleDateString();
    _transactionForm['transactionDocuments'] = this.documents;

    this.transactionService.saveTransaction(_transactionForm).subscribe({
      next: (data: { Status: number; Data: string; }) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error: any) => {
        this.notificationsService.showNotification(error.statusText, 'snack-bar-danger');
        console.error(error.statusText);
        this.isLoading = false;
      }
    });
  }

  getTransactionCode() {
    var VoucherTypeId = this.transactionForm.get('voucherTypeId')?.value;
    this.transactionService.getTransactionCode(VoucherTypeId).subscribe((data: any) => {
      this.transactionForm.get('code')?.patchValue(data.code);
    });
  }

  getAccountList(event: any) {
    var filter = event.currentTarget.value;
    var accountFlow = [''];

    this.accountService.getAccountByName(filter, accountFlow)
        .subscribe((data: any) => {
            this.accountList = data;
        });
}

  onOptionSelected(event: MatAutocompleteSelectedEvent, index: number): void {
    debugger
    const selectedValue = event.option.value;

    const transactionDetailArray = this.transactionForm.get('transactionDetails') as FormArray;

    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');

      return;
    }

    // Get the FormArray for transactionDetails

    // Check if index is valid
    if (!transactionDetailArray || index < 0 || index >= transactionDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    // Check if the selected itemId already exists in the form array (excluding the current index)
    const duplicateItem = transactionDetailArray.controls
      .filter((control: AbstractControl, controlIndex: number) => controlIndex !== index) // Exclude the current index
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;  // Cast AbstractControl to FormGroup
        return formGroup.get('accountId')?.value === selectedValue.id;  // Check if the itemId already exists
      });

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = transactionDetailArray.at(index) as FormGroup;
      currentFormGroup.get('id')?.patchValue(0);
      currentFormGroup.get('transactionId')?.patchValue(0);
      currentFormGroup.get('accountId')?.patchValue(0);
      currentFormGroup.get('accountName')?.patchValue('');
      currentFormGroup.get('account')?.patchValue(null);
      currentFormGroup.get('debitAmount')?.patchValue(0);
      currentFormGroup.get('creditAmount')?.patchValue(0);
      // This will clear all the values of the current form group
      return; // Do nothing if the itemId already exists
    }
    else {
      // Get the selected item details from your getaccount method
      const selectedItem = this.getaccount(selectedValue.id);
      if (!selectedItem) {
        console.error('Selected item not found.');
        return;
      }

      // Get the form group for the current index
      const detailFormGroup = transactionDetailArray.at(index) as FormGroup;

      // Patch the values into the form group
       detailFormGroup.get('accountId')?.patchValue(selectedValue.id);
       detailFormGroup.get('accountName')?.patchValue(selectedValue.code + ' : ' + selectedValue.name);
      detailFormGroup.get('account')?.patchValue(selectedValue);
    }
  }

  getaccount(itemId: string) {
    return this.accountList.find((option: { id: string; }) => option.id === itemId);
  }

  displayFn(item: any): string {
    if(item != ""){

    // If the item is an object, display its code and name
    if (item && item.code && item.name) {
      return `${item.code} : ${item.name}`;
    }

    // If it's just an itemId (number), find the item in the transactionDetails array
    else if (typeof item === 'number' && this.isdataload == false) {
      // Find the first FormGroup where the itemId matches

      var selectedItem = this.data.element.transactionDetails.filter((element: any) => {
        return element.itemId == item;
      })

      // If found, return the formatted string, else return an empty string
      return selectedItem[0] ? `${selectedItem[0].item?.code} : ${selectedItem[0].item.name}` : '';
    }

    // Return empty string by default if no valid item found
    return '';
  }
  else
  return '';
  }

  onInputCleared(event: Event, index: number): void {
    const inputValue = (event.target as HTMLInputElement).value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      console.log(`Input cleared at row index: ${index}`);
      this.resetitem(index); // Call a function when cleared
    }
  }

  resetitem(index: number) {
    const transactionDetailArray = this.transactionForm.get('transactionDetails') as FormArray;

    // Check if index is valid
    if (!transactionDetailArray || index < 0 || index >= transactionDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = transactionDetailArray.at(index) as FormGroup;
    currentFormGroup.get('id')?.patchValue(0);
    currentFormGroup.get('transactionId')?.patchValue(0);
    currentFormGroup.get('accountId')?.patchValue(0);
    currentFormGroup.get('accountName')?.patchValue('');
    currentFormGroup.get('account')?.patchValue(null);
    currentFormGroup.get('quantity')?.patchValue(0);
    currentFormGroup.get('debitAmount')?.patchValue(0);
    currentFormGroup.get('creditAmount')?.patchValue(0);
    this.accountList = [];  // Clear account list
    return; // Do nothing if the itemId already exists
  }

  getdepartmentList() {
    let _departmentFilter: any = {};
    this.departmentService.getAllDepartments(_departmentFilter).subscribe((data: any) => {
     this.departmentList = data.item1;
    });
  }
  
  getprojectList() {
    let _projectFilter: any = {};
    this.projectService.getAllProjects(_projectFilter).subscribe((data: any) => {
     this.projectList = data.item1;
    });
  }

  reset() {
    this.transactionForm.get('code')?.patchValue('');
    this.transactionTypeList = [];
  }

  async changeDrCr(type:any,index:number) {
 
    const transactionDetailArray = this.transactionForm.get('transactionDetails') as FormArray;

    // Get the form group for the current index
    const detailFormGroup = transactionDetailArray.at(index) as FormGroup;
    if(type == 'dr'){
      detailFormGroup.get('creditAmount')?.patchValue(0);
      detailFormGroup.get('creditAmount')?.setValidators([Validators.required, Validators.min(0), Validators.max(0)]);
      detailFormGroup.get('debitAmount')?.setValidators([Validators.required, Validators.min(1)]);
    }
    else{
      detailFormGroup.get('debitAmount')?.patchValue(0);
      detailFormGroup.get('debitAmount')?.setValidators([Validators.required, Validators.min(0), Validators.max(0)]);
      detailFormGroup.get('creditAmount')?.setValidators([Validators.required, Validators.min(1)]);
    }
      detailFormGroup.updateValueAndValidity();
      this.calculateTotals();
  }
  
  async calculateTotals() {
    var _tdebitAmount = 0;
    var _tcreditAmount = 0;

      (this.transactionDetails.controls as FormGroup[]).forEach((control: FormGroup) => {
        // Access item and update the unitRate value
        _tdebitAmount  = _tdebitAmount + control.get('debitAmount')?.value;
        _tcreditAmount  = _tcreditAmount + control.get('creditAmount')?.value;
      });

      this.tDebit = _tdebitAmount;
      this.tCredit = _tcreditAmount;
  }


  onFileBrowse(event: any) {
    const target = event.target as HTMLInputElement;
    if (target.files && target.files.length > 0) {
      this.processFiles(target.files);
    }
  }

  processFiles(files: FileList) {
    for (const file of Array.from(files)) {
      const fileName = file.name;
      const fileExtension = fileName.split('.').pop()?.toLowerCase();
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = (event: any) => {
        const fileUrl = event.target.result;
        const documentObj = {
          id: 0,
          path: fileUrl,
          fileName: fileName,
          statusId: 0,
          extension: fileExtension
        };
        this.documents.push(documentObj);
        this.uploadedMedia.push({
          FileName: file.name,
          FileSize: this.mediaService.getFileSize(file.size) + ' ' + this.mediaService.getFileSizeUnit(file.size),
          FileType: file.type,
          FileUrl: fileUrl,
          FileProgessSize: 0,
          FileProgress: 0,
          ngUnsubscribe: new Subject<any>(),
        });
        this.startProgress(file, this.uploadedMedia.length - 1);
      };
    }
  }

  async startProgress(file: any, index: any) {
    let filteredFile = this.uploadedMedia
      .filter((u, index) => index === index)
      .pop();
    if (filteredFile != null) {
      let fileSize = this.mediaService.getFileSize(file.size);
      let fileSizeInWords = this.mediaService.getFileSizeUnit(file.size);
      if (this.mediaService.isApiSetup) {
        let formData = new FormData();
        formData.append('File', file);
        this.mediaService
          .uploadMedia(formData)
          .pipe(takeUntil(file.ngUnsubscribe))
          .subscribe(
            (res: any) => {
              if (res.status === 'progress') {
                let completedPercentage = parseFloat(res.message);
                filteredFile.FileProgessSize = `${(
                  (fileSize * completedPercentage) /
                  100
                ).toFixed(2)} ${fileSizeInWords}`;
                filteredFile.FileProgress = completedPercentage;
              } else if (res.status === 'completed') {
                filteredFile.Id = res.Id;
                filteredFile.FileProgessSize = fileSize + ' ' + fileSizeInWords;
                filteredFile.FileProgress = 100;
              }
            },
            (error: any) => {
              console.log('file upload error');
              console.log(error);
            }
          );
      } else {
        for (
          var f = 0;
          f < fileSize + fileSize * 0.0001;
          f += fileSize * 0.01
        ) {
          filteredFile.FileProgessSize = f.toFixed(2) + ' ' + fileSizeInWords;
          var percentUploaded = Math.round((f / fileSize) * 100);
          filteredFile.FileProgress = percentUploaded;
          await this.fakeWaiter(Math.floor(Math.random() * 35) + 1);
        }
      }
    }
  }

  fakeWaiter(ms: number) {
    return new Promise((resolve) => {
      setTimeout(resolve, ms);
    });
  }

  removeImage(idx: number) {
    this.uploadedMedia = this.uploadedMedia.filter((u, index) => index !== idx);
  }

  GetDocument(event: any, path: any, template: any) {
    this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(path + '#toolbar=0');
    this.dialogRef = this.dialog.open(template, {
      width: '70%',
      maxHeight: '90vh',
      disableClose: true,
    });
  }

  
  getaccountSearch(itemId: string) {
    return this.accountSearchList.find((option: { id: string; }) => option.id === itemId);
  }

  SearchAccount(index: any, template: any) {
    this.selectedIndexSearch =  index;
    this.dialogRef = this.dialog.open(template, {
      width: '30%',
      maxHeight: '90vh',
      disableClose: true,
    });
  }
  
  getcategoryList() {
    this.subcategoryList = [];
    this.accountTypeList = [];
    this.accountSearchList = [];

    const searchForm = this.accountSearchForm;
     searchForm.get('accountId')?.patchValue('');
     searchForm.get('account')?.patchValue(null);
     searchForm.get('accountNameSearch')?.patchValue('');
    let _CategoryFilter: any = {};
    this.categoryService.getAllAccountCategorys(_CategoryFilter).subscribe((data: any) => {
     this.categoryList = data.item1;
    });
  }

  getsubcategoryList() {
    this.accountTypeList = [];
    this.accountSearchList = [];
    const searchForm = this.accountSearchForm;
    searchForm.get('accountId')?.patchValue('');
    searchForm.get('account')?.patchValue(null);
    searchForm.get('accountNameSearch')?.patchValue('');
    var AccountCategoryId =  this.accountSearchForm.get('accountCategoryId')?.value;
    this.subcategoryService.getSubCategoryByCategory(AccountCategoryId).subscribe((data: any) => {
     this.subcategoryList = data;
    });
  }

  getAccountTypeList() {
    this.accountSearchList = [];
    const searchForm = this.accountSearchForm;
    searchForm.get('accountId')?.patchValue('');
    searchForm.get('account')?.patchValue(null);
    searchForm.get('accountNameSearch')?.patchValue('');
    var accountTypeId =  this.accountSearchForm.get('accountSubCategoryId')?.value;
    this.accounttypeService.getAccounttypeBySubCategory(accountTypeId).subscribe((data: any) => {
     this.accountTypeList = data;
    });
  }

  changeAccountTypeList() {
    this.accountSearchList = [];
    const searchForm = this.accountSearchForm;
    searchForm.get('accountId')?.patchValue('');
    searchForm.get('account')?.patchValue(null);
    searchForm.get('accountNameSearch')?.patchValue('');
  }

  getAccountSearchList(event: any) {
    var accountTypeId =  this.accountSearchForm.get('accountTypeId')?.value;
    var filter = event.currentTarget.value;
        // Clone the form value and add paging data
        const _AccountFilterForm = {
          accountTypeId: accountTypeId,
          name: filter,
          PagingData: null
        };

    this.accountService.getAllAccounts(_AccountFilterForm).subscribe((data: any) => {
     this.accountSearchList = data.item1;
    });
  }

  onOptionSelectedSearch(event: MatAutocompleteSelectedEvent, index: number): void {
    const selectedValue = event.option.value;
    const transactionDetailArray = this.transactionForm.get('transactionDetails') as FormArray;

    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');

      return;
    }

    // Get the FormArray for transactionDetails

    // Check if index is valid
    if (!transactionDetailArray || index < 0 || index >= transactionDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    // Check if the selected itemId already exists in the form array (excluding the current index)
    const duplicateItem = transactionDetailArray.controls
      .filter((control: AbstractControl, controlIndex: number) => controlIndex !== index) // Exclude the current index
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;  // Cast AbstractControl to FormGroup
        return formGroup.get('accountId')?.value === selectedValue.id;  // Check if the itemId already exists
      });

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = transactionDetailArray.at(index) as FormGroup;
      currentFormGroup.get('id')?.patchValue(0);
      currentFormGroup.get('transactionId')?.patchValue(0);
      currentFormGroup.get('accountId')?.patchValue(0);
      currentFormGroup.get('accountName')?.patchValue('');
      currentFormGroup.get('account')?.patchValue(null);
      currentFormGroup.get('quantity')?.patchValue(0);
      currentFormGroup.get('debitAmount')?.patchValue(0);
      currentFormGroup.get('creditAmount')?.patchValue(0);
      // This will clear all the values of the current form group
      return; // Do nothing if the itemId already exists
    }
    else {
      // Get the selected item details from your getaccount method
      const selectedItem = this.getaccountSearch(selectedValue.id);
      if (!selectedItem) {
        console.error('Selected item not found.');
        return;
      }

      // Get the form group for the current index
      const detailFormGroup = transactionDetailArray.at(index) as FormGroup;
      const searchForm = this.accountSearchForm;

      // Patch the values into the form group
       detailFormGroup.get('accountId')?.patchValue(selectedValue.id);
       detailFormGroup.get('accountName')?.patchValue(selectedValue.code + ' : ' + selectedValue.name);
       detailFormGroup.get('account')?.patchValue(selectedValue);
       searchForm.get('accountId')?.patchValue(selectedValue.id);
       searchForm.get('account')?.patchValue(selectedValue);
       searchForm.get('accountNameSearch')?.patchValue(selectedValue.code + ' : ' + selectedValue.name);
    }
  }
}
