import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MenuLateralComponent } from './menu-lateral.component';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { RouterModule } from '@angular/router';
import { NzSkeletonModule } from 'ng-zorro-antd/skeleton';
import {VisualizarImagenModule} from "../visualizar-imagen/visualizar-imagen.module";

@NgModule({
	declarations: [
		MenuLateralComponent,
	],
	imports: [
		CommonModule,
		NzMenuModule,
		RouterModule,
		NzSkeletonModule,
		VisualizarImagenModule
	],
	exports: [
		MenuLateralComponent,
	]
})
export class MenuLateralModule { }
