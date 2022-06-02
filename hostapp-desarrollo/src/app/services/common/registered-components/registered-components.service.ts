import { Injectable } from '@angular/core';

@Injectable({
	providedIn: 'root'
})
export class RegisteredComponentsService {

	private componentes!: Comp[];

	async obtenerComponente(codigoComponente: number): Promise<any> {
		await this.cargarComponentes()

		const busqueda = this.componentes.find(c => c.codigo == codigoComponente)

		if (busqueda == null) throw new Error(`El componente ${codigoComponente} no ha sido encontrado`)

		return busqueda.componente;
	}

	private async cargarComponentes() {
		if (this.componentes == null) {
			this.componentes = []

			this.componentes.push({
				codigo: 7,
				componente: (await import('projects/mantenimiento/src/app/components/bienes/bienes.component')).BienesComponent
			});
			this.componentes.push({
				codigo: 3,
				componente: (await import('projects/mantenimiento/src/app/components/referencia-financiera/referencia-financiera.component')).ReferenciaFinancieraComponent
			});
			this.componentes.push({
				codigo: 10,
				componente: (await import('projects/mantenimiento/src/app/components/trabajos/trabajos.component')).TrabajosComponent
			});
		}
	}

}

interface Comp {
	codigo: number;
	componente: any;
}
