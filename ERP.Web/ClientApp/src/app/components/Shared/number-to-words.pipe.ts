import { Pipe, PipeTransform } from '@angular/core';

// Function to convert numbers to words
function convertNumberToWords(num: number): string {
  const a = [
    '', 'one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine',
    'ten', 'eleven', 'twelve', 'thirteen', 'fourteen', 'fifteen', 'sixteen',
    'seventeen', 'eighteen', 'nineteen'
  ];
  const b = [
    '', '', 'twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy',
    'eighty', 'ninety'
  ];

  if (num === 0) return 'zero';

  function numToWords(n: number): string {
    if (n < 20) return a[n];
    if (n < 100) return b[Math.floor(n / 10)] + (n % 10 ? ' ' + a[n % 10] : '');
    if (n < 1000) return a[Math.floor(n / 100)] + ' hundred' + (n % 100 ? ' ' + numToWords(n % 100) : '');
    if (n < 1000000) return numToWords(Math.floor(n / 1000)) + ' thousand' + (n % 1000 ? ' ' + numToWords(n % 1000) : '');
    if (n < 1000000000) return numToWords(Math.floor(n / 1000000)) + ' million' + (n % 1000000 ? ' ' + numToWords(n % 1000000) : '');
    return 'Number too large';
  }

  return numToWords(num);
}

@Pipe({
  name: 'numberToWords'
})
export class NumberToWordsPipe implements PipeTransform {
  transform(value: number): string {
    if (!value && value !== 0) return '';
    return convertNumberToWords(value);
  }
}
