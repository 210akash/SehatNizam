import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { DeliveryTermsEndPoints } from './deliveryterms.endpoints';

@Injectable({
    providedIn: 'root'
})

export class DeliveryTermsService extends BaseService<any> {

    endPointControllerName = "DeliveryTerms";
    constructor(httpClient: HttpClient, private http: HttpClient, private deliverytermsEndPoints: DeliveryTermsEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllDeliveryTerms(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.deliverytermsEndPoints.getAllDeliveryTerms)
            .pipe(map((data: any) => data));
    }

    saveDeliveryTerms(saveDeliveryTermsCommand: any) {
        return this.post(saveDeliveryTermsCommand, this.endPointControllerName + this.deliverytermsEndPoints.saveDeliveryTerms)
            .pipe(map((data: any) => data));
    }

    deleteDeliveryTerms(id: number) {
        return this.delete(id, this.endPointControllerName + this.deliverytermsEndPoints.deleteDeliveryTerms)
            .pipe();
    }

    getDeliveryTermsById(id: number) {
        return this.get(id, this.endPointControllerName + this.deliverytermsEndPoints.getDeliveryTermsById)
            .pipe(map((data: any) => data));
    }

    getDeliveryTermsByName(name: string) {
        return this.get(name, this.endPointControllerName + this.deliverytermsEndPoints.getDeliveryTermsByName)
            .pipe(map((data: any) => data));
    }
}
