import {createFeatureSelector, createSelector} from '@ngrx/store';
import * as fromRoot from '../../state/app.state';
import * as fromCategorySearch from './categories-search.reducer';

export interface State extends fromRoot.State {
  categoriesSearch: fromCategorySearch.CategoriesSearchState;
}

const getCategoriesSearchFeatureState = createFeatureSelector<fromCategorySearch.CategoriesSearchState>('categoriesSearchReducer');

export const getCategoryNameFilterAnd = createSelector(
  getCategoriesSearchFeatureState,
  state => state.categoryNameFilterAnd
);
export const getCategoryNameFilterOr = createSelector(
  getCategoriesSearchFeatureState,
  state => state.categoryNameFilterOr
);
export const getFilteredCategories = createSelector(
  getCategoriesSearchFeatureState,
  state => state.filteredCategories
);
export const getCategoryNameFilter = createSelector(
  getCategoriesSearchFeatureState,
  state => state.categoryNameFilter
);

export const getCategoryName2Filter = createSelector(
  getCategoriesSearchFeatureState,
  state => state.categoryName2Filter
);

export const getShowButtonShowSideNav = createSelector(
  getCategoriesSearchFeatureState,
  state => state.showButtonShowSideNav
);
