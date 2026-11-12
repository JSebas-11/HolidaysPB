import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule, MatIconRegistry } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

interface NavLink {
  path: string;
  label: string;
}

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatToolbarModule, MatButtonModule, MatIconModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly links: NavLink[] = [
    { path: '/validacion', label: 'Validación' },
    { path: '/paises', label: 'Países' },
    { path: '/tipos', label: 'Tipos' },
    { path: '/festivos', label: 'Festivos' },
  ];

  constructor() {
    // index.html loads the "Material Symbols" icon font; tell <mat-icon> to use it
    inject(MatIconRegistry).setDefaultFontSetClass('material-symbols-outlined');
  }
}
