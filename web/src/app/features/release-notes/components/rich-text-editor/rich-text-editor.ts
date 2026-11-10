import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Editor } from 'primeng/editor';

/** Quill formats the toolbar offers; anything else pasted in is dropped. */
const FORMATS = [
  'header',
  'bold',
  'italic',
  'underline',
  'strike',
  'list',
  'blockquote',
  'code-block',
  'link',
  'image',
];

/** Rich text editor for release note content (PrimeNG Editor), producing semantic HTML. */
@Component({
  selector: 'app-rich-text-editor',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, Editor],
  templateUrl: './rich-text-editor.html',
  styleUrl: './rich-text-editor.scss',
})
export class RichTextEditor {
  readonly control = input.required<FormControl<string | null>>();
  readonly placeholder = input('Describe what changed…');

  protected readonly formats = FORMATS;
}
