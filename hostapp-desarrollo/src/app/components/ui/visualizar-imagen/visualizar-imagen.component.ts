import {AfterContentInit, Component, Input} from "@angular/core";
import {MultimediaDetalleService} from "../../../../../projects/marketing/src/app/services/multimediadetalle.service";

@Component({
	selector: 'vs-visualizar-imagen',
	template: `
		<img class="rounded-sm display-block w-full" [src]="imagenActual">
	`
})
export class VisualizarImagenComponent implements AfterContentInit {

	@Input("codigoEspacio") codigoEspacio: any;
	imagenActual = null;

	constructor(
		private _multimediaDetalleService: MultimediaDetalleService
	) {
	}

	async obtenerImagenes() {
		await this._multimediaDetalleService.obtenerMultimediaDetalles(this.codigoEspacio).toPromise()
			.then((res) => {
				this.imagenActual = res.resultado[0].url;
			})
	}

	ngAfterContentInit(): void {
		this.obtenerImagenes();
	}
}
