import { Component } from '@angular/core';
import { MenuModule } from 'primeng/menu';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-station-link-menu',
  imports: [MenuModule],
  templateUrl: './station-link-menu.component.html',
  styleUrl: './station-link-menu.component.css',
  
  template: `
    <p-menubar [model]="items"></p-menubar>
  `
})
export class StationLinkMenuComponent {
items: MenuItem[] = [
    {
      label: 'Mapa',
      icon: 'pi-map',
      items: [
        { label: 'Nowy', icon: 'pi pi-fw pi-plus' },
        { label: 'Otwórz', icon: 'pi pi-fw pi-download' }
      ]
    },
    {
      label: 'Edycja',
      icon: 'pi pi-fw pi-pencil',
      items: [
        { label: 'Cofnij', icon: 'pi pi-fw pi-undo' },
        { label: 'Ponów', icon: 'pi pi-fw pi-redo' }
      ]
    },
    {
      label: 'Pomoc',
      icon: 'pi pi-fw pi-info',
      items: [
        { label: 'Dokumentacja', icon: 'pi pi-fw pi-book' }
      ]
    }
  ];
}
