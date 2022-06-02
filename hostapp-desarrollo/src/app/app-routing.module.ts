import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MantenimientoSharedModule } from 'projects/mantenimiento/src/app/app.module';
import { MarketingSharedModule } from 'projects/marketing/src/app/app.module';
import { SeguridadSharedModule } from 'projects/seguridad/src/app/app.module';
import { SociosSharedModule } from 'projects/socios/src/app/app.module';
import { AutenticacionSharedModule } from '../../projects/autenticacion/src/app/app.module';
import { SesionGuard } from './guards/sesion/sesion.guard';

const routes: Routes = [
	{ path: 'autenticacion', loadChildren: () => import('../../projects/autenticacion/src/app/app.module').then(m => m.AutenticacionSharedModule) },
	{ path: 'inicio', loadChildren: () => import('./pages/inicio/inicio.module').then(m => m.InicioModule), canActivate: [SesionGuard] },
	{ path: '', loadChildren: () => import('./pages/main-layout/main-layout.module').then(m => m.MainLayoutModule) },
	{
		path: '**', redirectTo: 'inicio', pathMatch: 'full'
	},
];

@NgModule({
	imports: [
		RouterModule.forRoot(routes),
		AutenticacionSharedModule.forRoot(),
		MantenimientoSharedModule.forRoot(),
		MarketingSharedModule.forRoot(),
		SociosSharedModule.forRoot(),
		SeguridadSharedModule.forRoot(),
	],
	exports: [RouterModule]
})
export class AppRoutingModule { }
