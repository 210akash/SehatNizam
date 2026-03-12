import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { DeviceEndPoints } from './device.endpoints';

@Injectable({
    providedIn: 'root'
})

export class DeviceService extends BaseService<any> {

    endPointControllerName = "Device";
    constructor(httpClient: HttpClient, private http: HttpClient, private deviceEndPoints: DeviceEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllDevices(devicesFilterForm: any) {
        return this.post(devicesFilterForm, this.endPointControllerName + this.deviceEndPoints.getAllDevices)
            .pipe(map((data: any) => data));
    }

    saveDevice(saveDeviceCommand: any) {
        return this.post(saveDeviceCommand, this.endPointControllerName + this.deviceEndPoints.saveDevice)
            .pipe(map((data: any) => data));
    }

    deleteDevice(id: number) {
        return this.delete(id, this.endPointControllerName + this.deviceEndPoints.deleteDevice)
            .pipe();
    }

        checkDeviceStatus(ipAdress:any,port: number) {
        return this.get('?ipAdress=' + ipAdress + '&port=' +  port, this.endPointControllerName + this.deviceEndPoints.deleteDevice)
            .pipe();
    }
}
