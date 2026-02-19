import { HttpBackend, HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
//import { AuthenticationService } from '@app/core/services/authentication.service';
// import { FileSaverService } from 'ngx-filesaver';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { GeneralService } from './general.service';


@Injectable({
  providedIn: 'root'
})
export class ConstantService {

  login() {
    this.router.navigate(['/login']);
  }

  public ckConfig = {
    toolbar: {
      items: ['heading', '|', 'fontfamily', 'fontsize', '|', 'alignment', '|',
        'fontColor', 'fontBackgroundColor', '|',
        'bold', 'italic', 'strikethrough', 'underline', 'subscript', 'superscript', '|',
        'link', '|',
        'custombutton',
        'outdent', 'indent', '|',
        'bulletedList', 'numberedList', 'todoList', '|',
        'code', 'codeBlock', '|',
        'insertTable', '|',
        'blockQuote', '|',
        'undo', 'redo'
      ],

    }
  };


  async getAllSelectWhereData(table: string, valueColumn: string, textColumn: string, where: string, parm: string, howmany?: number) {
    return await this.generalService.getAllSelectWhereData(table, valueColumn, textColumn, where, parm, howmany).toPromise();
  }

  async GetAllSites() {
    let member_type: any = localStorage.getItem('member_type');
    if (member_type == null) {
      member_type = JSON.stringify(await this.generalService.getAllMemberType().toPromise());
      localStorage.setItem('member_type', member_type);
    }
    return JSON.parse(member_type);
  }

  public disableConfig = {
    isReadOnly: true,
    toolbar: {
      items: [],

    }
  };

  defaultPage: number;
  defaultItemPerPage: number;
  constructor(
    private httpClient: HttpClient,
    private httpBackend: HttpBackend,
    private router: Router,
    private generalService: GeneralService,
    // private userService: UserService
  ) {
    this.defaultPage = 1;
    this.defaultItemPerPage = 5;
  }

  download(url: string, fromRemote = true) {
    const header = new HttpHeaders()
      .append('Content-Type', 'application/json')
      .append('Access-Control-Allow-Headers', 'Content-Type')
      .append('Access-Control-Allow-Methods', 'GET')
      .append('Access-Control-Allow-Origin', '*');
    this.httpClient = new HttpClient(this.httpBackend);
    const fileName = `save.png`;
    if (fromRemote) {
      this.httpClient.get(url, {
        observe: 'response',
        responseType: 'blob',
        headers: header
      }).subscribe(res => {
        //this.fileSaverService.save(res.body, fileName);
      });
      return;
    }
    //const fileType = this.fileSaverService.genType(fileName);
  }


  detachObject(model: any) {
    return JSON.parse(JSON.stringify(model));
  }

  public LoadData(data: any, formGroup: FormGroup) {
    let values: any;
    values = {};
    Object.keys(data).forEach(key => {
      values[key.toLowerCase()] = data[key];
    });
    Object.keys(formGroup.controls).forEach(key => {
      if (values[key.toLowerCase()] !== undefined) {
        formGroup.get(key)?.patchValue(values[key.toLowerCase()]);
      }
    });
  }

  addBodyBlur() {
    const body = document.getElementsByTagName('BODY')[0];
    // body.classList.add('filter-back');
    body.classList.add('overlay');
  }

  removeBodyBlur() {
    const body = document.getElementsByTagName('BODY')[0];
    // body.classList.remove('filter-back');
    body.classList.remove('overlay');
  }

  public convertDate(val: any) {
    const date = new Date(val);
    return ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '/' + ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate())) + '/' + date.getFullYear();
  }

  public convertTime(val: any): string {
    const date = new Date(val); // Convert the timestamp to a Date object
    
    // Extract hours and minutes
    var hours = date.getHours();
    var minutes = date.getMinutes();
  
    // Determine AM/PM
    let ampm = hours >= 12 ? 'PM' : 'AM';
    
    // Convert 24-hour time to 12-hour time
    hours = hours % 12;
    hours = hours ? hours : 12; // The hour '0' should be '12'
    var min;
    // Pad minutes with leading zero if necessary
    min = minutes < 10 ? '0' + minutes : minutes;
  
    // Return the formatted time
    return `${hours}:${minutes} ${ampm}`;
  }

  public formatDate(date: any) {
    const d = new Date(date);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    const year = d.getFullYear();
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    return [year, month, day].join('-');
  }

  dateDiff(startingDate: string, endingDate: string) {
    let startDate = new Date(new Date(startingDate).toISOString().substr(0, 10));
    if (!endingDate) {
      endingDate = new Date().toISOString().substr(0, 10);    // need date in YYYY-MM-DD format
    }
    let endDate = new Date(endingDate);
    if (startDate > endDate) {
      const swap = startDate;
      startDate = endDate;
      endDate = swap;
    }
    const startYear = startDate.getFullYear();
    const february = (startYear % 4 === 0 && startYear % 100 !== 0) || startYear % 400 === 0 ? 29 : 28;
    const daysInMonth = [31, february, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    let yearDiff = endDate.getFullYear() - startYear;
    let monthDiff = endDate.getMonth() - startDate.getMonth();
    if (monthDiff < 0) {
      yearDiff--;
      monthDiff += 12;
    }
    let dayDiff = endDate.getDate() - startDate.getDate();
    if (dayDiff < 0) {
      if (monthDiff > 0) {
        monthDiff--;
      } else {
        yearDiff--;
        monthDiff = 11;
      }
      dayDiff += daysInMonth[startDate.getMonth()];
    }

    let daysLabel = ' days, ';
    if (dayDiff <= 1) {
      daysLabel = ' day, ';
    }

    let monthsLabel = ' months, ';
    if (dayDiff <= 1) {
      monthsLabel = ' month, ';
    }


    return monthDiff + ' ' + monthsLabel + ' ' + dayDiff + ' ' + daysLabel;
  }

  // async GetAllRoles() {
  //   let roles: any = localStorage.getItem('roles');
  //   if (roles == null) {
  //     roles = JSON.stringify(await this.userService.getAllRoles().toPromise());
  //     localStorage.setItem('roles', roles);
  //   }
  //   return JSON.parse(roles);
  // }

  getTodayNumber() {
    const days = [1, 2, 3, 4, 5, 6, 7];
    const d = new Date();
    return days[d.getDay()];
  }

  setPostValue(schemeForm: any): any {
    let body: any;
    body = {};
    Object.keys(schemeForm.controls).forEach(key => {
      body[key] = schemeForm.controls[key].value;
    });
    return body;
  }

  getFileName(FileUrl: any) {
    const fileName = FileUrl.split('/').pop().substring(36);
    return fileName.replace(/%20/g, ' ');;
  }

  getFileNameWithSpace(fileName: any) {
    return fileName.replace(/%20/g, ' ');;
  }

  /**
   * Check file is image or not
   * @param filePath - File Path to get file extention
   */
  validatePath(path: string) {
    const _validFileExtensions = [".jpg", ".jpeg", ".bmp", ".gif", ".png"];
    var sFileName = path;
    if (sFileName.length > 0) {
      var blnValid = false;
      for (var j = 0; j < _validFileExtensions.length; j++) {
        var sCurExtension = _validFileExtensions[j];
        if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
          blnValid = true;
          break;
        }
      }
      if (!blnValid) {
        //  this.toastrService.info("Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensions.join(", "), "Info");
        return false;
      }
    }
    return true;
  }

  /**
   * Marks all controls in a form group as touched
   * @param formGroup - The form group to touch
   */
  public markFormGroupTouched(formGroup: FormGroup) {
    (<any>Object).values(formGroup.controls).forEach((control: any) => {
      control.markAsTouched();

      if (control.controls) {
        this.markFormGroupTouched(control);
      }
    });
  }
}


export enum IntimationEnum {
  Allocation_Letter = 1,
  Site_Plan = 2,
  Design = 3
}

export enum PossessionStatusEnum {
  All_Possession = 0,
  App_Print_Done = 1,
  IPD_Done = 2,
  Sent_To_Operations = 5,
  Operations_ToBeReceived = 10,
  Sent_To_Accounts = 15,
  Accounts_ToBeReceived = 20,
  // Sent_To_CFO = 25,
  CFO_ToBeReceived = 30,
  Sent_To_Audit = 35,
  Audit_ToBeReceived = 40,
  // Sent_To_Audit_HOD = 45,
  Audit_HOD_ToBeReceived = 50,
  Sent_To_Ops_EXE = 55,
  Ops_EXE_ToBeReceived = 60,
  Ops_EXE_Print_Allot_Letter_Done = 61,
  Ops_EXE_Signature_One_Done = 62,
  Ops_EXE_Signature_Two_Done = 63,
  // Sent_To_Ops_HOD = 65,
  Ops_HOD_ToBeReceived = 70,
  Sent_To_Ops_Dispatch = 75,
  Ops_Dispatch_ToBeReceived = 80,
  Sent_To_PossDesk_RWP = 85,
  PossDesk_ToBeReceived_RWP = 90,
  PossDesk_LHR_Processed_After_Second_Signature = 95,
  // PossDesk_LHR_ToBeReceived_After_Second_Signature = 100,
  Dispatch_To_LHR_After_Second_Signature = 105,
  PossDesk_RWP_Processed_After_Second_Signature = 106,
  PAL_Ready_For_Customer = 110,
  PAL_Received_by_Customer = 115,
}

export enum SitePlanStatusEnum {
  All_SitePlan = 0,
  PAL_Received_by_Customer = -1,
  App_Print_Done = 120,
  Rejected_RWP = 121,
  Rejected_LHR = 122,
  SitePlan_Upload_Done = 125,
  // SitePlan_Processed = 126,
  Sent_To_BB_RWP = 130,
  Sent_To_BB_LHR = 131,
  // Only For Grid View (Not using anywhere else) START
  Grid_Sent_To_BB_RWP = 1300,
  Grid_Sent_To_BB_LHR = 1310,
  // Only For Grid View (Not using anywhere else) END
  BB_ToBeReceived = 135,
  BB_SitePlan_Print_Downloaded = 136,
  BB_SitePlan_Print = 137,
  BB_SitePlan_Print_Done = 140,
  BB_First_Signature_Done = 145,
  BB_SitePlan_Upload_Done = 150,
  BB_Dispatched_RWP = 155,
  BB_Dispatched_LHR = 156,
  // Only For Grid View (Not using anywhere else) START
  Grid_BB_Dispatched_RWP = 1550,
  Grid_BB_Dispatched_LHR = 1560,
  // Only For Grid View (Not using anywhere else) END
  PossDesk_RWP_Site_Plan_Received = 160,
  PossDesk_RWP_Second_Signature_Done = 165,
  PossDesk_LHR_Site_Plan_Received = 170,
  PossDesk_LHR_Second_Signature_Done = 175,
  SitePlan_Ready_For_Customer = 180,
  Client_Received_Site_Plan = 185
}

export enum DesignStatusEnum {
  All_Design = 0,
  Site_Plan_Done = 185,
  // Voucher_Print_Done = 190,
  App_Print_Done = 195,
  Sent_To_Accounts = 200,
  Accounts_ToBeReceived = 205,
  Sent_To_Audit = 210,
  Audit_ToBeReceived = 215,
  Audit_HOD_ToBeReceived = 220,
  Sent_To_Ops_EXE = 225,
  Ops_EXE_ToBeReceived = 230,
  NOC_Print_Done = 231,
  BB_ToBeReceived = 235,
  Assigned_To_User = 240,
  Design_Approved = 245,
  Assigned_To_Draftsman = 250,
  Design_Ready_For_Customer = 255
}

export enum DraftsmanDesignStatusEnum {
  All_DraftsmanDesigns = 0,
  Architectural_Planning = 1002,
  Working_Drawings = 1003,
  MEP_Drawings = 1004,
  d3D = 1005,
  Soil_Test_Report = 1006,
  Structure = 1007
}
