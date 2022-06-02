import { AfterContentChecked, AfterContentInit, ChangeDetectorRef, Component, ContentChildren, EventEmitter, Input, OnDestroy, OnInit, Output, QueryList, TemplateRef } from '@angular/core';
import { Subscription } from 'rxjs';
import { TabComponent } from './tab/tab.component';

@Component({
	selector: 'vs-tabs',
	templateUrl: './tabs.component.html',
	styleUrls: ['./tabs.component.scss'],
})
export class TabsComponent implements AfterContentChecked, AfterContentInit, OnDestroy {

	@ContentChildren(TabComponent) tabs!: QueryList<TabComponent>;

	index!: number;

	@Input('index') set indexSetter(value: number) {
		this.index = value;

		if(value != null && this.tabs) {
			this.selectTab(this.tabs.toArray()[value])
		}
	}

	@Output('indexChange') indexEmmiter = new EventEmitter<number>();

	@Input('bodyClass') bodyClass = 'w-full p-20';
	@Input('bodyStyle') bodyStyle = '';

	@Input('leftLayout') leftLayout?: TemplateRef<any>;
	@Input('topLayout') topLayout?: TemplateRef<any>;
	@Input('rigthLayout') rigthLayout?: TemplateRef<any>;
	@Input('bottomLayout') bottomLayout?: TemplateRef<any>;

	layouts: TabLayouts = {
		left: true,
		rigth: true,
		bottom: true,
		top: true
	};

	tabSubscription!: Subscription;

	constructor(
		private cdr: ChangeDetectorRef
	) {

	}

	ngAfterContentChecked(): void {
		this.cdr.detectChanges()
	}

	ngAfterContentInit(): void {
		this.initTabs()
		this.tabSubscription = this.tabs.changes.subscribe(() => this.initTabs())
		this.cdr.detectChanges()
	}

	ngOnDestroy(): void {
		if (this.tabSubscription != null) {
			this.tabSubscription.unsubscribe()
		}
	}

	selectTab(tab: TabComponent) {
		if(tab && !tab.disabled) {
			const tabs = this.tabs.toArray();
			for(let i = 0; i < tabs.length; i++) {
				tabs[i].selected = false

				if(tabs[i] == tab) {
					tab.selected = true
					this.indexEmmiter.emit(i)

					this.layouts.left = !tab.disabledLayouts?.left;
					this.layouts.rigth = !tab.disabledLayouts?.rigth;
					this.layouts.top = !tab.disabledLayouts?.top;
					this.layouts.bottom = !tab.disabledLayouts?.bottom;
				}
			}
		}
	}

	initTabs() {
		const activeTabs = this.tabs.filter((tab) => tab.selected);

		if(activeTabs.length === 0) {
			if(this.index != null) {
				this.selectTab(this.tabs.toArray()[this.index])
			} else {
				this.selectTab(this.tabs.first);
			}
		}
	}

}

export interface TabLayouts {
	left?: boolean;
	rigth?: boolean;
	top?: boolean;
	bottom?: boolean;
}
