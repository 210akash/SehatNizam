import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { InterviewService } from '../interview.service';
import { DepartmentService } from '../../department/department.service';
import { CompanyService } from '../../company/company.service';
import { EmployeeDesignationService } from '../../hr/employee-designation/employee-designation.service';
import { EmployeeEducationService } from '../../hr/employee-education/employee-education.service';
import { MediaService } from '../../../Service/media.service';
import { UserService } from '../../user-management/user.service';
import { AuthenticationService } from '../../../Auth/authentication.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-add-interview',
  templateUrl: './add-interview.component.html',
  styleUrl: './add-interview.component.css',
  standalone: false
})

export class AddInterviewComponent {
  currentUser: any;
  interviewForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  companyList: any;
  departmentList: any;
  employeeDesignationList: any;
  employeeEducationList: any;
  interviewAttendeesList: any;

  documents: any[] = [];

  urlSafe: SafeResourceUrl | undefined;
  dialogRef: any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private mediaService: MediaService,
    private formBuilder: FormBuilder, private companyService: CompanyService, private departmentService: DepartmentService,
    private authenticationService: AuthenticationService, private employeeDesignationService: EmployeeDesignationService, private sanitizer: DomSanitizer,
    private employeeEducationService: EmployeeEducationService, private interviewService: InterviewService, private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.currentUser = this.authenticationService.currentUserValue;
    console.log('this.currentUser', this.currentUser);
    this.interviewForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      mobile: ['', Validators.required],
      email: ['', Validators.required],
      employeeEducationId: ['', Validators.required],
      departmentId: ['', Validators.required],
      employeeDesignationId: ['', Validators.required],
      currentSalary: ['', Validators.required],
      expectedSalary: ['', Validators.required],
      experience: ['', Validators.required],
      reference: ['', Validators.required],
      personalDetail: ['', Validators.required],
      reason: ['', Validators.required],
     remarks: ['', [Validators.required, Validators.maxLength(1000)]],
      // joinAfterDays: ['', Validators.required],
      // interviewDate: ['', Validators.required],
      joinDate: ['', Validators.required],
      companyId: [this.currentUser.department.companyId, Validators.required],
      // interviewAttendees: ['', Validators.required],
    });

    this.getEmployeeEducationList();
    this.getEmployeeDesignationList();
    // this.getCompanyList();
    this.getInterviewAttendees();
    this.getDepartmentByCompany();
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.interviewForm);
      var joinDate = this.constantService.formatDate(element.joinDate);
      this.interviewForm.get('joinDate')?.patchValue(joinDate);
      this.documents = element.attachments;
    }
    else{
    this.getCode();
    }
  }

  SaveData() {
    if (this.interviewForm.invalid) {
      this.constantService.markFormGroupTouched(this.interviewForm);
      return;
    }

    this.isLoading = true;
    let _interviewForm: any = {};
    _interviewForm = Object.assign(_interviewForm, this.interviewForm.value);
    _interviewForm['fileCommand'] = this.documents;

    this.interviewService.saveInterview(_interviewForm).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }

  // getCompanyList(): void {
  //   let _companyForm: any = {};
  //   this.companyService.getAllCompanys(_companyForm).subscribe(data => {
  //     this.companyList = data.item1;
  //   });
  // }

  getDepartmentByCompany(): void {
    this.departmentService.getDepartmentByCompany(this.currentUser.department.companyId).subscribe(data => {
      this.departmentList = data;
    });
  }

  getEmployeeDesignationList(): void {
    let _filterForm = {};
    this.employeeDesignationService.getAllEmployeeDesignations(_filterForm).subscribe(data => {
      this.employeeDesignationList = data.item1;
    });
  }

  getEmployeeEducationList(): void {
    let _filterForm = {};
    this.employeeEducationService.getAllEmployeeEducations(_filterForm).subscribe(data => {
      this.employeeEducationList = data.item1;
    });
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
        const imageName = event.target.result;
        let documentObj = {
          id: 0,
          filePath: imageName,
          fileName: fileName,
          statusId: 0,
          extension: fileExtension
        };
        this.documents.push(documentObj);

        // this.documents.push({
        //   FileName: file.name,
        //   FileSize: this.mediaService.getFileSize(file.size) + ' ' + this.mediaService.getFileSizeUnit(file.size),
        //   FileType: file.type,
        //   ImageName: imageName,
        //   FileProgessSize: 0,
        //   FileProgress: 0,
        //   ngUnsubscribe: new Subject<any>(),
        // });
        // this.startProgress(file, this.documents.length - 1);
      };
    }
  }

  // async startProgress(file: any, index: any) {
  //   let filteredFile = this.documents
  //     .filter((u, index) => index === index)
  //     .pop();
  //   if (filteredFile != null) {
  //     let fileSize = this.mediaService.getFileSize(file.size);
  //     let fileSizeInWords = this.mediaService.getFileSizeUnit(file.size);
  //     if (this.mediaService.isApiSetup) {
  //       let formData = new FormData();
  //       formData.append('File', file);
  //       this.mediaService
  //         .uploadMedia(formData)
  //         .pipe(takeUntil(file.ngUnsubscribe))
  //         .subscribe(
  //           (res: any) => {
  //             if (res.status === 'progress') {
  //               let completedPercentage = parseFloat(res.message);
  //               filteredFile.FileProgessSize = `${(
  //                 (fileSize * completedPercentage) /
  //                 100
  //               ).toFixed(2)} ${fileSizeInWords}`;
  //               filteredFile.FileProgress = completedPercentage;
  //             } else if (res.status === 'completed') {
  //               filteredFile.Id = res.Id;
  //               filteredFile.FileProgessSize = fileSize + ' ' + fileSizeInWords;
  //               filteredFile.FileProgress = 100;
  //             }
  //           },
  //           (error: any) => {
  //             console.log('file upload error');
  //             console.log(error);
  //           }
  //         );
  //     } else {
  //       for (
  //         var f = 0;
  //         f < fileSize + fileSize * 0.0001;
  //         f += fileSize * 0.01
  //       ) {
  //         filteredFile.FileProgessSize = f.toFixed(2) + ' ' + fileSizeInWords;
  //         var percentUploaded = Math.round((f / fileSize) * 100);
  //         filteredFile.FileProgress = percentUploaded;
  //         await this.fakeWaiter(Math.floor(Math.random() * 35) + 1);
  //       }
  //     }
  //   }
  // }

  // fakeWaiter(ms: number) {
  //   return new Promise((resolve) => {
  //     setTimeout(resolve, ms);
  //   });
  // }

  removeImage(index: number): void {
    this.documents.splice(index, 1);
  }

  getInterviewAttendees(): void {
    this.interviewService.getInterviewAttendees().subscribe(data => {
      this.interviewAttendeesList = data;
    });
  }

  getCode() {
    this.interviewService.getCode().subscribe((data: any) => {
      this.interviewForm.get('code')?.patchValue(data.code);
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

  isImage(filePath: string): boolean {
    if (!filePath) return false;
    const ext = filePath.split('.').pop()?.toLowerCase();
    return ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg'].includes(ext || '');
  }

  isPdf(filePath: string): boolean {
    return filePath?.toLowerCase().endsWith('.pdf');
  }


}