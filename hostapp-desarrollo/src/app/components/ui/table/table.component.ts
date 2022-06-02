import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
	selector: 'vs-table',
	templateUrl: './table.component.html',
	styleUrls: ['./table.component.scss'],
})
export class TableComponent {

	localData!: any[];
	data!: any[];

	@Input('showPagination') showPagination = true;
	@Input('loading') showLoading = false;

	@Input('data')
	set _data(value: any[]) {
		if (value) {
			this.localData = value;
			this.setPagination();
			this.setPagesNumber();
		}
	}

	@Input('index')
	set _index(index: number) {
		this.changeIndex(index)
	}

	indexSelected = 0;
	@Input('pageSize') pageSize = 15;
	@Input('totalPages') totalPages = 0;
	@Output('indexChange') indexEmitter: EventEmitter<number> = new EventEmitter<number>();

	// constructor() { }

	changeIndex(index: number) {
		if (index != this.indexSelected) {
			this.indexSelected = index
			this.setPagination()
			this.indexEmitter.emit(index)
		}
	}

	setPagination() {
		if (!this.showPagination) {
			this.data = this.localData
			return;
		}

		this.data = this.localData.slice(this.indexSelected * this.pageSize, (this.indexSelected + 1) * this.pageSize)
	}

	setPagesNumber() {
		if (!this.showPagination) return;

		if (!this.totalPages)
			this.totalPages = Math.ceil(this.localData.length / this.pageSize);
	}

}
