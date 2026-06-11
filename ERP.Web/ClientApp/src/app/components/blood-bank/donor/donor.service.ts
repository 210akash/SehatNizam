import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';
import { DonorEndPoints } from './donor.endpoints';

@Injectable({ providedIn: 'root' })
export class DonorService extends BaseService<any> {
    endPointControllerName = 'BloodDonor';

    constructor(httpClient: HttpClient, private endpoints: DonorEndPoints) {
        super(httpClient, environment.dev_uri);
    }

    getAll(filterForm: any) {
        return this.post(filterForm, this.endPointControllerName + this.endpoints.getAll).pipe(map((data: any) => data));
    }

    getById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.endpoints.getById).pipe(map((data: any) => data));
    }

    save(command: any) {
        return this.post(command, this.endPointControllerName + this.endpoints.save).pipe(map((data: any) => data));
    }

    deleteItem(id: number) {
        return this.delete(id, this.endPointControllerName + this.endpoints.delete).pipe();
    }
}
