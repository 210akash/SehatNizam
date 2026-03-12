import { Component, OnInit, Inject } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { PrimaryOrderService } from '../order.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { DealershipService } from '../../dealership/dealership.service';
import { ItemService } from '../../../item/item.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'app-create-order',
  templateUrl: './create-order.component.html',
  styleUrls: ['./create-order.component.css'], standalone: false
})

export class CreateOrderComponent implements OnInit {
  createOrderForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  dealershipList: any[] = [];
  productsList: any[] = [];
  selectedDealer: any;
  totalQuantity = 0;
  billingAmount = 0;
  totalStockQuantity = 0;
  documents: any[] = [];
  urlSafe: SafeResourceUrl | undefined;
  dialogRef: any;
  editData: any;

  constructor(private sanitizer: DomSanitizer, private itemService: ItemService, private dealershipService: DealershipService, private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, private orderService: PrimaryOrderService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createOrderForm = this.formBuilder.group({
      id: [0],
      createdDate: [new Date(), Validators.required],
      dealershipId: ['', Validators.required],
      dealershipName: ['', Validators.required],
      territory: ['', Validators.required],
      dealershipAddress: ['', Validators.required],
      orderItemsList: this.formBuilder.array([])
    });
    // this.getAllDealership();
    if (this.data.element != null) {
      this.getOrderById();
    }
    // else {
    //   this.getAllProducts();
    // }
  }

  get f() {
    return this.createOrderForm.controls;
  }

  async saveOrder() {
    this.isLoading = true;

    if (this.createOrderForm.invalid) {
      this.constantService.markFormGroupTouched(this.createOrderForm);
      return;
    }

    if (!this.checkIfAtLeastOneNonZero()) {
      this.notificationsService.showNotification('Please Add at least 1 Quantity!', 'snack-bar-success');
      return;
    }

    if (!this.documents || this.documents.length === 0) {
      this.notificationsService.showNotification('Please Attach Documents', 'snack-bar-success');
      return;
    }

    let _createOrderForm: any = {};
    _createOrderForm = Object.assign(_createOrderForm, this.createOrderForm.value);
    _createOrderForm['orderAttachments'] = this.documents;

    let chequeDate = new Date(this.createOrderForm.get('createdDate')?.value);
    _createOrderForm['createdDate'] = chequeDate.toLocaleString();

    (await this.orderService.saveOrder(_createOrderForm)).subscribe(
      {
        next: (data: any) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Order Saved Successfully!', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Name already exist!', 'snack-bar-danger');
            this.isLoading = false;
          }
        },
        error: (error: any) => {
          this.notificationsService.showNotification('Please Fill the required fields!', 'snack-bar-danger');
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  LoadData(element: any) {
    if(element != null) {
      this.isEditMode = true;
     this.createOrderForm.get('createdDate')?.patchValue(element?.createdDate.toLocaleString());
      }
    this.createOrderForm.get('dealershipId')?.patchValue(element?.dealership?.name);
    this.createOrderForm.get('dealershipName')?.patchValue(element?.dealership?.name);
    this.createOrderForm.get('territory')?.patchValue(element?.dealership?.territory?.name);

    this.constantService.LoadData(element, this.createOrderForm);
    this.setEditProducts(element.orderItems);
    this.documents = element?.orderAttachments?.filter((x: { isActive: boolean; }) => x.isActive == true);
  }

  // async getAllDealership() {

  //   let dealershipForm = {
  //     'dealershipTypeId': 1
  //   };

  //   (await this.dealershipService.getAllDealership(dealershipForm)).subscribe({
  //     next: (data) => {
  //       this.dealershipList = data.item1;
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }
  async getDealershipsList(event: any) {
    const filter = event.currentTarget.value;
    this.dealershipList = [];
    (await this.dealershipService.getAllActiveByName(filter)).subscribe(
      (data: any) => {
        this.dealershipList = data || [];
      },
      (error: any) => {
        console.error('Error fetching distributor list:', error);
        this.dealershipList = [];
      }
    );
  }

  onInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      console.log(`Input cleared!`);
      //this.resetitem(index); // Call a function when cleared
    }
  }

  onOptionDealershipSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      return;
    }
    this.createOrderForm.get('dealershipId')?.patchValue(selectedValue.id);
    this.createOrderForm.get('dealershipName')?.patchValue(selectedValue.name);
    this.createOrderForm.get('territory')?.patchValue(selectedValue?.territory?.name);

    this.onDealershipChange();
  }

  onDealershipChange() {

    let dealershipId = this.createOrderForm.get('dealershipId')?.value;

    this.selectedDealer = this.dealershipList.filter(x => x.id == dealershipId)[0];
    this.createOrderForm.get('dealershipAddress')?.patchValue(this.selectedDealer?.address);
    this.getAllProductsByDistributorId();
  }

  // async getAllProducts() {
  //   let productsForm = {};
  //   (await this.itemService.getAllItems(productsForm)).subscribe({
  //     next: (data) => {
  //       this.setAddProducts(data.item1);
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }

  async getAllProductsByDistributorId() {
    if (this.selectedDealer == undefined) {
      // meanz no distributor selected
      this.createOrderForm.setControl('orderItemsList', this.formBuilder.array([]));
      return;
    }
    (await this.itemService.getKCItems(this.selectedDealer.territoryId)).subscribe({
      next: (data: any) => {

        if (Array.isArray(data) && data.length > 1 && data[0]?.id) {
          this.setAddProducts(data);
          this.isLoading = false;
        }
        else if (data.Status == 500) {
          this.createOrderForm.setControl('orderItemsList', this.formBuilder.array([]));
          this.notificationsService.showNotification('No active Distributor Price Groups found for the Selected Distributor!', 'snack-bar-danger');
        }
        // if (data.Status === 200) {
        //   this.setAddProducts(data.Data);
        //   this.isLoading = false;
        // } else if (data.Status === 409) {
        //   this.notificationsService.showNotification('Some Error has been occure!', 'snack-bar-danger');
        //   this.isLoading = false;
        // }
      },
      error: (error: any) => {
        this.notificationsService.showNotification('Please Fill the required fields!', 'snack-bar-danger');
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async setAddProducts(data: any) {
    this.createOrderForm.setControl('orderItemsList', this.formBuilder.array([]));
    const productArray = this.createOrderForm.get('orderItemsList') as FormArray;

    data.filter((x: { isActive: boolean; }) => x.isActive == true).forEach((product: { id: any; name: any; quantity: number | null; image: any; itemType: any; volume: any; quantityInPack: any; retailPrice: any; tradePrice: any; distributorPrice: number; leftQuantity: number; distributorPromo: number; }) => {
      productArray.push(this.formBuilder.group({

        itemId: new FormControl(product.id),
        name: new FormControl(product.name),
        quantity: new FormControl(product.quantity == null ? 0 : product.quantity),
        image: new FormControl(product.image),
        type: new FormControl(product.itemType?.name),
        volume: new FormControl(product.volume),
        quantityInPack: new FormControl(product.quantityInPack),
        retailPrice: new FormControl(product.retailPrice),
        tradePrice: new FormControl(product.tradePrice),
        distributorPrice: new FormControl(product.distributorPrice),
        distributorPromo: new FormControl(product.distributorPromo),
        billPrice: new FormControl(product.quantity == null ? 0 : product.quantity * product.distributorPrice),
        leftQuantity: new FormControl(data.leftQuantity),

      }));
    });
  }

  async setEditProducts(data: any) {

    const productArray = this.createOrderForm.get('orderItemsList') as FormArray;
    data.forEach((data: { id: any; item: { id: any; name: any; image: any; itemType: any; volume: any; quantityInPack: any; }; quantity: number; retailPrice: any; tradePrice: any; distributorPrice: number; leftQuantity: number; distributorPromo: number; }) => {
      productArray.push(this.formBuilder.group({

        id: new FormControl(data.id),
        itemId: new FormControl(data.item?.id),
        name: new FormControl(data.item?.name),
        quantity: new FormControl(data.quantity),
        image: new FormControl(data.item?.image),
        type: new FormControl(data.item?.itemType?.name),
        volume: new FormControl(data.item?.volume),
        quantityInPack: new FormControl(data.item?.quantityInPack),
        retailPrice: new FormControl(data.retailPrice),
        tradePrice: new FormControl(data.tradePrice),
        distributorPrice: new FormControl(data.distributorPrice),
        billPrice: new FormControl(data.distributorPrice * data.quantity),
        leftQuantity: new FormControl(data.leftQuantity),
        distributorPromo: new FormControl(data.distributorPromo),
      }));
    });

    this.updateTotals();
  }

  get orderItemsList(): FormArray {
    return this.createOrderForm.get('orderItemsList') as FormArray;
  }

  checkIfAtLeastOneNonZero(): boolean {
    return this.orderItemsList.controls.some(formGroup => {
      const quantityControl = (formGroup as FormGroup).get('quantity');
      return quantityControl && quantityControl.value > 0;
    });
  }

  updateTotals() {
    this.totalQuantity = 0;
    this.billingAmount = 0;
    this.totalStockQuantity = 0;

    this.orderItemsList.controls.forEach(control => {
      const quantity = control.get('quantity')?.value || 0;
      const distributorPrice = control.get('distributorPrice')?.value || 0;
      const leftQuantity = control.get('leftQuantity')?.value || 0;

      this.totalQuantity += quantity;
      this.billingAmount += quantity * distributorPrice;
      this.totalStockQuantity += leftQuantity;
    });

  }
  updatePrice(index: number): void {
    const productControl = this.orderItemsList.at(index) as FormGroup;
    const quantity = productControl.get('quantity')?.value || 0;
    const distributorPrice = productControl.get('distributorPrice')?.value || 0;

    // Calculate the price
    const billPrice = quantity * distributorPrice;

    // Update the price field
    productControl.patchValue({ billPrice });
  }

  // Start Attatchments

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

          this.documents.push(documentObj);
        };
      }

      console.log(this.documents);
    }
  }


  // onDocumentSourceRemove(event: any, docIndex: number) {
  //   this.removeDraftsmanDesignDocuments(docIndex);
  // }

  onDocumentSourceRemove() {
    this.documents = [];
  }

  removeDraftsmanDesignDocuments(i: number) {
    this.documents.splice(i, 1);
  }

  GetDocument(event: any, index: number, template: any) {
    this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(this.documents[index].fileSource + '#toolbar=0');
    this.dialogRef = this.dialog.open(template, {
      width: '50%',
      height: '70%',
      disableClose: true,
    });
  }

  // End Attatchments

  async getOrderById() {
    (await this.orderService.getOrderById(this.data.element.id)).subscribe({
      next: (data: any) => {

        this.editData = data;
        this.LoadData(data);
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}
