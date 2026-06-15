import { Component, Inject } from '@angular/core';
import {  FormBuilder,FormGroup,Validators} from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { MediaService } from '../../../Service/media.service';
import { Subject, takeUntil } from 'rxjs';
import { AdmissionService } from '../admission/admission.service';

@Component({
  selector: 'app-discharge',
  templateUrl: './discharge.component.html',
  styleUrl: './discharge.component.css',
  standalone: false,
})
export class AddDischargeComponent {
  dischargeForm!: FormGroup;
  accountSearchForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  indentTypeList: any;
  storeList: any;
  dischargeTypeList: any;
  accountList: any[] = [];
  accountGroupList: any[] = [];
  isdataload: boolean = false;
  departmentList: any;
  projectList: any;
  tDebit = 0;
  tCredit = 0;
  urlSafe: SafeResourceUrl | undefined;
  dialogRef: any;
  uploadedMedia: Array<any> = [];
  documents: any[] = [];
  categoryList: any;
  subcategoryList: any;
  accountTypeList: any;
  accountSearchList: any;
  selectedIndexSearch: number = 0;

  constructor(
    private dialog: MatDialog,
    private notificationsService: NotificationsService,
    private formBuilder: FormBuilder,
    private admissionService: AdmissionService,
    private mediaService: MediaService,
    public sanitizer: DomSanitizer,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

  ngOnInit(): void {
    this.dischargeForm = this.formBuilder.group({
      admissionId: [this.data.element.id],
      dischargeDate: [new Date(), Validators.required],
      dischargeSummary: ['', Validators.required], // Validation
      files : []
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

    if (this.tDebit != this.tCredit) {
      this.notificationsService.showNotification(
        'Debit is not equal to credit.',
        'snack-bar-danger'
      );
      return;
    }

    this.isLoading = true;
    let _dischargeForm: any = {};
    _dischargeForm = Object.assign(
      _dischargeForm,
      this.dischargeForm.value
    );
    let date = new Date(this.dischargeForm.get('date')?.value);
    _dischargeForm['date'] = date.toLocaleDateString();
    _dischargeForm['files'] = this.documents;

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
          extension: fileExtension,
        };
        this.documents.push(documentObj);
        this.uploadedMedia.push({
          FileName: file.name,
          FileSize:
            this.mediaService.getFileSize(file.size) +
            ' ' +
            this.mediaService.getFileSizeUnit(file.size),
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
    this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(
      path + '#toolbar=0'
    );
    this.dialogRef = this.dialog.open(template, {
      width: '70%',
      maxHeight: '90vh',
      disableClose: true,
    });
  }

}
