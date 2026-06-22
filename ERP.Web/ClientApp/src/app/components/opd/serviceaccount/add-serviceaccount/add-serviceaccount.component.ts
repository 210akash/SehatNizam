import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';

import { ProjectService } from '../../../project/project.service';
import { ServiceAccountService } from '../serviceaccount.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { AccountService } from '../../../account/account.service';
import { ServiceService } from '../../service/service.service';
import { DepartmentService } from '../../../department/department.service';
import { ServiceTypeService } from '../../service-type/service-type.service';

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
      code: [{ value: '', disabled: true }],
      name: [{ value: '', disabled: true }],
      basePrice: [{ value: 0, disabled: true }],
      departmentId: [''],
      serviceTypeId: [''],
      projects: this.fb.array([])
    });

    this.loadServiceInfo();
    this.loadDepartments();
    this.loadServiceTypes();
    this.loadProjects();
  }

  loadServiceInfo(): void {
    if (this.data?.element?.code) {
      this.serviceInfo = this.data.element;
      this.form.patchValue({
        code: this.data.element.code,
        name: this.data.element.name,
        basePrice: this.data.element.basePrice,
        departmentId: this.data.element.departmentId,
        serviceTypeId: this.data.element.serviceTypeId
      });
    } else if (this.data?.element?.id) {
      this.serviceService.getServiceById(this.data.element.id).subscribe({
        next: (res: any) => {
          this.serviceInfo = res?.item1 ?? res ?? {};
          this.form.patchValue({
            code: this.serviceInfo.code,
            name: this.serviceInfo.name,
            basePrice: this.serviceInfo.basePrice,
            departmentId: this.serviceInfo.departmentId,
            serviceTypeId: this.serviceInfo.serviceTypeId
          });
        }
      });
    }
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

  loadServiceTypes(): void {
    this.serviceTypeService.getAllServiceTypes({}).subscribe({
      next: (res: any) => {
        this.serviceTypes = res?.item1 ?? res ?? [];
      },
      error: () => {
        this.serviceTypes = [];
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

      // 🔥 AFTER projects load → THEN load form data
      if (this.data?.element) {
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

      const projectRows = existing.filter(x => x.projectId === p.id);

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

    const isRequired = type === 1; // Payable

    return this.fb.group({
      id: [item?.id || 0],

      projectId: [project.id],
      serviceTypeId: [this.form.value.serviceTypeId || this.form.get('serviceTypeId')?.value || 0],

      accountType: [type],

      debitAccountId: [
        item?.debitAccountId || 0,
        isRequired ? Validators.required : null
      ],
      debitAccountName: [item?.debitAccountName || ''],

      creditAccountId: [
        item?.creditAccountId || 0,
        isRequired ? Validators.required : null
      ],
      creditAccountName: [item?.creditAccountName || '']
    });
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
  }

  // ================= SELECT CREDIT =================
  onCreditSelected(event: any, pIndex: number, rIndex: number): void {

    const acc = event.option.value;

    this.getServiceAccounts(pIndex).at(rIndex).patchValue({
      creditAccountId: acc.id,
      creditAccountName: `${acc.code} : ${acc.name}`
    });
  }

  // ================= SAVE =================
save(): void {

    if (this.form.invalid) return;

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
            this.notifications.showNotification(
              'Saved Successfully',
              'snack-bar-success'
            );
            this.dialog.closeAll();
          } else {
            this.notifications.showNotification(
              res.Message || 'Error',
              'snack-bar-danger'
            );
          }
        },
        error: () => {
          this.isLoading = false;
          this.notifications.showNotification(
            'Server Error',
            'snack-bar-danger'
          );
        }
      });
  }
}