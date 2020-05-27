export class GetRandom {

  public constructor() {
  }

  static Number(min?: number, max?: number): number {
    min = min || 0;
    max = max || 100;
    return Math.floor(Math.random() * (max - min + 1) + min);
  }

  static String(len?: number): string {
    // CI
    // let s = '';
    // while (s.length < len) {
    //   s += Math.random().toString(36).substr(2, len - s.length);
    // }
    // return s;
    // CS
    if (len === undefined) {
      len = 10;
    }
    let s = '';
    while (len--) {
      s += String.fromCodePoint(Math.floor(Math.random() * (126 - 33) + 33));
    }
    return s;
  }
}
