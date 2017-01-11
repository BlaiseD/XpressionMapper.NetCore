import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { IStudent } from '../istudent';
import { StudentService } from '../student.service';

@Component({
  selector: 'app-student-detail',
  templateUrl: './student-detail.component.html',
  styleUrls: ['./student-detail.component.css']
})
export class StudentDetailComponent implements OnInit {
  public title: string = 'Students';
  public student: IStudent;
  errorMessage: string;
  private sub: Subscription;

  constructor(private _route: ActivatedRoute,
    private _router: Router,
    private _studentService: StudentService) { }

  ngOnInit() {
    this.sub = this._route.params.subscribe(
      params => {
        let id = +params['id'];
        this.getStudent(id);
      });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  getStudent(id: number) {
    this._studentService.getStudent(id).subscribe(
      dept => this.student = dept,
      error => this.errorMessage = <any>error);
  }

  onBack(): void {
    this._router.navigate(['/students']);
  }

}
