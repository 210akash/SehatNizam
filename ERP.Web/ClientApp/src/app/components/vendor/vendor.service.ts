import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { VendorEndPoints } from './vendor.endpoints';

@Injectable({
    providedIn: 'root'
})

export class VendorService extends BaseService<any> {

    endPointControllerName = "Vendor";
    constructor(httpClient: HttpClient, private http: HttpClient, private vendorEndPoints: VendorEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllVendors(vendorsFilterForm: any) {
        return this.post(vendorsFilterForm, this.endPointControllerName + this.vendorEndPoints.getAllVendors)
            .pipe(map((data: any) => data));
    }

    saveVendor(saveVendorCommand: any) {
        return this.post(saveVendorCommand, this.endPointControllerName + this.vendorEndPoints.saveVendor)
            .pipe(map((data: any) => data));
    }

    deleteVendor(id: number) {
        return this.delete(id, this.endPointControllerName + this.vendorEndPoints.deleteVendor)
            .pipe();
    }

    getVendorById(id: number) {
        return this.get(id, this.endPointControllerName + this.vendorEndPoints.getVendorById)
            .pipe(map((data: any) => data));
    }

    getVendorByName(name: string) {
        return this.get(name, this.endPointControllerName + this.vendorEndPoints.getVendorByName)
            .pipe(map((data: any) => data));
    }

}
