import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { Toast } from 'primeng/toast';

/** Root component: routed content plus the app-wide toast and confirm dialog. */
@Component({
  selector: 'app-root',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterOutlet, Toast, ConfirmDialog],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {}
