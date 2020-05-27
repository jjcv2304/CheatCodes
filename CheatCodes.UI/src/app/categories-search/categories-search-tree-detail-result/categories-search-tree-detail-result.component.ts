/* tslint:disable:member-ordering */
import {Component, Input, OnChanges, OnInit} from '@angular/core';
import {CategoryTree} from '../model/category';
import {FlatTreeControl} from '@angular/cdk/tree';
import {MatTreeFlatDataSource, MatTreeFlattener, MatTreeNode, MatTreeNodeDef, MatTree} from '@angular/material/tree';
import {BehaviorSubject, Observable} from 'rxjs';

interface FlatNode {
  expandable: boolean;
  name: string;
  description: string;
  parentId: number;
  level: number;
}

@Component({
  selector: 'app-categories-search-tree-detail-result',
  templateUrl: './categories-search-tree-detail-result.component.html',
  styleUrls: ['./categories-search-tree-detail-result.component.scss']
})
export class CategoriesSearchTreeDetailResultComponent implements OnChanges {

  @Input() cardDetail: CategoryTree;
  selectedCard: CategoryTree;
  treeWidth = '25%';

  private _transformer = (node: CategoryTree, level: number) => {
    return {
      expandable: !!node.childs && node.childs.length > 0,
      name: node.name,
      description: node.description,
      parentId: node.parentId,
      level: level,
    };
  }

  treeControl = new FlatTreeControl<FlatNode>(node => node.level, node => node.expandable);

  treeFlattener = new MatTreeFlattener(
    this._transformer,
    node => node.level,
    node => node.expandable,
    node => node.childs);

  dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

  hasChild = (_: number, node: FlatNode) => node.expandable;
  activeNode: CategoryTree;

  ngOnChanges() {
    let cardsDetails: [CategoryTree];
    cardsDetails = [this.cardDetail];
    if (cardsDetails[0] !== undefined) {
      this.dataSource.data = cardsDetails;
    }
  }

  showDetail(node: CategoryTree) {
    this.activeNode = node;
    this.selectedCard = node;
  }

  toggleTreeWidth() {
    if (this.treeWidth === '25%') {
      this.treeWidth = '35%';
    } else if (this.treeWidth === '35%') {
      this.treeWidth = '45%';
    } else if (this.treeWidth === '45%') {
      this.treeWidth = '25%';
    }

  }
}
