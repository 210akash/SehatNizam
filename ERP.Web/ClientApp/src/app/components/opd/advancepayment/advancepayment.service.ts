import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { AdvancePaymentEndPoints } from './advancepayment.endpoints';

@Injectable({
    providedIn: 'root'
})
export class AdvancePaymentService extends BaseService<any>{

    endPointControllerName = "AdvancePayments";
    constructor(httpClient: HttpClient, private http: HttpClient, private admissionServiceEndPoints: AdvancePaymentEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllAdvancePayments(AdvancePaymentFilterForm: any) {
        return this.post(AdvancePaymentFilterForm, this.endPointControllerName + this.admissionServiceEndPoints.getAllAdvancePayments)
            .pipe(map((data: any) => data));
    }

    saveAdvancePayment(AdvancePaymentFilterForm: any) {
        return this.post(AdvancePaymentFilterForm, this.endPointControllerName + this.admissionServiceEndPoints.saveAdvancePayment)
            .pipe(map((data: any) => data));
    }

    deleteAdvancePayment(id: number) {
      return this.delete(id, this.endPointControllerName + this.admissionServiceEndPoints.deleteAdvancePayment)
          .pipe();
    }

     confirmAdvancePayment(id: number) {
      return this.delete(id, this.endPointControllerName + this.admissionServiceEndPoints.confirmAdvancePayment)
          .pipe();
    }
}
