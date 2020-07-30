import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Table } from '@models';

@Component({
  selector: 'app-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.scss'],
})
export class TableComponent implements OnInit {
  @Input() data: Table.Data;
  @Input() selectedId: string;

  @Output() change: EventEmitter<string> = new EventEmitter();
  @Output() remove: EventEmitter<string> = new EventEmitter();

  constructor() {}

  ngOnInit() {}

  onRemove(value: string) {
    event.stopPropagation();
    this.remove.emit(value);
  }

  onSelect(value: string) {
    event.stopPropagation();
    this.change.emit(value);
  }
}