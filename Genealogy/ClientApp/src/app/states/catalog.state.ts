import { AddCatalogItem, FetchCatalogItem, FetchCatalogList, FetchPurchaseList, GetCatalogItemsCount, UpdateCatalogItem } from '@actions';
import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BusinessObject, BusinessObjectInDto, BusinessObjectOutDto } from '@models';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { BusinessObjectService } from '@repository';
import { tap } from 'rxjs/operators';
import { first } from 'lodash';

export interface CatalogStateModel {
  item: BusinessObjectInDto;
  list: BusinessObjectInDto[];
  count: number;
  purchaseList: BusinessObjectInDto[];
}

@State<CatalogStateModel>({
  name: 'catalog',
  defaults: {
    item: null,
    list: [],
    count: null,
    purchaseList: [],
  },
})
@Injectable()
export class CatalogState {
  constructor(private boService: BusinessObjectService) {}

  @Selector()
  static list({ list }: CatalogStateModel): BusinessObject[] {
    return list;
  }

  @Selector()
  static item({ item }: CatalogStateModel): BusinessObject {
    return item;
  }

  @Selector()
  static count({ count }: CatalogStateModel): number {
    return count;
  }

  @Selector()
  static purchaseList({ purchaseList }: CatalogStateModel): BusinessObject[] {
    return purchaseList;
  }

  @Action(FetchCatalogList)
  fetchCatalogList(ctx: StateContext<CatalogStateModel>, { payload: filter }) {
    const params: HttpParams = filter;
    return this.boService.FetchBusinessObjectList(params).pipe(tap(list => ctx.patchState({ list })));
  }

  @Action(FetchCatalogItem)
  fetchCatalogItem(ctx: StateContext<CatalogStateModel>, { payload: filter }) {
    const params: HttpParams = filter;
    return this.boService.FetchBusinessObject(params).pipe(tap(items => ctx.patchState({ item: first(items) })));
  }

  @Action(AddCatalogItem)
  addCatalogItem(ctx: StateContext<CatalogStateModel>, { payload }) {
    const body: BusinessObjectOutDto = payload;
    return this.boService.AddBusinessObject(body).pipe(tap(item => ctx.patchState({ item })));
  }

  @Action(UpdateCatalogItem)
  updateCatalogItem(ctx: StateContext<CatalogStateModel>, { payload }) {
    const body: BusinessObjectOutDto = payload;
    return this.boService.UpdateBusinessObject(body).pipe(tap(item => ctx.patchState({ item })));
  }

  @Action(GetCatalogItemsCount)
  getCatalotItemsCount(ctx: StateContext<CatalogStateModel>, { payload: filter }) {
    const params: HttpParams = filter;
    return this.boService.GetBusinessObjectsCount(params).pipe(tap(({ count }) => ctx.patchState({ count })));
  }

  @Action(FetchPurchaseList)
  fetchPurchaseList(ctx: StateContext<CatalogStateModel>, { payload: filter }) {
    const params: HttpParams = filter;
    return this.boService.FetchBusinessObjectList(params).pipe(tap(purchaseList => ctx.patchState({ purchaseList })));
  }
}
