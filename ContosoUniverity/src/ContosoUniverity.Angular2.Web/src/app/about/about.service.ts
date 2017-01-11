import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/map';

import { IEnrollmentStats } from './enrollment-stats';

@Injectable()
export class AboutService {
  private _statsUrl = 'http://localhost:60360/api/about';

  constructor(private _http: Http) { }

  getStats(): Observable<IEnrollmentStats[]> {
        return this._http.get(this._statsUrl)
            .map((response: Response) => <IEnrollmentStats[]> response.json())
            .do(data => console.log('All: ' +  JSON.stringify(data)))
            .catch(this.handleError);
    }

    private handleError(error: Response) {
        console.error(error);
        return Observable.throw(error.json().error || 'Server error');
    }

}
