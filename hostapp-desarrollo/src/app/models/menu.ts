export interface Submenu {
    codigoOpcion: string;
    descripcion: string;
    opcionNet?: string;
    opcionAngular?: string;
}

export interface Menu {
	codigoOpcion: string;
	descripcion: string;
	submenus: Submenu[];
}
