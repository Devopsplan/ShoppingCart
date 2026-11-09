import { ToIntArrayPipe } from './to-int-array.pipe';

describe('ToIntArrayPipe', () => {
  let pipe: ToIntArrayPipe;

  beforeEach(() => {
    pipe = new ToIntArrayPipe();
  });

  it('converts comma-separated values to integers', () => {
    expect(pipe.transform('1,2,-3')).toEqual([1, 2, -3]);
  });
});
