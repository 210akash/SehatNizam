import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { FingerPrintEndPoints } from './fingerprint.endpoints';
import { environment } from 'environments/environment';
import { BaseService } from 'app/Service/base.service';

@Injectable({
    providedIn: 'root'
})
export class FingerPrintApiService extends BaseService<any>{

    endPointControllerName = 'FingerPrint';

    constructor(private http: HttpClient, httpClient: HttpClient, private fingerPrintEndPoints: FingerPrintEndPoints) {
        super(
            httpClient,
            'https://localhost:8083/api'
        );
    }

    createSessionID() {
        return this.getexternal('?dummy=' + Math.random(), this.fingerPrintEndPoints.createSessionID)
            .pipe(map((data: any) => data));
    }

    initDevice() {
        return this.getexternal('?dummy=' + Math.random(), this.fingerPrintEndPoints.initDevice)
            .pipe(map((data: any) => data));
    }

    unInitDevice() {
        return this.getexternal('?dummy=' + Math.random(), this.fingerPrintEndPoints.uninitDevice)
            .pipe(map((data: any) => data));
    }

    startCapturing(DeviceHandle: any, pageID: any, delayVal: any) {
        return this.getexternalwithcookies('?dummy=' + Math.random() + '&sHandle=' + DeviceHandle + '&id=' + pageID + '&resetTimer=' + delayVal, this.fingerPrintEndPoints.startCapturing)
            .pipe(map((data: any) => data));
    }

    GetScannerStatus(DeviceHandle: any) {
        return this.getexternalwithcookies1('?dummy=' + Math.random() + '&sHandle=' + DeviceHandle, this.fingerPrintEndPoints.getScannerStatus)
            .pipe(map((data: any) => data));
    }

    GetImageBuffer(DeviceHandle: any, pageID: any) {
        return this.getexternal('?dummy=' + Math.random() + '&sHandle=' + DeviceHandle + '&id=' + pageID + '&fileType=' + 1 + '&compressionRatio=' + 0.1, this.fingerPrintEndPoints.getImageData)
            .pipe(map((data: any) => data));
    }

    AutoCapture(DeviceHandle: any, pageID: any) {
        return this.getexternal('?dummy=' + Math.random() + '&sHandle=' + DeviceHandle + '&id=' + pageID, this.fingerPrintEndPoints.autoCapture)
            .pipe(map((data: any) => data));
    }

    Verify(DeviceHandle: any, pageID: any, userSerialNo: any) {
        return this.getexternal('?dummy=' + Math.random() + '&sHandle=' + DeviceHandle + '&id=' + pageID + '&userSerialNo=' + userSerialNo + '&extractEx=' + 0 + '&qualityLevel=' + 1, this.fingerPrintEndPoints.verify)
            .pipe(map((data: any) => data));
    }

    // Verify(DeviceHandle: any, pageID: any, userSerialNo: any) {
    //     const url = `${environment.dev_uri + '/db' + this.endPointControllerName + this.fingerPrintEndPoints.verify}/'?dummy=' + Math.random() + '&sHandle=' + DeviceHandle + '&id=' + pageID + '&userSerialNo=' + userSerialNo + '&extractEx=' + 0 + '&qualityLevel=' + 1`;
    //     return this.http.get(url);
    // }

    // Verify(DeviceHandle: any, pageID: any, userSerialNo: any) {
    //     this.http.get('https://localhost:8083/db/verify?dummy=' + Math.random() + '&sHandle=' + DeviceHandle + '&id=' + pageID + '&userSerialNo=' + userSerialNo + '&extractEx=' + 0 + '&qualityLevel=' + 1)
    //         .pipe(map((data: any) => data))
    //         .subscribe(res => {
    //             console.log(res);
    //             // You can perform additional actions with the response here
    //         });
    // }
    


    // startCapturing(DeviceHandle: any,pageID:any ,delayVal:any, username:any ) {
    //     const header = new HttpHeaders()
    //     .append('Content-Type', 'application/json')
    //     .append('Access-Control-Allow-Headers', 'Content-Type')
    //      .append('Access-Control-Allow-Credentials', 'true')  
    //      .append('cookie', 'username='+username)
    //      .append('Access-Control-Allow-Methods', 'GET')
    //     .append('Access-Control-Allow-Origin', 'http://localhost:4200');
    //       this.http.get('https://localhost:8083/api/startCapturing?dummy='+  Math.random()+'&sHandle='+ DeviceHandle+'&id='+pageID+'&resetTimer='+delayVal, {
    //         headers: header
    //       }).subscribe(res => {
    //         console.log(res);
    //         //this.fileSaverService.save(res.body, fileName);
    //       });
    //       return;
    //     //const fileType = this.fileSaverService.genType(fileName);
    //   }

    // async saveThumbImage(image: any) {
    //     //const url = "http://localhost:53780/api" || (this.endPointControllerName + this.fingerPrintEndPoints.saveFingerPrint);
    //     return await this.post(image, this.endPointControllerName + this.fingerPrintEndPoints.saveFingerPrint)
    //         .pipe(map((data: any) => data));
    // }

}