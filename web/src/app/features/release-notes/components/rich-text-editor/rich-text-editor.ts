import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { QuillEditorComponent, QuillModules } from 'ngx-quill';

/** Rich text editor for release note content, producing semantic HTML. */
@Component({
  selector: 'app-rich-text-editor',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, QuillEditorComponent],
  templateUrl: './rich-text-editor.html',
  styleUrl: './rich-text-editor.scss',
})
export class RichTextEditor {
  readonly control = input.required<FormControl<string>>();
  readonly placeholder = input('Describe what changed…');

  protected readonly modules: QuillModules = {
    toolbar: [
      [{ header: [2, 3, false] }],
      ['bold', 'italic', 'underline', 'strike'],
      [{ list: 'ordered' }, { list: 'bullet' }],
      ['blockquote', 'code-block', 'link', 'image'],
      ['clean'],
    ],
  };
}
