import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {VisualizarCarruselComponent} from "./visualizar-carrusel.component";
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { LanguageDirectiveModule } from '../directives/language/language.directive.module';

@NgModule({
	declarations: [VisualizarCarruselComponent],
	exports: [VisualizarCarruselComponent],
	imports: [
		CommonModule,
		NzSpinModule,
		LanguageDirectiveModule
	]
})
export class VisualizarCarruselModule {}
