import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { IndentrequestEndPoints } from './indentrequest.endpoints';

@Injectable({
    providedIn: 'root'
})

export class IndentrequestService extends BaseService<any> {

    endPointControllerName = "IndentRequest";
    constructor(httpClient: HttpClient, private http: HttpClient, private indentrequestEndPoints: IndentrequestEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllIndentrequests(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.indentrequestEndPoints.getAllIndentrequests)
            .pipe(map((data: any) => data));
    }

    saveIndentrequest(saveIndentrequestCommand: any) {
        return this.post(saveIndentrequestCommand, this.endPointControllerName + this.indentrequestEndPoints.saveIndentrequest)
            .pipe(map((data: any) => data));
    }

    deleteIndentrequest(id: number) {
        return this.delete(id, this.endPointControllerName + this.indentrequestEndPoints.deleteIndentrequest)
            .pipe();
    }

    getIndentrequestById(id: number) {
        return this.get(id, this.endPointControllerName + this.indentrequestEndPoints.getIndentrequestById)
            .pipe(map((data: any) => data));
    }

    getIndentrequestByName(name: string) {
        return this.get(name, this.endPointControllerName + this.indentrequestEndPoints.getIndentrequestByName)
            .pipe(map((data: any) => data));
    }

    getIndentrequestCode() {
        return this.get(this.endPointControllerName + this.indentrequestEndPoints.getIndentrequestCode)
            .pipe(map((data: any) => data));
    }
    
    processIndentrequest(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.indentrequestEndPoints.processIndentrequest)
            .pipe();
    }

    getIndentRequestCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.indentrequestEndPoints.getIndentRequestCount)
            .pipe(map((data: any) => data));
    }

    approveIndentrequest(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.indentrequestEndPoints.approveIndentrequest)
            .pipe();
    }


}
