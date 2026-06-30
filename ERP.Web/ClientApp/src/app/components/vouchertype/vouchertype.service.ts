import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { VoucherTypeEndPoints } from './vouchertype.endpoints';

@Injectable({
    providedIn: 'root'
})

export class VoucherTypeService extends BaseService<any> {

    endPointControllerName = "VoucherType";
    constructor(httpClient: HttpClient, private http: HttpClient, private vouchertypeEndPoints: VoucherTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllVoucherTypes(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.vouchertypeEndPoints.getAllVoucherTypes)
            .pipe(map((data: any) => data));
    }

    saveVoucherType(saveVoucherTypeCommand: any) {
        return this.post(saveVoucherTypeCommand, this.endPointControllerName + this.vouchertypeEndPoints.saveVoucherType)
            .pipe(map((data: any) => data));
    }
}
