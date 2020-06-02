import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {CategoriesBreadCrumbsComponent} from './categories-bread-crumbs.component';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {RouterTestingModule} from '@angular/router/testing';
import {CategoryCardComponent} from '../categories-generation/category-card/category-card.component';
import {CategoriesService} from '../categories-generation/categories/categories.service';
import {of} from 'rxjs';
import {CategoryBreadCrumb, CategoryBreadCrumbBuilder, CategoryBuilder} from '../categories-generation/models/category';
import {GetRandom} from '../test-utils/GetRandom';
import Mock = jest.Mock;


export const createSpyObj = (baseName, methodNames): { [key: string]: Mock<any> } => {
  let obj: any = {};

  for (let i = 0; i < methodNames.length; i++) {
    obj[methodNames[i]] = jest.fn();
  }

  return obj;
};

describe('CategoriesBreadCrumbsComponent', () => {
  let fixture: ComponentFixture<CategoriesBreadCrumbsComponent>;
  let mockCategoryService;

  beforeEach(async(() => {
    mockCategoryService = createSpyObj('CategoriesService', ['getCategoryBreadCrumbs']);
    // mockCategoryService = jest.mock('', () => ({getCategoryBreadCrumbs: jest.fn()}));
    // mockCategoryService = jest.spyOn(CategoriesService, 'getCategoryBreadCrumbs');
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule.withRoutes([])],
      declarations: [CategoriesBreadCrumbsComponent],
      providers: [{provide: CategoriesService, useValue: mockCategoryService}]
    }).compileComponents();
  }));

  it('should create', () => {
    fixture = TestBed.createComponent(CategoriesBreadCrumbsComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });
  it('ngInit should call getCategoryBreadCrumbs', () => {
     mockCategoryService.getCategoryBreadCrumbs.mockReturnValue(of(new CategoryBreadCrumb()));
    fixture = TestBed.createComponent(CategoriesBreadCrumbsComponent);
    fixture.detectChanges();

    // expect(mockCategoryService.getCategoryBreadCrumbs).toHaveBeenCalledTimes(1);
    expect(mockCategoryService.getCategoryBreadCrumbs).toHaveBeenCalledTimes(1);
  });
  it('should populate breadCrumbsText(1)', () => {
    const fakeBreadCrumb = CategoryBreadCrumbBuilder.basic();
    // mockCategoryService.getCategoryBreadCrumbs.and.returnValue(of(fakeBreadCrumb));
    mockCategoryService.getCategoryBreadCrumbs.mockReturnValue(of(fakeBreadCrumb));
    fixture = TestBed.createComponent(CategoriesBreadCrumbsComponent);
    fixture.detectChanges();

    expect(fixture.componentInstance.breadCrumbsText).toEqual(fakeBreadCrumb.name);
  });
  it('should populate breadCrumbsText(2)', () => {
    const fakeChildBreadCrumb = CategoryBreadCrumbBuilder.basic();
    const fakeBreadCrumb = CategoryBreadCrumbBuilder.basic().setChild(fakeChildBreadCrumb);
    mockCategoryService.getCategoryBreadCrumbs.and.returnValue(of(fakeBreadCrumb));
    fixture = TestBed.createComponent(CategoriesBreadCrumbsComponent);
    fixture.detectChanges();

    expect(fixture.componentInstance.breadCrumbsText).toEqual(fakeBreadCrumb.name + '>' + fakeChildBreadCrumb.name);
  });
  it('should populate breadCrumbsText(3)', () => {
    const fakeGrandChildBreadCrumb = CategoryBreadCrumbBuilder.basic();
    const fakeChildBreadCrumb = CategoryBreadCrumbBuilder.basic().setChild(fakeGrandChildBreadCrumb);
    const fakeBreadCrumb = CategoryBreadCrumbBuilder.basic().setChild(fakeChildBreadCrumb);
    mockCategoryService.getCategoryBreadCrumbs.and.returnValue(of(fakeBreadCrumb));
    fixture = TestBed.createComponent(CategoriesBreadCrumbsComponent);
    fixture.detectChanges();

    expect(fixture.componentInstance.breadCrumbsText)
      .toEqual(fakeBreadCrumb.name + '>' + fakeChildBreadCrumb.name + '>' + fakeGrandChildBreadCrumb.name);
  });
});
