import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CabeceraComponent } from './cabecera.component';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { VsPanelNotificacionesModule } from '../panel-notificaciones/panel-notificaciones.module';
import { NzPopoverModule } from 'ng-zorro-antd/popover';
import {LanguageDirectiveModule} from 'src/app/components/ui/directives/language/language.directive.module';
import {VisualizarImagenModule} from "../visualizar-imagen/visualizar-imagen.module";

@NgModule({
	declarations: [CabeceraComponent],
	exports: [CabeceraComponent],
	imports: [
		CommonModule,
		NzInputModule,
		NzIconModule,
		NzButtonModule,
		VsPanelNotificacionesModule,
		NzPopoverModule,
		LanguageDirectiveModule,
		VisualizarImagenModule
	],
})
export class CabeceraModule { }
