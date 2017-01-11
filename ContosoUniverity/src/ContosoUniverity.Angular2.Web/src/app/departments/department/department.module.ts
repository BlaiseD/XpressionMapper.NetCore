import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule} from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DepartmentListComponent } from '../department-list/department-list.component';
import { DepartmentCreateComponent } from '../department-create/department-create.component';
import { DepartmentEditComponent } from '../department-edit/department-edit.component';
import { DepartmentDetailComponent } from '../department-detail/department-detail.component';
import { DepartmentDeleteComponent } from '../department-delete/department-delete.component';

@NgModule({
  imports: [
    CommonModule,
        RouterModule.forChild([
            { path: 'departments', component: DepartmentListComponent },
            { path: 'department-create', component: DepartmentCreateComponent },
            { path: 'department-edit/:id', component: DepartmentEditComponent },
            { path: 'department-delete/:id', component: DepartmentDeleteComponent },
            { path: 'department-detail/:id', component: DepartmentDetailComponent }
        ])
        ,FormsModule
  ],
  declarations: [
    DepartmentListComponent,
    DepartmentCreateComponent,
    DepartmentEditComponent,
    DepartmentDetailComponent,
    DepartmentDeleteComponent
  ]
})
export class DepartmentModule { }
