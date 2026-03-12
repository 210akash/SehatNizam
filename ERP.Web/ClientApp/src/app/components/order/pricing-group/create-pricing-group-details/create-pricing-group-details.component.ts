import { Component, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { PricingGroupService } from '../pricing-group.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-create-pricing-group-details',
  templateUrl: './create-pricing-group-details.component.html',
  styleUrls: ['./create-pricing-group-details.component.css'], standalone: false
})

export class CreatePricingGroupDetailsComponent implements OnInit {
  createPricingGroupDetailsForm!: FormGroup;
  isLoading = false;

  constructor(private notificationsService: NotificationsService,
    private pricingGroupService: PricingGroupService,
    private constantService: ConstantService,
    private formBuilder: FormBuilder,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.getAllProducts();
    this.createPricingGroupDetailsForm = this.formBuilder.group({
      id: [0],
      getProductGroupDetails: this.formBuilder.array([]),
    });
  }

  async saveProductPricingDetails() {
    // Loop through each product and validate the fields
    const productArray = this.getProductGroupDetails.controls;
    for (const productControl of productArray) {

      const distributorPrice = productControl.get('distributorPrice');
      const distributorPromo = productControl.get('distributorPromo');
      const netDistributorPrice = productControl.get('netDistributorPrice');
      const tradePrice = productControl.get('tradePrice');
      const retailPrice = productControl.get('retailPrice');

      if (distributorPrice?.invalid || distributorPromo?.invalid || netDistributorPrice?.invalid || tradePrice?.invalid || retailPrice?.invalid) {
        this.notificationsService.showNotification('Please ensure all prices are valid integers and not empty.', 'snack-bar-danger');
        return;
      }
      netDistributorPrice?.enable();
    }

    // Your save logic here, e.g., sending data to API
    this.isLoading = true;
    this.createPricingGroupDetailsForm.value.id = this.data.element.id;
    var dataabc = this.createPricingGroupDetailsForm.value
    console.log("Final Data Sent to API:", dataabc);

    (await this.pricingGroupService.saveProductPricingDetails(this.createPricingGroupDetailsForm.value)).subscribe(
      {
        next: (data) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Distributor has been mapped Successfully!', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Some Error Occure!', 'snack-bar-danger');
            this.isLoading = false;
          }
        },
        error: (error) => {
          this.notificationsService.showNotification('Something went wrong!', 'snack-bar-danger');
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  get f() {
    return this.createPricingGroupDetailsForm.controls;
  }

  get getProductGroupDetails(): FormArray {
    return this.createPricingGroupDetailsForm.get('getProductGroupDetails') as FormArray;
  }

  async getAllProducts() {
    this.isLoading = true;
    (await this.pricingGroupService.getProductGroupDetailsByGroupId(this.data.element.id)).subscribe({
      next: (data) => {
        if (data.Status === 200) {
          this.setEditProducts(data.Data);
          this.isLoading = false;
        } else if (data.Status === 409) {
          this.notificationsService.showNotification('Some Error has been occure!', 'snack-bar-danger');
          this.isLoading = false;
        }
      },
      error: (error) => {
        this.notificationsService.showNotification('Please Fill the required fields!', 'snack-bar-danger');
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async setEditProducts(data: any) {
    const productArray = this.createPricingGroupDetailsForm.get('getProductGroupDetails') as FormArray;
    data.forEach((product: any) => {
      const productFormGroup = this.formBuilder.group({
        itemId: new FormControl(product.ItemId),
        productName: new FormControl(product.ProductName),
        productType: new FormControl(product.ProductType),
        volumeInMl: new FormControl(product.VolumeInMl),
        imageName: new FormControl(product.ImageName),
        quantityInPack: new FormControl(product.QuantityInPack),
        priceGroupDetailsId: new FormControl(product.PriceGroupDetailsId),
        retailPrice: new FormControl(product.RetailPrice, [Validators.required, Validators.pattern('^[0-9]+$')]),
        tradePrice: new FormControl(product.TradePrice, [Validators.required, Validators.pattern('^[0-9]+$')]),
        distributorPrice: new FormControl(product.DistributorPrice, [Validators.required, Validators.pattern('^[0-9]+$')]),
        distributorPromo: new FormControl(product.DistributorPromo || 0, [Validators.required, Validators.pattern('^[0-9]+$')]),
        netDistributorPrice: new FormControl(
          product.DistributorPrice - (product.DistributorPromo || 0),
          [Validators.required, Validators.pattern('^[0-9]+$')]
        ),
      });

      productFormGroup.get('netDistributorPrice')?.disable(); // Disable netDistributorPrice

      // Listen for changes in distributorPrice and distributorPromo
      productFormGroup.get('distributorPrice')?.valueChanges.subscribe(() => this.updateNetDistributorPrice(productFormGroup));
      productFormGroup.get('distributorPromo')?.valueChanges.subscribe(() => this.updateNetDistributorPrice(productFormGroup));

      productArray.push(productFormGroup);
    });
  }

  updateNetDistributorPrice(productFormGroup: FormGroup) {
    const distributorPrice = productFormGroup.get('distributorPrice')?.value || 0;
    let distributorPromo = productFormGroup.get('distributorPromo')?.value || 0;

    // Ensure distributorPromo is not greater than distributorPrice
    if (distributorPromo > distributorPrice) {
      distributorPromo = distributorPrice; // Adjust promo to max allowable value
      productFormGroup.get('distributorPromo')?.setValue(distributorPromo, { emitEvent: false });
    }

    const netPrice = distributorPrice - distributorPromo;
    productFormGroup.get('netDistributorPrice')?.setValue(netPrice, { emitEvent: false });
  }

  // Function to validate price (on input)
  validatePrice(event: any) {
    const value = event.target.value;
    if (!/^\d+$/.test(value)) {
      // If input is not an integer, reset the value
      event.target.value = '';
    }
  }


}
