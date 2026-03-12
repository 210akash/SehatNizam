import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { ShipmentModeEndPoints } from './shipmentmode.endpoints';

@Injectable({
    providedIn: 'root'
})

export class ShipmentModeService extends BaseService<any> {

    endPointControllerName = "ShipmentMode";
    constructor(httpClient: HttpClient, private http: HttpClient, private shipmentmodeEndPoints: ShipmentModeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllShipmentModes(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.shipmentmodeEndPoints.getAllShipmentModes)
            .pipe(map((data: any) => data));
    }

    saveShipmentMode(saveShipmentModeCommand: any) {
        return this.post(saveShipmentModeCommand, this.endPointControllerName + this.shipmentmodeEndPoints.saveShipmentMode)
            .pipe(map((data: any) => data));
    }

    deleteShipmentMode(id: number) {
        return this.delete(id, this.endPointControllerName + this.shipmentmodeEndPoints.deleteShipmentMode)
            .pipe();
    }

    getShipmentModeById(id: number) {
        return this.get(id, this.endPointControllerName + this.shipmentmodeEndPoints.getShipmentModeById)
            .pipe(map((data: any) => data));
    }

    getShipmentModeByName(name: string) {
        return this.get(name, this.endPointControllerName + this.shipmentmodeEndPoints.getShipmentModeByName)
            .pipe(map((data: any) => data));
    }
}
