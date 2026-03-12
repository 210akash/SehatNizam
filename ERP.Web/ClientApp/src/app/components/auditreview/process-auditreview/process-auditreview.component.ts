import { Component, Inject, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService, OrderStatusEnum } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { AuditReviewService } from '../auditreview.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { AccountService } from '../../account/account.service';
import { PrimaryOrderService } from '../../order/primary-order/order.service';
import { LedgerService } from '../../ledger/ledger.service';
import { ReportViewerComponent } from '../../report/report-viewer.component';
import { AuthenticationService } from '../../../Auth/authentication.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-process-auditreview',
  templateUrl: './process-auditreview.component.html',
  styleUrl: './process-auditreview.component.css',
  standalone: false
})

export class ProcessAuditReviewComponent {
  accountReviewForm!: FormGroup;
  iGPForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  documents: any;
  urlSafe: SafeResourceUrl | undefined;
  dialogRef: any;
  totalQuantity = 0;
  billingAmount = 0;
  currentBalance = 0;
  isFieldsVisible: boolean = false; // Initially hidden
  bankAccountList: any;
  statusEnum: any;
  currentUser: any;
  accountGroupId: any;
  accountGroupCode: any;
  revokeToInprocessComments: any;
  reportsUrl: any;
  @ViewChild('confirmationDialog') confirmationDialog!: TemplateRef<any>;
  constructor(private authenticationService: AuthenticationService, private ledgerService: LedgerService, private orderService: PrimaryOrderService, private accountService: AccountService, private formBuilder: FormBuilder, private sanitizer: DomSanitizer, private dialog: MatDialog, private notificationsService: NotificationsService, private auditreviewService: AuditReviewService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) {
    this.reportsUrl = environment.reports_uri;
  }

  ngOnInit(): void {
    this.statusEnum = OrderStatusEnum;
    this.currentUser = this.authenticationService.currentUserValue;
    this.accountReviewForm = this.formBuilder.group({
      orderId: [0],
      bank: [''],
      transactionId: [''],
      isTransactionLedgerEntry: [false],
      description: [''],
      amount: [0],
    });
    this.getOrderById();

    this.accountReviewForm.patchValue({ orderId: this.data.element.id });
    this.accountGroupId = this.data.element.dealership.accountGroup[0].id;
    this.accountGroupCode = this.data.element.dealership.accountGroup[0].code;
    this.getaccountList();
  }

  async customerCurrentBalance() {
    (await this.ledgerService.customerCurrentBalance(this.data.element.dealershipId)).subscribe({
      next: (data: any) => {
        this.currentBalance = data;
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getOrderById() {
    (await this.orderService.getOrderById(this.data.element.id)).subscribe({
      next: (data: any) => {
        this.data.element = data;
        this.documents = this.data.element?.orderAttachments.filter((x: { isActive: boolean; }) => x.isActive == true);
        this.data.element.orderItems.forEach((item: any) => {
          this.totalQuantity += item.quantity;
          this.billingAmount += item.quantity * item.distributorPrice;
          this.customerCurrentBalance();
        });
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  toggleFields(): void {
    this.isFieldsVisible = this.accountReviewForm.get('isTransactionLedgerEntry')?.value || false;

    if (this.isFieldsVisible) {
      // Add required validation when toggle is true
      this.accountReviewForm.get('bank')?.setValidators([Validators.required]);
      this.accountReviewForm.get('transactionId')?.setValidators([Validators.required]);
      this.accountReviewForm.get('amount')?.setValidators([Validators.required, Validators.min(1)]);
    } else {
      // Remove required validation when toggle is false
      this.accountReviewForm.get('bank')?.clearValidators();
      this.accountReviewForm.get('transactionId')?.clearValidators();
      this.accountReviewForm.get('amount')?.clearValidators();
    }

    // Update the validity state of the form controls
    this.accountReviewForm.get('bank')?.updateValueAndValidity();
    this.accountReviewForm.get('transactionId')?.updateValueAndValidity();
    this.accountReviewForm.get('amount')?.updateValueAndValidity();
  }
  async RevokeToSale() {

    let _updateOrderStatus = {
      orderId: this.data.element.id,
      fromStatusId: this.data.element.orderStatusId,
      toStatusId: this.statusEnum.Create,
      comments: this.accountReviewForm.get('description')?.value
    };
    (await this.orderService.updateOrderStatus(_updateOrderStatus)).subscribe({
      next: (data: { Status: number; Message: any; }) => {
        if (data.Status == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification('Order revoke (Back to Sale) Successfully', 'snack-bar-success');
          this.dialog.closeAll();
        }
        else {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
  async SaveData() {
    if (this.accountReviewForm.invalid) {
      this.constantService.markFormGroupTouched(this.accountReviewForm);
      return;
    }

    this.isLoading = true;
    let _accountReviewForm: any = {};
    _accountReviewForm = Object.assign(_accountReviewForm, this.accountReviewForm.value);

    this.auditreviewService.saveAuditReview(_accountReviewForm).subscribe({
      next: (data: { Status: number; Data: string; }) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
          this.isLoading = false;
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

  preventDecimal(event: KeyboardEvent): void {
    if (event.key === '.' || event.key === ',') {
      event.preventDefault(); // Prevent decimal input
    }
  }

  async process() {
    (await this.auditreviewService.processAuditReview(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
      },
      error: (error) => {
        console.log(error);
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }

  GetDocument(event: any, index: number, template: any) {
    this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(this.documents[index].fileSource + '#toolbar=0');
    this.dialogRef = this.dialog.open(template, {
      width: '50%',
      height: '70%',
      disableClose: true,
    });
  }

  getaccountList() {
    let _AccountFilter: any = {};
    _AccountFilter.accountFlowId = 2; //For Bank
    this.accountService.getAccountByAccountFlow(_AccountFilter).subscribe((data: any) => {
      this.bankAccountList = data;
    });
  }

  getFilteredOrderComments(orderStatusId: number): string | null {
    const filteredProcesses = this.data.element.orderProcess.filter(
      (process: any) => process.toStatusId === orderStatusId
    );

    return filteredProcesses.length > 0
      ? filteredProcesses[filteredProcesses.length - 1].comments
      : null; // Return null if no matching process found
  }

  openReportDialog(reportName: string, parameters: any) {
    this.dialog.open(ReportViewerComponent, {
      width: '80%',  // You can customize the dialog width as needed
      data: {
        reportName: reportName,
        parameters: parameters
      }
    });
  }

  async revokeToInprocess() {
    this.revokeToInprocessComments = ''; // Reset comments before opening the dialog

    const dialogRef = this.dialog.open(this.confirmationDialog);

    // ✅ Only call afterClosed() once
    const comments = await dialogRef.afterClosed().toPromise();

    // Cancelled (e.g., user clicked "Cancel")
    if (comments === false) {
      console.log('Revoke canceled');
      return;
    }

    // Validation: Empty comments
    if (!comments || comments.trim() === '') {
      this.notificationsService.showNotification('Please enter comments for revoking reason', 'snack-bar-danger');
      return;
    }

    // Proceed with the update
    this.isLoading = true;

    const _updateOrderStatus = {
      orderId: this.data.element.id,
      fromStatusId: this.data.element.orderStatusId,
      toStatusId: this.statusEnum.InProcess,
      comments: comments
    };

    (await this.orderService.updateOrderStatus(_updateOrderStatus)).subscribe({
      next: (data: { Status: number; Message: any; }) => {
        this.isLoading = false;
        if (data.Status === 200) {
          this.notificationsService.showNotification('Order revoke (To Inprocess) Successfully', 'snack-bar-success');
          this.dialog.closeAll();
        } else {
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async revokeToAccountReviewed() {
    let _updateOrderStatus = {
      orderId: this.data.element.id,
      fromStatusId: this.data.element.orderStatusId,
      toStatusId: this.statusEnum.AccountReviewed,
      comments: this.accountReviewForm.get('description')?.value
    };
    (await this.orderService.updateOrderStatus(_updateOrderStatus)).subscribe({
      next: (data: { Status: number; Message: any; }) => {
        if (data.Status == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification('Order revoke (Back to Sale) Successfully', 'snack-bar-success');
          this.dialog.closeAll();
        }
        else {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  redirectToaccountledger() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FERPReports%2FAccountLedger&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId=' + this.currentUser.department.companyId + '&Account=' + this.accountGroupCode;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }
}
