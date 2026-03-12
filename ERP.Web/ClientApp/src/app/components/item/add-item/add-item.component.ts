import { Component, Inject } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { ItemService } from '../item.service';
import { CategoryService } from '../../category/category.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ItemtypeService } from '../../itemtype/itemtype.service';
import { UomService } from '../../uom/uom.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { MediaService } from '../../../Service/media.service';
import { Subject, takeUntil } from 'rxjs';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';

@Component({
  selector: 'app-add-item',
  templateUrl: './add-item.component.html',
  styleUrl: './add-item.component.css',
  standalone: false
})

export class AddItemComponent {
  itemForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  categoryList: any;
  subcategoryList: any;
  itemTypeList: any;
  UomList: any;

  dialogRef: any;
  imageData: any;
  urlSafe: SafeResourceUrl | undefined;

  logos: string[] = [];
  uploadedMedia: Array<any> = [];
  documents: any;
  imageSrc: any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private itemService: ItemService,
    private subcategoryService: SubcategoryService, private itemtypeService: ItemtypeService, private uomService: UomService, private categoryService: CategoryService,
    private constantService: ConstantService, private mediaService: MediaService, private sanitizer: DomSanitizer, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.itemForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      categoryId: ['', Validators.required],
      subCategoryId: ['', Validators.required],
      itemTypeId: ['', Validators.required],
      uomId: ['', Validators.required],
      recordLevel: [0, Validators.required],
      leadTime: [0, Validators.required],
      rate: [0, Validators.required],
      weight: [0, Validators.required],
      length: [0, Validators.required],
      height: [0, Validators.required],
      width: [0, Validators.required],
      model: [0, Validators.required],
      make: [0, Validators.required],
      excessQtyPer: [0, Validators.required],
      openingQty: [0, Validators.required],
      companyId: [0],
      image: [''],
      quantityInPack: [0],
      isGroupItem: [false],
      itemGroup: this.formBuilder.array([]) // Initialize as a FormArray
    });

    this.LoadData(this.data.element);
    this.getcategoryList();
    this.getuomList();
  }

  get itemGroup(): FormArray {
    return this.itemForm.get('itemGroup') as FormArray;
  }

  addGroup(index: number) {
    const newGroup = this.formBuilder.group({
      id: [0], // Default value
      itemId: [0],
      name: ['', Validators.required],
      description: ['', Validators.required],
    });

    // Insert the new group after the current index
    this.itemGroup.insert(index + 1, newGroup);
  }

  removeGroup(index: number) {
    if (this.itemGroup.length > 1) {
      this.itemGroup.removeAt(index);
    } else {
      this.notificationsService.showNotification('At least one Item Group is required.', 'snack-bar-danger');
    }
  }


  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.itemForm);
      this.itemForm.get('categoryId')?.patchValue(element.itemType.subCategory.category.id);
      this.itemForm.get('subCategoryId')?.patchValue(element.itemType.subCategory.id);

      // Populate the transactionDetails FormArray
      const groupArray = this.itemForm.get('itemGroup') as FormArray;
      groupArray.clear(); // Clear existing data

      if (element.itemGroup && element.itemGroup.length > 0 && element.isGroupItem) {
        var i = 0;
        element.itemGroup.forEach((detail: any) => {
          // Conditional validation for accountId and accountGroupId
          const detailGroup = this.formBuilder.group({
            id: [detail.id, Validators.required],
            itemId: [detail.itemId, Validators.required],
            name: [detail.name, Validators.required],
            description: [detail.description, Validators.required],
          });
          groupArray.push(detailGroup);
          i++;
        });
      }

      // let documentObj = {
      //   'fileSource': element.image,
      //   'imageName': element.image,
      //   'extension': ''
      // }

       this.imageSrc = element.image;
      this.getsubcategoryList();
      this.getItemTypeList();
    }
    // else   
    //  this.getItemCode();
  }

  SaveData() {
    console.log(this.itemForm);
    if (this.itemForm.invalid) {
      this.constantService.markFormGroupTouched(this.itemForm);
      return;
    }

    this.isLoading = true;
    let _addItemForm: any = {};
    _addItemForm = Object.assign(_addItemForm, this.itemForm.value);

    _addItemForm['productImage'] = this.imageData;

    this.itemService.saveItem(_addItemForm).subscribe({
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

  getcategoryList() {
    let _CategoryFilter: any = {};
    this.categoryService.getAllCategorys(_CategoryFilter).subscribe((data: any) => {
      this.categoryList = data.item1;
    });
  }

  getsubcategoryList() {
    var CategoryId = this.itemForm.get('categoryId')?.value;
    this.subcategoryService.getSubCategoryByCategory(CategoryId).subscribe((data: any) => {
      this.subcategoryList = data;
    });
  }

  getItemTypeList() {
    var subCategoryId = this.itemForm.get('subCategoryId')?.value;
    this.itemtypeService.getItemtypeBySubCategory(subCategoryId).subscribe((data: any) => {
      this.itemTypeList = data;
    });
  }

  getuomList() {
    this.uomService.GetUOMByCompany(0).subscribe((data: any) => {
      this.UomList = data;
    });
  }

  getItemCode() {
    var ItemTypeId = this.itemForm.get('itemTypeId')?.value;
    var Id = this.itemForm.get('id')?.value;
    this.itemService.getItemCode(ItemTypeId, Id).subscribe((data: any) => {
      this.itemForm.get('code')?.patchValue(data.code);
    });
  }

  reset() {
    this.itemForm.get('code')?.patchValue('');
    this.itemTypeList = [];
  }

  onDocumentSourceChange(event: any) {
    if (event.target.files.length > 0) {
      const selectedFiles = event.target.files;
      for (let file of selectedFiles) {
        let fileName = file.name;
        let fileExtension = fileName.split('.').pop().toLowerCase();
        let reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = (event) => {
          let fileSource = event.target?.result;

          let documentObj = {
            'id': 0,
            'fileSource': fileSource,
            'imageName': fileName,
            'extension': fileExtension
          }

          this.imageData = documentObj;
        };
      }

      console.log(this.imageData);
    }
  }

    onFileChange(event: any) {
    const reader = new FileReader();
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);

      reader.onload = () => {
        this.imageSrc = reader.result as string;
        let fileName = file.name;
        let fileExtension = fileName.split('.').pop().toLowerCase();
        let documentObj = {
            'id': 0,
            'fileSource': this.imageSrc,
            'imageName': fileName,
            'extension': fileExtension
          }
          this.imageData = documentObj;
      };
    }
  }

  removeDocument() {
    this.imageData = null;
  }

  onFileBrowse(event: any) {
    this.uploadedMedia = [];
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
        this.documents = {
          id: 0,
          filePath: fileUrl,
          fileName: fileName,
          statusId: 0,
          extension: fileExtension
        };
        // this.documents.push(documentObj);

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

  setFileData(event: Event): void {
    const eventTarget: HTMLInputElement | null = event.target as HTMLInputElement | null;
    if (eventTarget?.files?.[0]) {
      const file: File = eventTarget.files[0];
      const reader = new FileReader();
      reader.addEventListener('load', () => {
        this.itemForm.get('image')?.setValue(reader.result as string);
      });
      reader.readAsDataURL(file);
    }
  }

  removeImage(index: number): void {
    this.logos.splice(index, 1);
    this.itemForm.get('image')?.setValue(this.logos);
    this.uploadedMedia = [];
  }


  GetDocument(template: any) {
    this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(this.imageData?.fileSource + '#toolbar=0');
    this.dialogRef = this.dialog.open(template, {
      width: '50%',
      height: '70%',
      disableClose: true,
    });
  }

  showOptions(event: MatSlideToggleChange): void {
    const checked = event.checked;

    if (checked) {
      this.itemForm.get('isGroupItem')?.setValue(true);
      // If group mode ON, ensure we have at least one row
      if (this.itemGroup.length === 0) {
        this.addGroup(0);
      }
    } else {
      this.itemForm.get('isGroupItem')?.setValue(false);
      // If group mode OFF, clear the array
      this.itemGroup.clear();
    }
  }
 
    onFileSourceRemove(event: any) {
    this.itemForm
      .get('image')?.patchValue('');
    this.imageSrc = '';
  }

}
