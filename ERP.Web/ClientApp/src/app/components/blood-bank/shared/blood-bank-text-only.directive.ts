import { Directive, HostListener, Input, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { BLOOD_BANK_NAME_MAX_LENGTH, sanitizeBloodBankName } from './blood-bank-input.utils';

@Directive({
    selector: 'input[appBloodBankTextOnly]',
    standalone: false
})
export class BloodBankTextOnlyDirective {
    @Input() appBloodBankTextOnlyMaxLength = BLOOD_BANK_NAME_MAX_LENGTH;

    constructor(@Optional() @Self() private ngControl: NgControl) { }

    @HostListener('keypress', ['$event'])
    onKeypress(event: KeyboardEvent): void {
        const key = event.key;
        if (key === 'Backspace' || key === 'Delete' || key === 'Tab' || key === 'ArrowLeft' || key === 'ArrowRight') {
            return;
        }
        if (!/^[a-zA-Z\s.'-]$/.test(key)) {
            event.preventDefault();
        }
    }

    @HostListener('input', ['$event'])
    onInput(event: Event): void {
        const input = event.target as HTMLInputElement;
        const sanitized = sanitizeBloodBankName(input.value, this.appBloodBankTextOnlyMaxLength);
        if (input.value === sanitized) {
            return;
        }

        input.value = sanitized;
        this.ngControl?.control?.setValue(sanitized);
    }
}
