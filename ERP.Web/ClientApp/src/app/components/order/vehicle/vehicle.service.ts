import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { VehicleEndPoints } from './vehicle.endpoints';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';



@Injectable({
    providedIn: 'root'
})

export class VehicleService extends BaseService<any> {

    endPointControllerName = 'Vehicle';

    constructor(private http: HttpClient, httpClient: HttpClient, private vehicleEndPoints: VehicleEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async saveVehicle(createVehicleForm: any) {
        return await this.post(createVehicleForm, this.endPointControllerName + this.vehicleEndPoints.saveVehicle)
            .pipe(map((data: any) => data));
    }

    async getAllVehicle(vehicleFilterForm: any) {
        return await this.post(vehicleFilterForm, this.endPointControllerName + this.vehicleEndPoints.getAllVehicle)
            .pipe(map((data: any) => data));
    }

    async getVehicleById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.vehicleEndPoints.getVehicleById)
            .pipe(map((data: any) => data));
    }

    async deleteVehicle(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.vehicleEndPoints.deleteVehicle)
            .pipe(map((data: any) => data));
    }

    async getVehiclesByDealership(dealershipId: number) {
        return this.get('?dealershipId=' + dealershipId, this.endPointControllerName + this.vehicleEndPoints.getVehiclesByDealership)
            .pipe(map((data: any) => data));
    }

    async getVehicleByName(name: any) {
        return this.get('?name=' + name, this.endPointControllerName + this.vehicleEndPoints.getVehicleByName)
            .pipe(map((data: any) => data));
    }

}