import { Component, Input } from '@angular/core';
import { TabLayouts } from '../tabs.component';

@Component({
	selector: 'vs-tab',
	template: `<ng-container *ngIf="selected"><ng-content></ng-content></ng-container>`,
	styleUrls: ['../tabs.component.scss']
})
export class TabComponent {

	@Input('selected') selected = false;
	@Input('label') label = '';
	@Input('icon') icon: string | null = null;
	@Input('disabled') disabled = false;

	@Input('disabledLayouts') disabledLayouts!: TabLayouts;

}
