import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';
import { AppointmentPaymentEndPoints } from './appointment-payment.endpoints';

@Injectable({ providedIn: 'root' })
export class AppointmentPaymentService extends BaseService<any> {
    endPointControllerName = 'AppointmentPayment';

    constructor(httpClient: HttpClient, private endpoints: AppointmentPaymentEndPoints) {
        super(httpClient, environment.dev_uri);
    }

    getAll(filterForm: any) {
        return this.post(filterForm, this.endPointControllerName + this.endpoints.getAll)
            .pipe(map((data: any) => data));
    }

    getGroups(filterForm: any) {
        return this.post(filterForm, this.endPointControllerName + this.endpoints.getGroups)
            .pipe(map((data: any) => data));
    }

    savePayment(command: any) {
        return this.post(command, this.endPointControllerName + this.endpoints.save)
            .pipe(map((data: any) => data));
    }

    approvePayments(command: any) {
        return this.post(command, this.endPointControllerName + this.endpoints.approve)
            .pipe(map((data: any) => data));
    }
}
