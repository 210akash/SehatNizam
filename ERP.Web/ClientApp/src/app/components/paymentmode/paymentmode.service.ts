import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { PaymentModeEndPoints } from './paymentmode.endpoints';

@Injectable({
    providedIn: 'root'
})

export class PaymentModeService extends BaseService<any> {

    endPointControllerName = "PaymentMode";
    constructor(httpClient: HttpClient, private http: HttpClient, private paymentmodeEndPoints: PaymentModeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllPaymentModes(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.paymentmodeEndPoints.getAllPaymentModes)
            .pipe(map((data: any) => data));
    }

    savePaymentMode(savePaymentModeCommand: any) {
        return this.post(savePaymentModeCommand, this.endPointControllerName + this.paymentmodeEndPoints.savePaymentMode)
            .pipe(map((data: any) => data));
    }

    deletePaymentMode(id: number) {
        return this.delete(id, this.endPointControllerName + this.paymentmodeEndPoints.deletePaymentMode)
            .pipe();
    }

    getPaymentModeById(id: number) {
        return this.get(id, this.endPointControllerName + this.paymentmodeEndPoints.getPaymentModeById)
            .pipe(map((data: any) => data));
    }

    getPaymentModeByName(name: string) {
        return this.get(name, this.endPointControllerName + this.paymentmodeEndPoints.getPaymentModeByName)
            .pipe(map((data: any) => data));
    }
}
