import { fireEvent, render } from '@testing-library/react';
import React from 'react';
import { DatesFilterComponent } from './DatesFilterComponent';
import { currentDateLinuxTime, dateToUnixTime } from '../helpers/date-helper';

test('should render DatesFilterComponent', () => {
  const currentDate = currentDateLinuxTime();
  const newDateSelected = (newDate: number) => {};
  render(<DatesFilterComponent currentDate={currentDate} newDateSelected={newDateSelected} />);
});

test('should select particular date', async () => {
  const expectedYear = 2021;
  const expectedMonth = 3;
  const expectedMonthName = 'Apr';
  const currentDate = new Date(expectedYear, expectedMonth, 5);

  const currentDateUnixTime = dateToUnixTime(currentDate);

  const newDateSelected = (newDate: number) => {};
  const { findAllByText } = render(
    <DatesFilterComponent currentDate={currentDateUnixTime} newDateSelected={newDateSelected} />,
  );

  const chipSelectedClassName = 'MuiChip-filled';
  const chipNotSelectedClassName = 'MuiChip-outlined';
  // NOTE: MUI chip finally has structure: <div><span>2024</span></div>
  // The query below finds a span by text. But the style showing whether the chip is selected or not - is applied in a div style
  // That's why we are checking the parent html element here

  // Get all chips with year starting from 2000.
  const allChipsWithYear = await findAllByText(/^2\d{3}$/i);
  expect(allChipsWithYear).toBeTruthy();
  for (const chipWithYear of allChipsWithYear) {
    const year = chipWithYear.innerHTML;
    expect(chipWithYear.parentElement).toBeTruthy();
    if (+year === expectedYear) {
      expect(chipWithYear.parentElement).toHaveClass(chipSelectedClassName);
      expect(chipWithYear.parentElement).not.toHaveClass(chipNotSelectedClassName);
    } else {
      expect(chipWithYear.parentElement).not.toHaveClass(chipSelectedClassName);
      expect(chipWithYear.parentElement).toHaveClass(chipNotSelectedClassName);
    }
  }

  // All month chips
  const allChipsWithMonth = await findAllByText(/[^0-9]{3}$/i);
  expect(allChipsWithMonth).toBeTruthy();
  for (const chipWithMonth of allChipsWithMonth) {
    const month = chipWithMonth.innerHTML;
    expect(chipWithMonth.parentElement).toBeTruthy();
    if (month === expectedMonthName) {
      expect(chipWithMonth.parentElement).toHaveClass(chipSelectedClassName);
      expect(chipWithMonth.parentElement).not.toHaveClass(chipNotSelectedClassName);
    } else {
      expect(chipWithMonth.parentElement).not.toHaveClass(chipSelectedClassName);
      expect(chipWithMonth.parentElement).toHaveClass(chipNotSelectedClassName);
    }
  }
});

test('should select particular date on year and month click', () => {
  const currentDate = currentDateLinuxTime();
  let selectedDate = 0;

  const yearToSelect = 2011;
  const monthToSelect = 'Apr';
  const expectedDate = dateToUnixTime(new Date(yearToSelect, 3, 1));

  const newDateSelected = (newDate: number) => {
    selectedDate = newDate;
  };
  const { getByText } = render(
    <DatesFilterComponent currentDate={currentDate} newDateSelected={newDateSelected} />,
  );

  const yearChip = getByText(yearToSelect.toString());
  expect(yearChip.parentElement).toBeTruthy();
  fireEvent.click(yearChip.parentElement!);

  const monthChip = getByText(monthToSelect);
  expect(monthChip.parentElement).toBeTruthy();
  fireEvent.click(monthChip.parentElement!);

  expect(selectedDate).toBe(expectedDate);
});
