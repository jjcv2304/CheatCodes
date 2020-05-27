import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {CategoriesBreadCrumbsRoutingModule} from './categories-bread-crumbs-routing.module';
import {CategoriesBreadCrumbsComponent} from './categories-bread-crumbs.component';


@NgModule({
  imports: [CommonModule, CategoriesBreadCrumbsRoutingModule],
  declarations: [CategoriesBreadCrumbsComponent],
  exports: [CategoriesBreadCrumbsComponent]
})
export class CategoriesBreadCrumbsModule {
}
