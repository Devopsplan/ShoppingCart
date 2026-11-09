import { ReversePipe } from './reverse.pipe';

describe('ReversePipe', () => {
  let pipe: ReversePipe;

  beforeEach(() => {
    pipe = new ReversePipe();
  });

  it('returns the values in reverse order without changing the input', () => {
    const values = [1, 2, 3];

    expect(pipe.transform(values)).toEqual([3, 2, 1]);
    expect(values).toEqual([1, 2, 3]);
  });

  it('returns an empty array for empty input', () => {
    expect(pipe.transform([])).toEqual([]);
  });
});
