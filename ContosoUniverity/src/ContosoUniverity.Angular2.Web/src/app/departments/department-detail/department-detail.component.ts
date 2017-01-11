import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { IDepartment } from '../idepartment';
import { DepartmentService } from '../department.service';

@Component({
  selector: 'app-department-detail',
  templateUrl: './department-detail.component.html',
  styleUrls: ['./department-detail.component.css']
})
export class DepartmentDetailComponent implements OnInit {
  public title: string = 'Departments';
  public department: IDepartment;
  errorMessage: string;
  private sub: Subscription;

  constructor(private _route: ActivatedRoute,
    private _router: Router,
    private _departmentService: DepartmentService) { }

  ngOnInit() {
    this.sub = this._route.params.subscribe(
      params => {
        let id = +params['id'];
        this.getDepartment(id);
      });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  getDepartment(id: number) {
    this._departmentService.getDepartment(id).subscribe(
      dept => this.department = dept,
      error => this.errorMessage = <any>error);
  }

  onBack(): void {
    this._router.navigate(['/departments']);
  }

}
