import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { IndentTypeEndPoints } from './indenttype.endpoints';

@Injectable({
    providedIn: 'root'
})

export class IndentTypeService extends BaseService<any> {

    endPointControllerName = "IndentType";
    constructor(httpClient: HttpClient, private http: HttpClient, private indenttypeEndPoints: IndentTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllIndentTypes(indenttypesFilterForm: any) {
        return this.post(indenttypesFilterForm, this.endPointControllerName + this.indenttypeEndPoints.getAllIndentTypes)
            .pipe(map((data: any) => data));
    }

    saveIndentType(saveIndentTypeCommand: any) {
        return this.post(saveIndentTypeCommand, this.endPointControllerName + this.indenttypeEndPoints.saveIndentType)
            .pipe(map((data: any) => data));
    }

    deleteIndentType(id: number) {
        return this.delete(id, this.endPointControllerName + this.indenttypeEndPoints.deleteIndentType)
            .pipe();
    }

    getIndentTypeById(id: number) {
        return this.get(id, this.endPointControllerName + this.indenttypeEndPoints.getIndentTypeById)
            .pipe(map((data: any) => data));
    }

    getIndentTypeByName(name: string) {
        return this.get(name, this.endPointControllerName + this.indenttypeEndPoints.getIndentTypeByName)
            .pipe(map((data: any) => data));
    }

    GetIndentTypeByCompany(companyId: number) {
        return this.get('?CompanyId=' + companyId, this.endPointControllerName + this.indenttypeEndPoints.getIndentTypeByCompany)
            .pipe(map((data: any) => data));
    }
}
