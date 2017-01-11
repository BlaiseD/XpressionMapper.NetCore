import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Ng2PaginationModule } from 'ng2-pagination';
import { StudentDeleteComponent } from '../student-delete/student-delete.component';
import { StudentEditComponent } from '../student-edit/student-edit.component';
import { StudentDetailComponent } from '../student-detail/student-detail.component';
import { StudentListComponent } from '../student-list/student-list.component';
import { StudentCreateComponent } from '../student-create/student-create.component';

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild([
      { path: 'students', component: StudentListComponent },
      { path: 'student-create', component: StudentCreateComponent },
      { path: 'student-edit/:id', component: StudentEditComponent },
      { path: 'student-delete/:id', component: StudentDeleteComponent },
      { path: 'student-detail/:id', component: StudentDetailComponent }
    ]),
    Ng2PaginationModule,
    FormsModule
  ],
  declarations: [
    StudentDeleteComponent,
    StudentEditComponent,
    StudentDetailComponent,
    StudentListComponent,
    StudentCreateComponent
  ]
})
export class StudentModule { }
