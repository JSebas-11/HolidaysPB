import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

/** Placeholder for a list with no rows: icon + message. Extra content (e.g. a button) goes inside the tag. */
@Component({
  selector: 'app-empty-state',
  imports: [MatIconModule],
  templateUrl: './empty-state.html',
  styleUrl: './empty-state.scss',
})
export class EmptyState {
  /** Material Symbols icon name. */
  readonly icon = input.required<string>();
  readonly message = input.required<string>();
}
