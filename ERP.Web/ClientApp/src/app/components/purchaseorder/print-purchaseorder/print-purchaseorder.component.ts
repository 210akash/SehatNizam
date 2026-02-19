import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AuthenticationService } from '../../../Auth/authentication.service';

@Component({
    selector: 'app-print-purchaseorder',
    templateUrl: './print-purchaseorder.component.html',
    styleUrl: './print-purchaseorder.component.css',
    standalone: false
})

export class PrintPurchaseOrderComponent {
  purchaseorderForm!: FormGroup;
  isLoading = false;
  currentUser: any;
  currentDate : any;
  currentTime : any;
  tAmount: any = 0;
  tDiscount: any = 0;
  tExpense: any = 0;
  tSaleTax: any = 0;
  tfed: any = 0;
  partyAmount: any = 0;
  netAmount: any = 0;

  private readonly a:any = [
    '', 'One ', 'Two ', 'Three ', 'Four ', 'Five ', 'Six ', 'Seven ', 'Eight ', 'Nine ', 'Ten ',
    'Eleven ', 'Twelve ', 'Thirteen ', 'Fourteen ', 'Fifteen ', 'Sixteen ', 'Seventeen ', 'Eighteen ', 'Nineteen '
  ];
  private readonly b:any= [
    '', '', 'Twenty ', 'Thirty ', 'Forty ', 'Fifty ', 'Sixty ', 'Seventy ', 'Eighty ', 'Ninety '
  ];


  constructor(private constantService :ConstantService, private authenticationService :AuthenticationService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.currentUser = this.authenticationService.currentUserValue;
    this.currentDate = this.constantService.convertDate(new Date());
    this.currentTime = this.constantService.convertTime(new Date().getTime());
    this.calculateTotals();
    
  }
 
  convertNumberToWords(value: string): string {
    value = parseInt(value).toString();
    if ((value = value.toString()).length > 9) return 'Overflow';
    const paddedValue = '000000000' + value;
    const n = paddedValue.slice(-9).match(/^(\d{2})(\d{2})(\d{2})(\d{1})(\d{2})$/);
    if (!n) return 'Invalid number';

    let str = '';
    str += Number(n[1]) !== 0 ? (this.a[Number(n[1])] || this.b[n[1][0]] + this.a[n[1][1]]) + 'Crore ' : '';
    str += Number(n[2]) !== 0 ? (this.a[Number(n[2])] || this.b[n[2][0]] + this.a[n[2][1]]) + 'Lakh ' : '';
    str += Number(n[3]) !== 0 ? (this.a[Number(n[3])] || this.b[n[3][0]] + this.a[n[3][1]]) + 'Thousand ' : '';
    str += Number(n[4]) !== 0 ? (this.a[Number(n[4])] || this.b[n[4][0]] + this.a[n[4][1]]) + 'Hundred ' : '';
    str += Number(n[5]) !== 0 ? ((str !== '' ? 'And ' : '') + (this.a[Number(n[5])] || this.b[n[5][0]] + this.a[n[5][1]])) : '';
    str += 'Only';

    return str;
}

async calculateTotals() {
    var _tAmount = 0;
    var _tSaleTax = 0;
    var _tfed = 0;
    this.tExpense = this.data.element.deliveryCharges + this.data.element.otherCharges;
    this.tDiscount = this.data.element.discount;
    (this.data.element.purchaseOrderDetail).forEach((detail: any) => {
      // Access item and update the unitRate value
      var value = detail.value;
      var fed = detail.fed;

      _tAmount = _tAmount + value;
      var gst = detail.gst;
      _tSaleTax = _tSaleTax + gst;
      _tfed = _tfed + fed;
    });
    this.tAmount = _tAmount;
    this.tSaleTax = _tSaleTax;
    this.tfed = _tfed;
    this.netAmount = this.tAmount + this.tSaleTax + this.tExpense - this.tDiscount;
    this.partyAmount = (this.tAmount + this.tExpense) - this.tDiscount;
  }

  printDocument() {
    const printContent = document.getElementById("printDoc");
    const cssStyles = `
      <style>
        body {
            margin: 0;
            font-size: 1rem;
            font-weight: 400;
            line-height: 1.5;
            color: #212529;
            text-align: left;
            background-color: #fff;
            font-family: sans-serif;
        }
        .text-center{
            text-align: center !important;
        }
        .text-left{
            text-align: left !important;
        }
        .text-right {
            text-align: right !important;
        }
        .m-0{
            margin: 0 !important;
        }
        .mb-3{
            margin-bottom: 1rem !important;
        }
        .mb-4{
            margin-bottom: 1.5rem !important;
        }
        .mt-4, .my-4 {
            margin-top: 1.5rem!important;
        }
        *,
        ::after,
        ::before {
            box-sizing: border-box;
        }

        .row {
            display: -ms-flexbox;
            display: flex;
            -ms-flex-wrap: wrap;
            flex-wrap: wrap;
            margin-right: -15px;
            margin-left: -15px;
        }
        hr {
            box-sizing: content-box;
            height: 0;
            overflow: visible;
            border: 0;
            border-top: 1px solid rgba(0,0,0,.1);
        }

        .mt-5,
        .my-5 {
            margin-top: 3rem !important;
        }

        .container {
            width: 100%;
            padding-right: 20px;
            padding-left: 20px;
            margin-right: auto;
            margin-left: auto;
        }
        .container, .container-lg, .container-md, .container-sm, .container-xl {
            max-width: 1140px;
        }
        p, label, input, h1, h2, h3, h4, h5, h6, li {
            font-family: sans-serif;
        }
        h1, h2, h3, h4, h5, h6 {
            margin-top: 0;
            margin-bottom: 0.5rem;
        }
        .h1, .h2, .h3, .h4, .h5, .h6, h1, h2, h3, h4, h5, h6 {
            margin-bottom: 0.5rem;
            font-weight: 500;
            line-height: 1.2;
        }
        th,td {
            border: 1px solid;
            padding: 5px;
        }

        .bl_table td {
            text-align: center;
        }

        p {
            margin-bottom: 5px;
            margin-top: 5px;
        }

        input {
            border: none;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            border: 1px solid;
        }

        p,
        p,
        input,
        h1,
        h2,
        h3,
        h4,
        h5,
        h6,
        li {
            font-family: sans-serif;
        }

        p,
        p,
        input,
        li {
            font-size: 14px;
        }

        .underline {
            border-bottom: 2px solid #000;
        }

        h5 {
            font-size: 1.2rem;
            font-weight: 600;
        }
        .col, .col-1, .col-10, .col-11, .col-12, .col-2, .col-3, .col-4, .col-5, .col-6, .col-7, .col-8, .col-9, .col-auto, .col-lg, .col-lg-1, .col-lg-10, .col-lg-11, .col-lg-12, .col-lg-2, .col-lg-3, .col-lg-4, .col-lg-5, .col-lg-6, .col-lg-7, .col-lg-8, .col-lg-9, .col-lg-auto, .col-md, .col-md-1, .col-md-10, .col-md-11, .col-md-12, .col-md-2, .col-md-3, .col-md-4, .col-md-5, .col-md-6, .col-md-7, .col-md-8, .col-md-9, .col-md-auto, .col-sm, .col-sm-1, .col-sm-10, .col-sm-11, .col-sm-12, .col-sm-2, .col-sm-3, .col-sm-4, .col-sm-5, .col-sm-6, .col-sm-7, .col-sm-8, .col-sm-9, .col-sm-auto, .col-xl, .col-xl-1, .col-xl-10, .col-xl-11, .col-xl-12, .col-xl-2, .col-xl-3, .col-xl-4, .col-xl-5, .col-xl-6, .col-xl-7, .col-xl-8, .col-xl-9, .col-xl-auto {
            position: relative;
            width: 100%;
            padding-right: 15px;
            padding-left: 15px;
            padding: 0;
        }
        
        .pt-4, .py-4 {
            padding-top: 1.5rem!important;
        }
        .mt-2, .my-2 {
            margin-top: 0.5rem!important;
        }
        .mt-3 {
            margin-top: 1rem!important;
        }
        .h1, h1 {
            font-size: 2.5rem;
        }
        .h3, h3 {
            font-size: 1.25rem;
        }
        .h2, h2 {
            font-size: 2rem;
        }
        .mb-0, .my-0 {
            margin-bottom: 0!important;
        }
        .mb-5, .my-5 {
            margin-bottom: 3rem!important;
        }
        .possession_counter td:nth-child(even){
            text-align: center;
            padding: 0;
        }

        .justify-content-start{
            justify-content: flex-start;
        }
        .justify-content-center{
            justify-content: center;
        }
        .justify-content-end{
            justify-content: flex-end;
        }
        .p-0{
            padding: 0 !important;
        }
        @media (min-width: 768px){
            .col-md-1 {
                -ms-flex: 0 0 8.333333%;
                flex: 0 0 8.333333%;
                max-width: 8.333333%;
            }
            .col-md-2 {
                -ms-flex: 0 0 16.666667%;
                flex: 0 0 16.666667%;
                max-width: 16.666667%;
            }
            .col-md-3 {
                -ms-flex: 0 0 25%;
                flex: 0 0 25%;
                max-width: 25%;
            }
            .col-md-4 {
                -ms-flex: 0 0 33.333333%;
                flex: 0 0 33.333333%;
                max-width: 33.333333%;
            }
            .col-md-6 {
                -ms-flex: 0 0 50%;
                flex: 0 0 50%;
                max-width: 50%;
            }
            .col-md-7 {
                -ms-flex: 0 0 58.333333%;
                flex: 0 0 58.333333%;
                max-width: 58.333333%;
            }
            .col-md-8 {
                -ms-flex: 0 0 66.666667%;
                flex: 0 0 66.666667%;
                max-width: 66.666667%;
            }
            .col-md-10 {
                -ms-flex: 0 0 83.333333%;
                flex: 0 0 83.333333%;
                max-width: 83.333333%;
            }
            .col-md-11 {
                -ms-flex: 0 0 91.666667%;
                flex: 0 0 91.666667%;
                max-width: 91.666667%;
            }
            .col-md-12 {
                -ms-flex: 0 0 100%;
                flex: 0 0 100%;
                max-width: 100%;
            }
        }
        @media print{*,::after,::before{text-shadow:none!important;box-shadow:none!important}a:not(.btn){text-decoration:underline}abbr[title]::after{content:" (" attr(title) ")"}pre{white-space:pre-wrap!important}blockquote,pre{border:1px solid #adb5bd;page-break-inside:avoid}thead{display:table-header-group}img,tr{page-break-inside:avoid}h2,h3,p{orphans:3;widows:3}h2,h3{page-break-after:avoid}body{min-width:992px!important}.container{min-width:992px!important}}

        @media print {
            .row {
                display: -ms-flexbox;
                display: flex;
                -ms-flex-wrap: wrap;
                flex-wrap: wrap;
                margin-right: -15px;
                margin-left: -15px;
            }
            .col-md-1 {
                -ms-flex: 0 0 8.333333%;
                flex: 0 0 8.333333%;
                max-width: 8.333333%;
            }
            .col-md-2 {
                -ms-flex: 0 0 16.666667%;
                flex: 0 0 16.666667%;
                max-width: 16.666667%;
            }
            .col-md-3 {
                -ms-flex: 0 0 25%;
                flex: 0 0 25%;
                max-width: 25%;
            }
            .col-md-4 {
                -ms-flex: 0 0 33.333333% !important;
                flex: 0 0 33.333333% !important;
                max-width: 33.333333% !important;
            }
            .col-md-6 {
                -ms-flex: 0 0 50%;
                flex: 0 0 50%;
                max-width: 50%;
            }
            .col-md-7 {
                -ms-flex: 0 0 58.333333%;
                flex: 0 0 58.333333%;
                max-width: 58.333333%;
            }
            .col-md-8 {
                -ms-flex: 0 0 66.666667%;
                flex: 0 0 66.666667%;
                max-width: 66.666667%;
            }
            .col-md-11 {
                -ms-flex: 0 0 91.666667%;
                flex: 0 0 91.666667%;
                max-width: 91.666667%;
            }
            .col-md-12 {
                -ms-flex: 0 0 100%;
                flex: 0 0 100%;
                max-width: 100%;
            }
            .container {
                max-width: 1200px;
            }
            body {
                -webkit-print-color-adjust: exact;
                page-break-after: always;
            }

            .blue_header {
                background-color: #1f5ca8 !important;
                print-color-adjust: exact;
            }

            .lightblue_header {
                background-color: #D9E2F3 !important;
                print-color-adjust: exact;
            }
        }

        @media print {
            .pg_brk {
                page-break-after: always;
            }
        }
        .fw-bold{
            font-weight: 600;
        }
        th{
            font-weight: 600;
            font-family: sans-serif;
        }
        .no_border{
            border: 0;
        }
        .left_side{
            display: inline-flex;
            align-items: end;
        }
        .top_left{
            font-size: 14px;
        }
        .top_left th,
        .top_left td{
            border: 0;
            padding-top: 5px;
            padding-bottom: 0;
        }
        .top_left th{
            padding-left: 0;
        }
        .top_right{
            font-size: 14px;
        }
        .table_one{
            border: 0;
            text-align: center;
            font-size: 14px;
        }
        .table_one th{
            font-size: 12px;
        }
        .table_one td{
            font-size: 12px;
        }
        .sign_wrap{
            width: 100%;
            display: inline-flex;
        }
        .signed_by {
            width: 100%;
            max-width: 250px;
            display: inline-flex;
            justify-content: center;
            align-items: center;
            flex-direction: column;
        }
        .heading{
            border-top: 2px solid;
            width: 100%;
            text-align: center;
        }
        .print_date {
          display: inline-flex;
          gap: 20px;
          font-size: 14px;
          justify-content: flex-end;
          width: 100%;
        }
        .table_ship{
            font-size: 14px;
        }
        .table_ship td{
            height: 40px;
        }
        .remarks{
            width: 100%;
            display: inline-block;
        }
        .remarks ul{
            list-style: decimal;
        }
        .note {
            width: 100%;
            display: inline-flex;
            gap: 20px;
            align-items: center;
            font-weight: 600;
            font-size: 14px;
        }
        .logo{
            width: 100%;
            text-align: center;
            display: inline-flex;
            justify-content: flex-start;
            align-items: center;
            font-weight: 600;
            font-size: 1rem;
            gap: 15px;
         }
      </style>    
    `;
  
    if (printContent) {
      const WindowPrt = window.open('', '', 'left=0,top=0,width=1100,height=1100,toolbar=0,scrollbars=0,status=0');
      if (WindowPrt) {
        WindowPrt!.document.write(printContent.innerHTML);
        WindowPrt!.document.write(cssStyles);
        setTimeout(() => {
          WindowPrt!.focus();
          WindowPrt!.print();
          WindowPrt!.close();
        }, 100);    
      }
    }
  }
  
}
