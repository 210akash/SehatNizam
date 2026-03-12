import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { LedgerEndPoints } from './ledger.endpoints';

@Injectable({
    providedIn: 'root'
})

export class LedgerService extends BaseService<any> {

    endPointControllerName = "Ledger";
    constructor(httpClient: HttpClient, private http: HttpClient, private ledgerEndPoints: LedgerEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    customerCurrentBalance(customerId: number) {
        return this.get('?customerId='+customerId, this.endPointControllerName + this.ledgerEndPoints.customerCurrentBalance)
            .pipe(map((data: any) => data));
    }

     async itemCurrentBalance(itemId: any) {
      return  await this.get('?itemId='+itemId, this.endPointControllerName + this.ledgerEndPoints.itemCurrentBalance)
      .pipe(map((data: any) => data));
    }
}
