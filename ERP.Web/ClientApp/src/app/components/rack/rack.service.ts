import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RackEndPoints } from './rack.endpoints';
import { BaseService } from '../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class RackService extends BaseService<any> {

    endPointControllerName = 'Rack';

    constructor(private http: HttpClient, httpClient: HttpClient, private rackEndPoints: RackEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getRackByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.rackEndPoints.getRackByName)
            .pipe(map((data: any) => data));
    }

    async saveRack(createRackForm: any) {
        return await this.post(createRackForm, this.endPointControllerName + this.rackEndPoints.saveRack)
            .pipe(map((data: any) => data));
    }

    async getAllRack(rackFilterForm: any) {
        return await this.post(rackFilterForm, this.endPointControllerName + this.rackEndPoints.getAllRack)
            .pipe(map((data: any) => data));
    }

    async getRackById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.rackEndPoints.getRackById)
            .pipe(map((data: any) => data));
    }

    async deleteRack(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.rackEndPoints.deleteRack)
            .pipe(map((data: any) => data));
    }


}
