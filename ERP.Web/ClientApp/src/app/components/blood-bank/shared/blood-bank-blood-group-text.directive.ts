import { Directive, HostListener, Input, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { BLOOD_BANK_BLOOD_GROUP_MAX_LENGTH, sanitizeBloodBankBloodGroup } from './blood-bank-input.utils';

@Directive({
    selector: 'input[appBloodBankBloodGroupText]',
    standalone: false
})
export class BloodBankBloodGroupTextDirective {
    @Input() appBloodBankBloodGroupTextMaxLength = BLOOD_BANK_BLOOD_GROUP_MAX_LENGTH;

    constructor(@Optional() @Self() private ngControl: NgControl) { }

    @HostListener('keypress', ['$event'])
    onKeypress(event: KeyboardEvent): void {
        const key = event.key;
        if (key === 'Backspace' || key === 'Delete' || key === 'Tab' || key === 'ArrowLeft' || key === 'ArrowRight') {
            return;
        }
        if (!/^[a-zA-Z+\-\s]$/.test(key)) {
            event.preventDefault();
        }
    }

    @HostListener('input', ['$event'])
    onInput(event: Event): void {
        const input = event.target as HTMLInputElement;
        const sanitized = sanitizeBloodBankBloodGroup(input.value, this.appBloodBankBloodGroupTextMaxLength);
        if (input.value === sanitized) {
            return;
        }

        input.value = sanitized;
        this.ngControl?.control?.setValue(sanitized);
    }
}
