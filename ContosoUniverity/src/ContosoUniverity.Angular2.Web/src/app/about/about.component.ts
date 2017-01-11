import { Component, OnInit } from '@angular/core';

import { IEnrollmentStats } from './enrollment-stats'
import { AboutService } from './about.service';

@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent implements OnInit {

  constructor(private _aboutService: AboutService) { }

  public title: string = 'Student Body Statistics';

  errorMessage: string;
  stats: IEnrollmentStats[];

  ngOnInit() {
    this._aboutService.getStats()
            .subscribe(stats => this.stats = stats,
                           error => this.errorMessage = <any>error);
  }

}
