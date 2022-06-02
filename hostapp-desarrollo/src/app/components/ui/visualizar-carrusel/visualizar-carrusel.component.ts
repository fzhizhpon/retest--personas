import {AfterContentInit, Component, Input, OnDestroy} from "@angular/core";
import {MultimediaDetalleService} from "../../../../../projects/marketing/src/app/services/multimediadetalle.service";

@Component({
	selector: 'vs-visualizar-carrusel',
	template: `
		<ng-container *ngIf="imagenActual === null">
			<div class="flex content-start justify-center  pt-50" style="color: white">
				<div class="h-35 h-35 overflow-visible">
					<nz-spin nzSimple [nzSize]="'large'"></nz-spin>
				</div>
				<p lang langLabel="vs-tabla:cargando" class="w-full display-block font-medium text-center m-0">
					Cargando</p>
			</div>
		</ng-container>
		<div *ngIf="imagenActual" class="w-full" style="max-width: 50rem;">
			<div class="w-full mb-15">
				<img class="shadow-md rounded display-block w-full" [src]="imagenActual">
			</div>
			<div class="w-full flex items-center justify-center flex-no-wrap">
				<ng-container *ngFor="let el of imgs; let index = index">
					<div (click)="actual = index" [ngStyle]="{opacity: actual == index ? '1' : '0.7'}"
						 class="cursor-pointer w-10 h-10 mx-5 display-block rounded-full bg-white"></div>
				</ng-container>
			</div>
		</div>
	`
})
export class VisualizarCarruselComponent implements AfterContentInit, OnDestroy {


	@Input("codigoEspacio") codigoEspacio: any;

	imagenActual = null;
	imgs: any = [];
	actual = 0;
	finalizar = false;

	constructor(
		private _multimediaDetalleService: MultimediaDetalleService
	) {
	}

	cambiarImagen(indice: number, finalizar?: boolean) {
		if (finalizar || finalizar === undefined) {
			return
		} else {
			this.finalizar = false;
			// * asignamos la imagen a mostrar
			this.imagenActual = this.imgs[indice].url;
			const tiempoEspera = this.imgs[indice].tiempo * 1000;
			if (this.actual + 1 >= this.imgs.length) {
				this.actual = 0;
				indice = 0;
				setTimeout(() => {
					this.cambiarImagen(indice, this.finalizar);
				}, tiempoEspera)
			} else {
				this.actual++;
				indice++;
				setTimeout(() => {
					this.cambiarImagen(indice, this.finalizar);
				}, tiempoEspera)
			}
		}
	}

	async obtenerImagenes() {
		await this._multimediaDetalleService.obtenerMultimediaDetalles(this.codigoEspacio).toPromise()
			.then((imagenesRecuperadas) => {
				this.imgs = imagenesRecuperadas.resultado.map((item: any) => {
					return {
						url: item.url,
						orden: item.orden,
						tiempo: item.tiempo
					}
				});
				// * ordenando
				this.imgs.sort(function (a: any, b: any) {
					return a.orden - b.orden;
				});
			})
			.catch((err) => {
				console.error(err);
			})
			.finally(() => {
				this.cambiarImagen(this.actual, this.finalizar);
			})

	}

	ngAfterContentInit(): void {
		this.obtenerImagenes();
	}

	ngOnDestroy(): void {
		this.actual = 0;
		this.finalizar = true;
	}


}
