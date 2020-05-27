import {Component, OnInit} from '@angular/core';
import {Location} from '@angular/common';
import {CategoriesService} from '../categories/categories.service';

@Component({
  selector: 'app-categories-container',
  templateUrl: './categories-container.component.html',
  styleUrls: ['./categories-container.component.scss']
})
export class CategoriesContainerComponent implements OnInit {

  currentParentId = 0;

  constructor(private location: Location, private categoriesService: CategoriesService) {
    categoriesService.currentCategoriesParentId$.subscribe((newParentId: number) => {
      this.currentParentId = newParentId;
      this.changeCurrentUrl(newParentId);
    });
  }

  ngOnInit() {
  }

  changeCurrentUrl(newParentId: number) {
    const parts = window.location.pathname.split('/');

    if (parts.length > 1 && parts[1] === 'categoryList') {
      parts[2] = newParentId.toString();
      this.changeUrl(parts.join('/'));
    }
  }

  changeUrl(url: string) {
    this.location.replaceState(url);
  }

}
