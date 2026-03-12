import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { EmployeeBankEndPoints } from './employee-bank.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class EmployeeBankService extends BaseService<any> {

    endPointControllerName = "EmployeeBank";
    constructor(httpClient: HttpClient, private http: HttpClient, private employeeBankEndPoints: EmployeeBankEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllEmployeeBanks(employeeBanksFilterForm: any) {
        return this.post(employeeBanksFilterForm, this.endPointControllerName + this.employeeBankEndPoints.getAllEmployeeBanks)
            .pipe(map((data: any) => data));
    }

    saveEmployeeBank(saveEmployeeBankCommand: any) {
        return this.post(saveEmployeeBankCommand, this.endPointControllerName + this.employeeBankEndPoints.saveEmployeeBank)
            .pipe(map((data: any) => data));
    }

    deleteEmployeeBank(id: number) {
        return this.delete(id, this.endPointControllerName + this.employeeBankEndPoints.deleteEmployeeBank)
            .pipe();
    }

    getEmployeeBankById(id: number) {
        return this.get(id, this.endPointControllerName + this.employeeBankEndPoints.getEmployeeBankById)
            .pipe(map((data: any) => data));
    }

    getEmployeeBankByName(name: string) {
        return this.get(name, this.endPointControllerName + this.employeeBankEndPoints.getEmployeeBankByName)
            .pipe(map((data: any) => data));
    }
}
