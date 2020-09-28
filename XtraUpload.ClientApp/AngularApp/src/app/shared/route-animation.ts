import { trigger, transition, style, query, group, animate, keyframes, animateChild} from '@angular/animations';
import { AbsoluteSourceSpan } from '@angular/compiler';

export const fader = trigger('routeAnimations', [
  transition('* <=> *', [
  // Hide the current and the requested page
    query(':enter, :leave', [
      style({
        position: 'absolute',
        left: 0,
        width: '100%',
        opacity: 0,
        transform: 'scale(0)'
      })
    ]),
    // Show the requested page
    query(':enter', [
      animate('0.6s ease-in',
      style({ opacity: 1, transform: 'scale(1)'})
      )
    ])
  ])
]);
