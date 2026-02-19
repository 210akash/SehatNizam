import { HttpBackend, HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FingerPrintEndPoints } from './fingerprint.endpoints';
import { BaseService } from 'app/Service/base.service';
import { environment } from 'environments/environment';
import { map } from 'rxjs';



@Injectable({
  providedIn: 'root'
})
export class FingerPrintService extends BaseService<any>{

  constructor(private http: HttpClient, httpClient: HttpClient, private fingerPrintEndPoints: FingerPrintEndPoints) {
    super(
      httpClient,
      'https://localhost:8083'
    );
  }

  Verify(DeviceHandle: any, pageID: any, userSerialNo: any) {
    return this.getexternal('?dummy=' + Math.random() + '&sHandle=' + DeviceHandle + '&id=' + pageID + '&userSerialNo=' + userSerialNo + '&extractEx=' + 0 + '&qualityLevel=' + 1, this.fingerPrintEndPoints.verify)
      .pipe(map((data: any) => data));
  }
  Verify1(DeviceHandle: any, pageID: any, userSerialNo: any) {
    return this.getexternal('?dummy=' + Math.random() + '&sHandle=' + DeviceHandle + '&id=' + pageID + '&userSerialNo=' + userSerialNo + '&extractEx=' + 0 + '&qualityLevel=' + 1, this.fingerPrintEndPoints.verify)
      .pipe(map((data: any) => data));
  }
  VerifyTest(DeviceHandle: any, pageID: any, tempLen: any, tempData: any) {
    return this.getexternalwithcookies1('db/verifyTemplate?dummy=' + Math.random() + '&sHandle=' + DeviceHandle + '&id=' + pageID + '&tempLen=' + tempLen + '&tempData=' + tempData + '&encrypt=0'+'&encryptKey='+
    '&extractEx=0'+'&qualityLevel=1')
      .pipe(map((data: any) => data));
  }
  VerifyTest2(DeviceHandle: any, pageID: any) {
    return this.getexternal('api/getTemplateData?dummy=' + Math.random() + '&sHandle=' + DeviceHandle + '&id=' + pageID + '&encrypt='+'&encryptKey='+
    '&extractEx='+'&qualityLevel=1')
      .pipe(map((data: any) => data));
  }
  // Init() {
  //   (this.fingerprintapiService.initDevice()).subscribe(
  //     {
  //       next: (data) => {
  //         console.log(data);
  //       },
  //       error: (error) => {
  //         console.log(error);
  //       }
  //     });
  // }

}
