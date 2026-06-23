import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ProjectService } from '../../../project/project.service';
import { ServiceAccountService } from '../serviceaccount.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { AccountService } from '../../../account/account.service';
import { ServiceService } from '../../service/service.service';
import { DepartmentService } from '../../../department/department.service';
import { ServiceTypeService } from '../../service-type/service-type.service';

// ✅ Custom validator: both accounts must be filled OR both empty
export function validateAccounts(group: AbstractControl): ValidationErrors | null {
  const debit = group.get('debitAccountId')?.value;
  const credit = group.get('creditAccountId')?.value;
  const hasDebit = debit && debit > 0;
  const hasCredit = credit && credit > 0;
  if (hasDebit && !hasCredit) {
    return { missingCredit: true };
  }
  if (!hasDebit && hasCredit) {
    return { missingDebit: true };
  }
  return null; // both filled or both empty → valid
}

@Component({
  selector: 'app-add-serviceaccount',
  templateUrl: './add-serviceaccount.component.html',
  styleUrls: ['./add-serviceaccount.component.css'],
  standalone: false
})
export class AddServiceAccountComponent implements OnInit {

  form!: FormGroup;
  isLoading = false;
  isEdit = false;

  projects: any[] = [];
  accountList: any[] = [];
  serviceInfo: any;
  departments: any[] = [];
  serviceTypes: any[] = [];

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private projectService: ProjectService,
    private serviceAccountService: ServiceAccountService,
    private accountService: AccountService,
    private notifications: NotificationsService,
    private constantService: ConstantService,
    private serviceService: ServiceService,
    private departmentService: DepartmentService,
    private serviceTypeService: ServiceTypeService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      serviceId: [this.data.element.id, Validators.required],
      projects: this.fb.array([])
    });

    this.loadDepartments();
    this.loadProjects();
    this.serviceInfo = this.data.element;
  }

  loadDepartments(): void {
    this.departmentService.getClinicalDepartment().subscribe({
      next: (res: any) => {
        this.departments = res?.item1 ?? res ?? [];
      },
      error: () => {
        this.departments = [];
      }
    });
  }

  // ================= PROJECT FORM ARRAY =================
  get projectsFA(): FormArray {
    return this.form?.get('projects') as FormArray || this.fb.array([]);
  }

  getServiceAccounts(projectIndex: number): FormArray {
    const project = this.projectsFA.at(projectIndex);
    return project ? project.get('serviceAccounts') as FormArray : this.fb.array([]);
  }

  // ================= LOAD DATA =================
  loadData(element: any): void {
    this.isEdit = true;
    this.constantService.LoadData(element, this.form);
    const serviceAccounts = element?.serviceAccounts || [];
    this.initProjects(this.projects, serviceAccounts);
  }

  // ================= LOAD PROJECTS =================
  loadProjects(): void {
    this.projectService.getAllProjects({}).subscribe({
      next: (res: any) => {
        this.projects = res?.item1 ?? res ?? [];
        if (this.data?.element.serviceAccounts) {
          this.loadData(this.data.element);
        } else {
          this.initProjects(this.projects, []);
        }
      }
    });
  }

  // ================= INIT PROJECTS =================
  initProjects(projects: any[], existing: any[]): void {
    this.projectsFA.clear();
    projects.forEach(p => {
      const projectRows = existing.filter(x => Number(x.projectId) === Number(p.id));
      this.projectsFA.push(this.createProjectGroup(p, projectRows));
    });
  }

  // ================= PROJECT GROUP =================
  createProjectGroup(project: any, existingRows: any[]): FormGroup {
    return this.fb.group({
      projectId: [project.id],
      projectName: [project.name],
      serviceAccounts: this.fb.array([
        this.createRow(project, 1, existingRows.find(x => x.accountType === 1)), // Payable
        this.createRow(project, 2, existingRows.find(x => x.accountType === 2))  // Discount
      ])
    });
  }

  // ================= ROW (PROJECT + TYPE) =================
  createRow(project: any, type: number, item: any): FormGroup {
    const isRequired = type === 1;

    let debitName = '';
    let creditName = '';
    if (item) {
      debitName = item.debitAccount ? `${item.debitAccount.code} : ${item.debitAccount.name}` : '';
      creditName = item.creditAccount ? `${item.creditAccount.code} : ${item.creditAccount.name}` : '';
    }

    return this.fb.group({
      id: [item?.id || 0],
      projectId: [project.id],
      accountType: [type],
      debitAccountId: [
        item?.debitAccountId || 0,
        isRequired ? Validators.required : null
      ],
      debitAccountName: [debitName],
      creditAccountId: [
        item?.creditAccountId || 0,
        isRequired ? Validators.required : null
      ],
      creditAccountName: [creditName]
    }, { validators: validateAccounts }); // ✅ Cross-field validation
  }

  // ================= ACCOUNT SEARCH =================
  getAccountList(event: any): void {
    const filter = event.target.value;
    this.accountService.getAccountByName(filter, [''])
      .subscribe((data: any) => {
        this.accountList = data;
      });
  }

  // ================= SELECT DEBIT =================
  onDebitSelected(event: any, pIndex: number, rIndex: number): void {
    const acc = event.option.value;
    this.getServiceAccounts(pIndex).at(rIndex).patchValue({
      debitAccountId: acc.id,
      debitAccountName: `${acc.code} : ${acc.name}`
    });
    // Trigger validation on the group
    this.getServiceAccounts(pIndex).at(rIndex).updateValueAndValidity();
  }

  // ================= SELECT CREDIT =================
  onCreditSelected(event: any, pIndex: number, rIndex: number): void {
    const acc = event.option.value;
    this.getServiceAccounts(pIndex).at(rIndex).patchValue({
      creditAccountId: acc.id,
      creditAccountName: `${acc.code} : ${acc.name}`
    });
    this.getServiceAccounts(pIndex).at(rIndex).updateValueAndValidity();
  }

  // ================= CLEAR ACCOUNT ON EMPTY INPUT =================
  onAccountInput(event: any, pIndex: number, rIndex: number, type: 'debit' | 'credit'): void {
    const inputValue = event.target.value;
    if (!inputValue || inputValue.trim() === '') {
      const row = this.getServiceAccounts(pIndex).at(rIndex);
      if (type === 'debit') {
        row.patchValue({ debitAccountId: 0, debitAccountName: '' });
      } else {
        row.patchValue({ creditAccountId: 0, creditAccountName: '' });
      }
      row.updateValueAndValidity();
    }
  }

  // ================= SAVE =================
  save(): void {
    if (this.form.invalid) {
      // Optionally scroll to first error or show a general message
      this.notifications.showNotification('Please fix validation errors.', 'snack-bar-warning');
      return;
    }

    this.isLoading = true;

    const projects = this.form.value.projects;
    const serviceAccounts = projects.flatMap((p: any) =>
      p.serviceAccounts.filter((sa: any) =>
        sa.creditAccountId > 0 && sa.creditAccountId !== null &&
        sa.debitAccountId > 0 && sa.debitAccountId !== null
      )
    );

    const command = {
      serviceId: this.form.value.serviceId,
      serviceAccounts: serviceAccounts
    };

    this.serviceAccountService.saveServiceAccount(command)
      .subscribe({
        next: (res: any) => {
          this.isLoading = false;
          if (res.Status === 200) {
            this.notifications.showNotification('Saved Successfully', 'snack-bar-success');
            this.dialog.closeAll();
          } else {
            this.notifications.showNotification(res.Message || 'Error', 'snack-bar-danger');
          }
        },
        error: () => {
          this.isLoading = false;
          this.notifications.showNotification('Server Error', 'snack-bar-danger');
        }
      });
  }
}