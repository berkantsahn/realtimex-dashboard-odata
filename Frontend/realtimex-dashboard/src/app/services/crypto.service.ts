import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';

@Injectable({
  providedIn: 'root'
})
export class CryptoService {
  private readonly secretKey = 'RealTimeX-SecretKey-2024';

  constructor() {}

  encrypt(value: string): string {
    try {
      return CryptoJS.AES.encrypt(value, this.secretKey).toString();
    } catch (error) {
      console.error('Şifreleme hatası:', error);
      throw new Error('Şifreleme işlemi başarısız oldu');
    }
  }

  decrypt(value: string): string {
    try {
      const bytes = CryptoJS.AES.decrypt(value, this.secretKey);
      return bytes.toString(CryptoJS.enc.Utf8);
    } catch (error) {
      console.error('Şifre çözme hatası:', error);
      throw new Error('Şifre çözme işlemi başarısız oldu');
    }
  }
} 