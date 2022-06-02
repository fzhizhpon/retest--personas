import { Component, Input } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
	selector: 'vs-map',
	templateUrl: './vs-map.component.html',
	styleUrls: ['./vs-map.component.scss']
})
export class VsMapComponent {
	longitud!: number;
	latitud!: number;

	mostrarModalMapa = false;
	@Input('longitud')
	set _longitud(value: number) {
		if (value) {
			this.longitud = value;
		}
	}
	@Input('latitud')
	set _latitud(value: number) {
		if (value) {
			this.latitud = value;
		}
	}

	constructor(
		private _dom: DomSanitizer,
	) {

	}

	handleOk(): void {
		this.mostrarModalMapa = false;
	}

	sanitizeUrl(url: string) {
		return this._dom.bypassSecurityTrustResourceUrl(url)
	}

}
