import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {CategoriesSearchContainerComponent} from './categories-search-container/categories-search-container.component';
import {CategoriesSearchFiltersComponent} from './categories-search-filters/categories-search-filters.component';

import {CategoriesSearchHttpService} from './categories-search-http.service';
import {MatSliderModule} from '@angular/material/slider';
import {LayoutModule} from '@angular/cdk/layout';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatButtonModule} from '@angular/material/button';
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatIconModule} from '@angular/material/icon';
import {MatListModule} from '@angular/material/list';
import {RouterModule} from '@angular/router';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {CategoriesSearchListResultComponent} from './categories-search-list-result/categories-search-list-result.component';
// tslint:disable-next-line:max-line-length
import {CategoriesSearchTreeDetailResultComponent} from './categories-search-tree-detail-result/categories-search-tree-detail-result.component';
import {CardResultBasicComponent} from './card-result-basic/card-result-basic.component';
import {CardResultDetailComponent} from './card-result-detail/card-result-detail.component';
import {MatCardModule} from '@angular/material/card';
import {MatTreeModule} from '@angular/material/tree';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {LoggedInGuard} from '../guards/logged-in.guard';
import {StoreModule} from '@ngrx/store';
import {reducer} from './state/categories-search.reducer';
import {EffectsModule} from '@ngrx/effects';
import {CategoriesSearchEffects} from './state/categories-search.effects';
import {AppModule} from '../app.module';
import {CategoriesBreadCrumbsModule} from '../categories-bread-crumbs/categories-bread-crumbs.module';
import {MatButtonToggleModule} from '@angular/material/button-toggle';

@NgModule({
  declarations: [
    CategoriesSearchContainerComponent,
    CategoriesSearchFiltersComponent,
    CategoriesSearchListResultComponent,
    CategoriesSearchTreeDetailResultComponent,
    CardResultBasicComponent,
    CardResultDetailComponent
  ],
  imports: [
    CommonModule,
    MatSliderModule,
    LayoutModule,
    MatToolbarModule,
    MatButtonModule,
    MatSidenavModule,
    MatIconModule,
    MatListModule,
    RouterModule.forChild([
      {path: '', component: CategoriesSearchContainerComponent},
    ]),
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTreeModule,
    MatCheckboxModule,
    FormsModule,
    StoreModule.forFeature('categoriesSearchReducer', reducer),
    EffectsModule.forFeature([CategoriesSearchEffects]),
    CategoriesBreadCrumbsModule,
    MatButtonToggleModule
  ],
  providers: [CategoriesSearchHttpService],
})
export class CategoriesSearchModule {
}
