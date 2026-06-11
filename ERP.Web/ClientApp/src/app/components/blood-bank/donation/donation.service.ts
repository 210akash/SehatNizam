import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';
import { DonationEndPoints } from './donation.endpoints';

@Injectable({ providedIn: 'root' })
export class DonationService extends BaseService<any> {
    endPointControllerName = 'BloodDonation';

    constructor(httpClient: HttpClient, private endpoints: DonationEndPoints) {
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
