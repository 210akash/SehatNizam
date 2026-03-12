import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { CurrencyEndPoints } from './currency.endpoints';

@Injectable({
    providedIn: 'root'
})

export class CurrencyService extends BaseService<any> {

    endPointControllerName = "Currency";
    constructor(httpClient: HttpClient, private http: HttpClient, private currencyEndPoints: CurrencyEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllCurrencys(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.currencyEndPoints.getAllCurrencys)
            .pipe(map((data: any) => data));
    }

    saveCurrency(saveCurrencyCommand: any) {
        return this.post(saveCurrencyCommand, this.endPointControllerName + this.currencyEndPoints.saveCurrency)
            .pipe(map((data: any) => data));
    }

    deleteCurrency(id: number) {
        return this.delete(id, this.endPointControllerName + this.currencyEndPoints.deleteCurrency)
            .pipe();
    }

    getCurrencyById(id: number) {
        return this.get(id, this.endPointControllerName + this.currencyEndPoints.getCurrencyById)
            .pipe(map((data: any) => data));
    }

    getCurrencyByName(name: string) {
        return this.get(name, this.endPointControllerName + this.currencyEndPoints.getCurrencyByName)
            .pipe(map((data: any) => data));
    }
}
