import { ChangeDetectionStrategy, Component, input, model } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Select } from 'primeng/select';

/** A labelled version picker: choose one of the demo's versions or type any other. */
@Component({
  selector: 'app-version-field',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, Select],
  templateUrl: './version-field.html',
  styleUrl: './version-field.scss',
})
export class VersionField {
  readonly inputId = input.required<string>();
  readonly label = input.required<string>();
  readonly hint = input('');
  readonly versions = input<string[]>([]);
  readonly placeholder = input('Any');
  readonly value = model('');
}
