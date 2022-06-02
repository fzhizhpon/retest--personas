import { animate, style, transition, trigger } from '@angular/animations';
import { Component } from '@angular/core';
import { Router, Event as RouterEvent,
	NavigationStart,
	NavigationEnd,
	NavigationCancel,
	NavigationError } from '@angular/router';
//import { NzDatePickerComponent } from 'ng-zorro-antd/date-picker';
import { LoadingService } from './services/common/loading/loading.service';

@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.scss'],
	animations: [
		trigger(
			'inOutAnimation',
			[
				transition(
					':enter',
					[
						style({ opacity: 0 }),
						animate('0.4s ease-out',
							style({ opacity: 1 }))
					]
				)
			]
		)
	]
})
export class AppComponent {
	title = 'VimaCoop';

	showLoading = false;

	constructor(
		private router: Router,
		private loading: LoadingService,
		//private nzdate:NzDatePickerComponent,
	) {
		//this.nzdate.nzFormat='dd/MM/yyyy';
		this.loading.loading$.subscribe(loadings => this.showLoading = loadings.length > 0)

		router.events.subscribe((event: RouterEvent) => {
			this.navigationInterceptor(event)
		})

		this.setAutocompleteOff();

		this.escalarPantalla();
	}

	escalarPantalla() {
		const width = document.body.offsetWidth

		if (width < 1400) {
			const el = <any> (document.querySelector("body"));

			if (el) {
				// el.style.zoom = width/1400;
				// console.log(width * (1 + (width/1400)));
				// document.body.style.width = `${width * (1 + (width/1400))}px`
				// const r = <any> document.querySelector(':root');
				// r.style.setProperty('--default-size', `${width*9/1400}px`);
			}
		}
	}

	setAutocompleteOff() {
		const body = document.getElementsByTagName('body')[0]

		const observer = new MutationObserver(list => {
			const elements = document.querySelectorAll('[autocomplete="off"]')
			elements.forEach(el => el.setAttribute('autocomplete', `new-${Date.now().toString()}`) )
		});

		const attributes = false;
		const childList = true;
		const subtree = true;

		observer.observe(body, { attributes, childList, subtree });
	}

	navigationInterceptor(event: RouterEvent): void {
		if (event instanceof NavigationStart) {
			this.loading.show()
		}
		if (event instanceof NavigationEnd) {
			this.loading.hide()
		}

		// Set loading state to false in both of the below events to hide the spinner in case a request fails
		if (event instanceof NavigationCancel) {
			this.loading.hide()
		}
		if (event instanceof NavigationError) {
			this.loading.hide()
		}
	}
}
