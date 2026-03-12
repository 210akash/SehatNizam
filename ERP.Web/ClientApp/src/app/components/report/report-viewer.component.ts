import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { DatePipe } from '@angular/common';  // Import DatePipe
import { AccountService } from '../account/account.service';
import { AccountGroupService } from '../accountgroup/accountgroup.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { ConstantService } from '../../Service/constant.service';
import { AuthenticationService } from '../../Auth/authentication.service';
import { environment } from '../../../environments/environment';
@Component({
  selector: 'app-report-viewer',
  templateUrl: './report-viewer.component.html',
  styleUrls: ['./report-viewer.component.css'],
  standalone: false,  // Ensure it's standalone
  providers: [DatePipe]  // Provide DatePipe
})
export class ReportViewerComponent {

  reportUrlSafe: SafeResourceUrl = '';
  TransactionFilterForm!: FormGroup;
  accountList : any;
  currentUser: any;
  reportServerUrl : any;
  constructor(
    private sanitizer: DomSanitizer, private formBuilder: FormBuilder,private authenticationService :AuthenticationService, private constantService: ConstantService,private datePipe: DatePipe,  // Inject DatePipe
    private accountService: AccountService, 
    private accountgroupService: AccountGroupService, 
  ) {this.reportServerUrl = environment.reports_uri;}

  ngOnInit(): void {
    this.currentUser = this.authenticationService.currentUserValue;
    this.TransactionFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      accountName: [''],
      account: [''],
      companyId: [this.currentUser.department.companyId],
      isGroup: [false]
    });

    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const endDate = new Date(currentYear, 11, 31);
    this.TransactionFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.TransactionFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
  }


  
  async filterData() {
 
    // Get the form values dynamically
  const reportParameters = {
    // Format the dates to 'MAR-YYYY'
    FromDate: this.datePipe.transform(this.TransactionFilterForm.get('fdate')?.value, 'MM-dd-yyyy'),
    ToDate: this.datePipe.transform(this.TransactionFilterForm.get('tdate')?.value, 'MM-dd-yyyy'),
    Account: this.TransactionFilterForm.get('account')?.value,
    CompanyId: this.TransactionFilterForm.get('companyId')?.value,
  };

    // Constructing the URL with parameters
    let url = `${this.reportServerUrl}ReportServer/Pages/ReportViewer.aspx?/ERPReports/AccountLedgertest&rs:Command=Render&rc:Zoom=Page+Width&rs:Embed=true`;

    // Add additional parameters dynamically to the URL
    for (const [key, value] of Object.entries(reportParameters)) {
      if (value) {
        url += `&${key}=${encodeURIComponent(value)}`;
      }
    }
    // Sanitize the URL for safe embedding in iframe
    this.reportUrlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }
  
  getAccountList(event: any) {
    var filter = event.currentTarget.value;
    this.accountList = [];  // Empty the list before updating
    if(!this.TransactionFilterForm.get('isGroup')?.value)
{
  var accountFlow = [''];
    
  this.accountService.getAccountByName(filter, accountFlow)
      .subscribe((data: any) => {
          this.accountList = data;
      });
}
else{
  // Clone the form value and add paging data
  const filter = event.currentTarget.value;
  const accountFlow: string[] = [];
  this.accountList = [];  // Empty the list before updating
  this.accountgroupService.getAccountGroupByName(filter, accountFlow).subscribe(
    (data: any) => {
      this.accountList = data || []; // Ensure it's an array even if no data is returned
    },
    (error) => {
      console.error('Error fetching account list:', error);
      this.accountList = [];  // Reset in case of an error
    }
  );
}
}


onInputCleared(event: Event): void {
  const inputValue = (event.target as HTMLInputElement)?.value;
  console.log('Current Input Value:', inputValue); // Debugging output

  if (!inputValue.trim()) {
   // console.log(`Input cleared at row index: ${index}`);
  }
}

  onOptionSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    this.TransactionFilterForm.get('account')?.patchValue(selectedValue.code);
    this.TransactionFilterForm.get('accountName')?.patchValue(selectedValue.code + ' : ' + selectedValue.name);
  }

        showOptions(event:MatCheckboxChange): void {
          this.accountList = [];
          this.TransactionFilterForm.get('accountName')?.patchValue('');
          }

}
