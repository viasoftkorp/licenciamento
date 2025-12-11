import { ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';

class CnpjOrCpfValidator {
    private isCpfValid(cpf: string) {
        // Make sure string contains numbers only
        if (!cpf) { return false; }
        cpf = cpf.replace(/\D/g, '');

        // Catch commonly known invalid CPFs
        if (cpf === '' ||
            cpf.length !== 11 ||
            cpf === '00000000000' ||
            cpf === '11111111111' ||
            cpf === '22222222222' ||
            cpf === '33333333333' ||
            cpf === '44444444444' ||
            cpf === '55555555555' ||
            cpf === '66666666666' ||
            cpf === '77777777777' ||
            cpf === '88888888888' ||
            cpf === '99999999999'
        ) {
            return false;
        }

        // Check first digit
        let firstSum = 0;
        for (let i = 0; i < 9; i++) {
            firstSum += parseInt(cpf.charAt(i), 10) * (10 - i);
        }

        let firstRev = 11 - (firstSum % 11);
        if (firstRev === 10 || firstRev === 11) { firstRev = 0; }
        if (firstRev !== parseInt(cpf.charAt(9), 10)) { return false; }

        // Check second digit
        let secondSum = 0;
        for (let i = 0; i < 10; i++) {
            secondSum += parseInt(cpf.charAt(i), 10) * (11 - i);
        }

        let secondRev = 11 - (secondSum % 11);
        if (secondRev === 10 || secondRev === 11) { secondRev = 0; }
        if (secondRev !== parseInt(cpf.charAt(10), 10)) { return false; }

        // All good!
        return true;
    }

    private isCnpjValid(cnpj: string) {
        if (!cnpj) { return false; }
        // Make sure string contains numbers only
        cnpj = cnpj.replace(/\D/g, '');

        // permitimos esse cnpj para estrangeiros
        const cnpjForForeigner = '00000000000000';
        if(cnpj == cnpjForForeigner) return true;

        // Catch commonly known invalid CNPJs
        if (cnpj === '' ||
            cnpj.length !== 14 ||
            cnpj === '11111111111111' ||
            cnpj === '22222222222222' ||
            cnpj === '33333333333333' ||
            cnpj === '44444444444444' ||
            cnpj === '55555555555555' ||
            cnpj === '66666666666666' ||
            cnpj === '77777777777777' ||
            cnpj === '88888888888888' ||
            cnpj === '99999999999999'
        ) {
            return false;
        }

        // Check First digit
        let size = cnpj.length - 2;
        let numbers = cnpj.substring(0, size);
        const digits = cnpj.substring(size);
        let sum = 0;
        let pos = size - 7;
        let result: number;

        for (let i = size; i >= 1; i--) {
            sum += Number(numbers.charAt(size - i)) * pos--;
            if (pos < 2) { pos = 9; }
        }

        result = sum % 11 < 2 ? 0 : 11 - sum % 11;
        if (result !== Number(digits.charAt(0))) { return false; }

        // Check second digit
        size = size + 1;
        numbers = cnpj.substring(0, size);
        sum = 0;
        pos = size - 7;

        for (let i = size; i >= 1; i--) {
            sum += Number(numbers.charAt(size - i)) * pos--;
            if (pos < 2) { pos = 9; }
        }

        result = sum % 11 < 2 ? 0 : 11 - sum % 11;
        if (result !== Number(digits.charAt(1))) { return false; }

        // All good!
        return true;
    }

    private cnpjOrCpfValidatorWithComma(cnpjOrCpf: string): boolean {
        const list = cnpjOrCpf.split(',');
        let output = true;
        if (list.length !== 1) {
            list.forEach(cnpjCpf => {
                if (cnpjCpf.length === 11) {
                    const result = this.isCpfValid(cnpjCpf);
                    if (result === false) {
                        output = result;
                    }
                }
                if (cnpjCpf.length === 14) {
                    const result = this.isCnpjValid(cnpjCpf);
                    if (result === false) {
                        output = result;
                    }
                }
                if (cnpjCpf.length !== 14 && cnpjCpf.length !== 11) {
                    output = false;
                }
            });
            return output;
        }
        if (this.isCnpjValid(list[0]) || this.isCpfValid(list[0])) {
            return output;
        }
        return false;
    }

    private isCnpjOrCpfValid(cnpjOrCpf: string){
      if (cnpjOrCpf.length === 11) {
        return this.isCpfValid(cnpjOrCpf);
      }

      if (cnpjOrCpf.length === 14) {
        return this.isCnpjValid(cnpjOrCpf);
      }

      return false;
    }

    cnpjOrCpfWithCommas(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (!control.value || this.cnpjOrCpfValidatorWithComma(control.value)) {
                return null;
            }
            return {cpfAndCnpjWithCommasInvalid: true};
        };
    }

    cnpjOrCpf(): ValidatorFn {
      return (control: AbstractControl): ValidationErrors | null => {
          if (!control.value || this.isCnpjOrCpfValid(control.value)) {
            return null;
          }
          return {cpforCnpjInvalid: true};
      };
  }
}

export const cpfOrCnpjValidator = new CnpjOrCpfValidator();
