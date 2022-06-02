import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {VisualizarImagenComponent} from "./visualizar-imagen.component";


@NgModule({
	declarations: [VisualizarImagenComponent],
	exports: [VisualizarImagenComponent],
	imports: [
		CommonModule
	]
})
export class VisualizarImagenModule {
}
