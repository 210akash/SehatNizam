import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { SaleMaterialEndPoints } from './salematerial.endpoints';

@Injectable({
    providedIn: 'root'
})

export class SaleMaterialService extends BaseService<any> {

    endPointControllerName = "SaleMaterial";
    constructor(httpClient: HttpClient, private http: HttpClient, private salematerialEndPoints: SaleMaterialEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllSaleMaterials(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.salematerialEndPoints.getAllSaleMaterials)
            .pipe(map((data: any) => data));
    }

    saveSaleMaterial(saveSaleMaterialCommand: any) {
        return this.post(saveSaleMaterialCommand, this.endPointControllerName + this.salematerialEndPoints.saveSaleMaterial)
            .pipe(map((data: any) => data));
    }

    deleteSaleMaterial(id: number) {
        return this.delete(id, this.endPointControllerName + this.salematerialEndPoints.deleteSaleMaterial)
            .pipe();
    }

    getSaleMaterialById(id: number) {
        return this.get(id, this.endPointControllerName + this.salematerialEndPoints.getSaleMaterialById)
            .pipe(map((data: any) => data));
    }

    getSaleMaterialByName(name: string) {
        return this.get(name, this.endPointControllerName + this.salematerialEndPoints.getSaleMaterialByName)
            .pipe(map((data: any) => data));
    }

    getSaleMaterialCode() {
        return this.get(this.endPointControllerName + this.salematerialEndPoints.getSaleMaterialCode)
            .pipe(map((data: any) => data));
    }
    
    processSaleMaterial(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.salematerialEndPoints.processSaleMaterial)
            .pipe();
    }

    getSaleMaterialCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.salematerialEndPoints.getSaleMaterialCount)
            .pipe(map((data: any) => data));
    }

    approveSaleMaterial(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.salematerialEndPoints.approveSaleMaterial)
            .pipe();
    }

      rejectSaleMaterial(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.salematerialEndPoints.rejectSaleMaterial)
            .pipe();
    }

    getSaleMaterialByItem(itemId: number) {
        return this.get('?itemId='+itemId, this.endPointControllerName + this.salematerialEndPoints.getSaleMaterialByItem)
            .pipe();
    }
}
