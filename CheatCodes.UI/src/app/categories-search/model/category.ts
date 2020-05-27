export interface ICategoryBasic {
  id: number;
  name: string;
  description: string;
  parentId: number;
  color: string;
}

export class CategoryBasic implements ICategoryBasic {
  public constructor(init?: Partial<CategoryBasic>) {
    Object.assign(this, init);
  }

  id: number = null;
  name: string = null;
  description = '';
  parentId: number;
  color = 'lightskyblue';
}

export interface ICategoryTree {
  id: number;
  name: string;
  description: string;
  parentId: number;
  color: string;
  childs: ICategoryTree[];
}

export class CategoryTree implements ICategoryTree {
  public constructor(init?: Partial<CategoryTree>) {
    Object.assign(this, init);
  }

  id: number = null;
  name: string = null;
  description = '';
  parentId: number;
  color = 'lightskyblue';
  childs: CategoryTree[];
}
