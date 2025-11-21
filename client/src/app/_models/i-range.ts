export interface IRange {
  value: Date[];
  label: string;
}

export class RangePreDefinirani {
  private ranges: IRange[] = [{
    value: [new Date(new Date().setDate(new Date().getDate() - 7)), new Date()],
    label: 'Zadnjih 7 dni',
  }, {
    value: [new Date(), new Date(new Date().setDate(new Date().getDate() - 30))],
    label: 'Zadnjih 30 dni'
  }, {
    value: [new Date(new Date().setFullYear(new Date().getFullYear() - 1)), new Date()],
    label: 'Zadnjih 12 mesecev'
  }, {
    value: [new Date(new Date().setFullYear(new Date().getFullYear() - 5)), new Date()],
    label: 'Zadnjih 5 let'
  }];

  getRanges(): IRange[] {
    return this.ranges;
  }
}

